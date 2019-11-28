using CryptoHelper;
using Microsoft.AspNetCore.Http;
using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StudyGroups.WebAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        ICourseRepository courseRepository;
        IStudentRepository studentRepository;
        ISubjectRepository subjectRepository;
        ITeacherRepository teacherRepository;
        IUserRepository userRepository;

        public AuthenticationService(ICourseRepository courseRepository, IStudentRepository studentRepository, ISubjectRepository subjectRepository
           , ITeacherRepository teacherRepository, IUserRepository userRepository)
        {
            this.courseRepository = courseRepository;
            this.studentRepository = studentRepository;
            this.subjectRepository = subjectRepository;
            this.teacherRepository = teacherRepository;
            this.userRepository = userRepository;
        }

        public async Task RegisterUserAsync(StudentRegistrationDTO studentRegistrationDTO)
        {
            //todo: validate these values!!
            //check username existence

            if (studentRegistrationDTO.GradeBookExport != null && studentRegistrationDTO.CoursesExport != null)
            {

                var student = new Student();
                student.DateOfBirth = studentRegistrationDTO.DateOfBirth;
                student.Email = studentRegistrationDTO.Email;
                student.FirstName = studentRegistrationDTO.FirstName;
                student.LastName = studentRegistrationDTO.LastName;
                student.GenderType = (int)studentRegistrationDTO.GenderType;
                student.InstagramName = studentRegistrationDTO.InstagramName;
                student.MessengerName = studentRegistrationDTO.MessengerName;
                student.NeptunCode = studentRegistrationDTO.NeptunCode;

                student.Password = Crypto.HashPassword(studentRegistrationDTO.Password);
                student.UserName = studentRegistrationDTO.UserName;

                if (studentRegistrationDTO.Image != null)
                {
                    student.ImagePath = StoreFile(studentRegistrationDTO.Image, "Images");
                }

                studentRepository.CreateUserStudent(student);
                var stud = studentRepository.FindStudentByUserName(student.UserName);

                await ProcessNeptunExportsAsync(stud, studentRegistrationDTO.GradeBookExport, studentRegistrationDTO.CoursesExport);

            }
            else
            {
                throw new Exception("Neptun exports weren't included, or was in bad format. required file: .CSV - comma or semicolon separated, and UTF-8 coded");
            }

        }

        private async Task ProcessNeptunExportsAsync(Student student, IFormFile gradeBook, IFormFile courses)
        {
            //store file
            string courseServerPath = StoreFile(courses, "Exports");
            string gradebookServerPath = StoreFile(gradeBook, "Exports");
            string thisSemester = SemesterManager.GetCurrentSemester();

            //TODO: execute these parallel
            //process subjects
            var gradebookExports = CSVProcesser.ProcessCSV<GradeBookExportModel>(gradebookServerPath);
            var createSubjects = GetNonExistingSubjectsFromExport(gradebookExports);

            //process course
            var courseExports = CSVProcesser.ProcessCSV<CourseExportModel>(courseServerPath);
            var createCourses = await GetNonExistingCoursesFromExportAsync(courseExports, thisSemester);

            //process teachers
            var createTeachers = GetNonExistingTeachersFromExport(courseExports);

            //write all to db if no error
            var subjectsNewlyCreated = new List<Subject>();
            // todo: create list of nodes in neo4j level
            foreach (var sub in createSubjects)
            {
                subjectsNewlyCreated.Add(subjectRepository.Create(sub));
            }

            var teachersNewlyCreated = new List<Teacher>();
            foreach (var teacher in createTeachers)
            {
                teachersNewlyCreated.Add(teacherRepository.Create(teacher));
            }

            //create relationships between nodes:
            //course-subject :BELONGS_TO
            foreach (var crs in createCourses)
            {
                courseRepository.CreateCourseBelongingToSubject(crs.Course, crs.SubjectCode);
            }

            //Teacher-course :TEACHES
            foreach (var courseExport in courseExports)
            {
                teacherRepository.CreateTeachesRelationshipWithCourseParams(courseExport.SubjectCode, courseExport.CourseCode, thisSemester, courseExport.TeacherName);
            }

            //Student-course :ATTENDS
            foreach (var cExp in courseExports)
            {
                var s = courseRepository.GetCourseWithSubject(cExp.CourseCode, cExp.SubjectCode, thisSemester);
                Guid courseID = s.CourseID;
                studentRepository.CreateAttendsToRelationShipWithCourse(Guid.Parse(student.UserID), courseID);
            }

            //Student-subject :ENROLLED_TO {grade: x}
            foreach (var item in gradebookExports)
            {
                Guid subjectID = Guid.Parse(subjectRepository.FindSubjectBySubjectCode(item.SubjectCode).SubjectID);
                var grade = item.GetGrade();
                if (grade == null)
                    studentRepository.CreateEnrolledToRelationShipWithSubject(Guid.Parse(student.UserID), subjectID, item.Semester);
                else
                    studentRepository.CreateEnrolledToRelationShipWithSubjectAndGrade(Guid.Parse(student.UserID), subjectID, item.Semester, (int)grade);
            }

            Console.WriteLine();
        }

        private IEnumerable<Teacher> GetNonExistingTeachersFromExport(IEnumerable<CourseExportModel> courseExports)
        {
            var exportedTeachers = courseExports.Select(x => x.TeacherName.Split(",")).SelectMany(x => x).Distinct();
            var existingTeachers = teacherRepository.FindAll();

            var nonExistingTeachers = exportedTeachers.Except(existingTeachers.Select(x => x.Name).Distinct());
            return nonExistingTeachers.Select(x => new Teacher { Name = x });

        }

        private IEnumerable<Subject> GetNonExistingSubjectsFromExport(IEnumerable<GradeBookExportModel> gradebookExports)
        {
            var dbsubjects = subjectRepository.FindAll();

            var existingSubjectCodes = dbsubjects.Select(x => x.SubjectCode).Intersect(gradebookExports.Select(y => y.SubjectCode).Distinct()).ToList();
            //var toCreateSubjectCodes = gradebookExports.Where(x => !existingSubjectCodes.Contains(x.SubjectCode)).Select(y => y.SubjectCode).Distinct();

            var toCreateSubjects = gradebookExports.Where(x => !existingSubjectCodes.Contains(x.SubjectCode))
                .Select(y => new Subject
                {
                    SubjectCode = y.SubjectCode,
                    Credits = y.Credits,
                    Name = y.Name.Split(",").FirstOrDefault()
                }).GroupBy(p => p.SubjectCode).Select(g => g.First());

            return toCreateSubjects;

        }

        private async System.Threading.Tasks.Task<IEnumerable<CourseSubjectCode>> GetNonExistingCoursesFromExportAsync(IEnumerable<CourseExportModel> courseExports, string semester)
        {
            //var courseExports = CSVProcesser.ProcessCSV<CourseExportModel>(csvFilePath);
            var dbcourses = await courseRepository.GetAllCoursesWithTheirSubjectsInSemesterAsync("2017/18/1");

            var coursesNotInDB = courseExports.Where(
                    y => dbcourses.Where(
                        x => y.CourseCode == x.Course.CourseCode &&
                        y.SubjectCode == x.SubjectCode &&
                        x.Course.Semester == semester).Count() == 0
                                );

            var coursesToCreate = coursesNotInDB.Select(x => MapCourse.MapCourseExportToCourseSubjectCode(x, semester));

            return coursesToCreate;
        }

        private string StoreFile(IFormFile file, string path)
        {
            var folderName = Path.Combine("Resources", path);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return dbPath;
            }
            else
                throw new Exception("Form File was empty, cannot store it");
        }

    }
}

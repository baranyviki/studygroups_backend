using CryptoHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace StudyGroups.WebAPI.Services.Services
{

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICourseRepository courseRepository;
        private readonly IStudentRepository studentRepository;
        private readonly ISubjectRepository subjectRepository;
        private readonly ITeacherRepository teacherRepository;
        private readonly IUserRepository userRepository;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public AuthenticationService(ICourseRepository courseRepository, IStudentRepository studentRepository, ISubjectRepository subjectRepository
           , ITeacherRepository teacherRepository, IUserRepository userRepository, IConfiguration config, ILogger<AuthenticationService> logger)
        {
            this.courseRepository = courseRepository;
            this.studentRepository = studentRepository;
            this.subjectRepository = subjectRepository;
            this.teacherRepository = teacherRepository;
            this.userRepository = userRepository;
            _config = config;
            _logger = logger;
        }

        public string Login(LoginDTO user)
        {
            if (user == null || user.UserName == null || user.Password == null)
            {
                throw new ParameterException("Login object was null");
            }

            var loggedInUser = userRepository.FindUserByUserName(user.UserName);
            if (loggedInUser != null && loggedInUser.IsDisabled)
                throw new AuthenticationException("User is disabled, please contact administrator");
            if (loggedInUser != null && Crypto.VerifyHashedPassword(loggedInUser.Password, user.Password))
            {
                var roles = userRepository.GetUserLabelsByUserID(loggedInUser.UserID);
                roles.Remove("User");

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSecretKey"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loggedInUser.UserName)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, loggedInUser.UserID));
                claims.Add(new Claim("Role", roles[0]));

                var tokenOptions = new JwtSecurityToken(
                    issuer: _config["ValidClientURI"],
                    audience: _config["ValidClientURI"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(180),
                    signingCredentials: signinCredentials

                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return tokenString;
            }
            else
            {
                throw new AuthenticationException("Username or password is wrong");
            }
        }

        public void RegisterUser(StudentRegistrationDTO studentRegistrationDTO)
        {
            //todo: validate these values!!
            //check username existence

            var usernameExisting = studentRepository.FindStudentByUserName(studentRegistrationDTO.UserName);
            if (usernameExisting != null)
            {
                throw new RegistrationException("This username is already taken.");
            }

            if (studentRegistrationDTO.GradeBook != null && studentRegistrationDTO.Courses != null)
            {

                var student = new Student
                {
                    Email = studentRegistrationDTO.Email,
                    FirstName = studentRegistrationDTO.FirstName,
                    LastName = studentRegistrationDTO.LastName,
                    GenderType = (int)studentRegistrationDTO.Gender,
                    InstagramName = studentRegistrationDTO.InstagramName,
                    MessengerName = studentRegistrationDTO.MessengerName,
                    NeptunCode = studentRegistrationDTO.NeptunCode,
                    Password = Crypto.HashPassword(studentRegistrationDTO.Password),
                    UserName = studentRegistrationDTO.UserName
                };

                if (studentRegistrationDTO.Image != null)
                {
                    student.ImagePath = StoreFile(studentRegistrationDTO.Image, "Images");
                }

                studentRepository.CreateUserStudent(student);
                var stud = studentRepository.FindStudentByUserName(student.UserName);

                try
                {
                    ProcessNeptunExports(stud, studentRegistrationDTO.GradeBook, studentRegistrationDTO.Courses);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, e.Message);
                    studentRepository.Delete(stud, stud.UserID);
                    throw new RegistrationException("Neptun export processing was unsuccesful.");
                }
            }
            else
            {
                throw new RegistrationException("Neptun exports weren't included, or was in bad format. required file: .CSV - comma or semicolon separated, and UTF-8 coded");
            }

        }

        private void ProcessNeptunExports(Student student, IFormFile gradeBook, IFormFile courses)
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
            var createCourses = GetNonExistingCoursesFromExport(courseExports, thisSemester);

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
            var existingTeachers = teacherRepository.FindAll().Select(x => x.Name).ToList();

            var nonExistingTeachers = exportedTeachers.Where(x => !existingTeachers.Contains(x));

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

        private IEnumerable<CourseSubjectCode> GetNonExistingCoursesFromExport(IEnumerable<CourseExportModel> courseExports, string semester)
        {
            //var courseExports = CSVProcesser.ProcessCSV<CourseExportModel>(csvFilePath);
            var dbcourses = courseRepository.GetAllCoursesWithTheirSubjectsInSemester(semester).ToList();

            var courseList = courseExports.ToList();

            var coursesNotInDB = courseList.Where(
                    y => dbcourses.Where(x => y.CourseCode == x.Course.CourseCode && y.SubjectCode == x.SubjectCode && x.Course.Semester == semester).Count() == 0);


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
                throw new FileNotFoundException("Form File was empty, cannot store it");
        }


    }
}

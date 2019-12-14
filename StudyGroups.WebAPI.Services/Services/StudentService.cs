using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text.RegularExpressions;

namespace StudyGroups.WebAPI.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ISubjectRepository _subjectRepository;

        public StudentService(IStudentRepository studentRepository, ISubjectRepository subjectRepository)
        {
            _studentRepository = studentRepository;
            _subjectRepository = subjectRepository;
        }

        public IEnumerable<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester)
        {
            if (subjectID == null || semester == null)
                throw new ParameterException("Id parameters cannot be null");
            if (!Guid.TryParse(subjectID, out Guid subjectGuid))
                throw new ParameterException("Ids must be GUIDs");
            if (!Regex.Match(semester, "[1-2]{1}[0-9]{3}/[0-9]{2}/[1-2]{1}").Success)
                throw new ParameterException("Semester is in invalid format");
            var studentDBList = _studentRepository.GetStudentsAttendedToSubject(subjectID, semester);
            if (studentDBList == null)
                return new List<StudentListItemDTO>().AsEnumerable();
            var studentDTOs = studentDBList.Select(x => MapStudent.MapStudentDBModelToStudentListItemDTO(x));
            return studentDTOs;
        }

        public StudentDTO GetStudentDetails(string userID)
        {
            if (userID == null || !Guid.TryParse(userID, out Guid userguid))
                throw new ParameterException("user ID is invalid");
            var currentStudent = _studentRepository.FindStudentByUserID(userID);

            if (currentStudent == null)
                throw new AuthenticationException("Requested student does not exists");

            StudentDTO studentDTO = MapStudent.MapStudentDBModelToStudentDTO(currentStudent);

            var subjectsTutoring = _subjectRepository.GetSubjectsStudentIsTutoring(userID);

            if (subjectsTutoring != null)
            {
                var subjectDtos = subjectsTutoring.Select(x => MapSubject.MapSubjectToSubjectListItemDTO(x));
                studentDTO.TutoringSubjects = subjectDtos;
            }
            return studentDTO;
        }

        public IEnumerable<StudentListItemDTO> GetStudentFromStudyGroupSearch(StudyGroupSearchDTO searchParams, string loggedInUserId)
        {

            if (loggedInUserId == null || !Guid.TryParse(loggedInUserId, out Guid userguid))
                throw new ParameterException("user ID is invalid");
            if (searchParams == null || searchParams.CourseID == null)
                throw new ParameterException("Search parameters cannot be null");

            IEnumerable<Student> filteredStudents = new List<Student>();
            var currentSemester = SemesterManager.GetCurrentSemester();

            if (searchParams.IsHavingOtherCourseInCommonCurrently == true)
            {
                filteredStudents = _studentRepository.GetStudentsHavingCommonPracticalCoursesInCurrentSemester(loggedInUserId, searchParams.CourseID, currentSemester);
            }
            else
            {
                filteredStudents = _studentRepository.GetStudentsAttendingToCourseInCurrentSemester(loggedInUserId, searchParams.CourseID, currentSemester);
            }
            if (searchParams.IsSameCompletedSemesters && filteredStudents.Count() > 0)
            {
                //same completed semesters +-1            
                int userSemesterCnt = _studentRepository.GetStudentSemesterCount(loggedInUserId);
                var filteredOutStudents = new List<Student>();
                foreach (var stud in filteredStudents)
                {
                    int semesterCnt = _studentRepository.GetStudentSemesterCount(stud.UserID);
                    if (Math.Abs(userSemesterCnt - semesterCnt) > 1)
                    {
                        filteredOutStudents.Add(stud);
                    }
                }
                filteredStudents = filteredStudents.Where(x => !filteredOutStudents.Select(y => y.UserID).Contains(x.UserID));
            }
            if (searchParams.IsSameGradeAverage && filteredStudents.Count() > 0)
            {
                //same avg +-0.5
                double userGradeAvg = _studentRepository.GetStudentGradeAverage(loggedInUserId);
                var filteredOutStudents = new List<Student>();
                foreach (var stud in filteredStudents)
                {
                    double currStudAvg = _studentRepository.GetStudentGradeAverage(stud.UserID);
                    if (Math.Abs(currStudAvg - userGradeAvg) > 0.5)
                    {
                        filteredOutStudents.Add(stud);
                    }
                }
                filteredStudents = filteredStudents.Where(x => !filteredOutStudents.Select(y => y.UserID).Contains(x.UserID));
            }

            var filteredStudentListDtos = filteredStudents.Select(x => MapStudent.MapStudentDBModelToStudentListItemDTO(x));
            return filteredStudentListDtos;

        }

        public void UpdateStudentAndTutoringRelationShips(StudentDTO studentDTO, string userId)
        {
            if (studentDTO == null)
                throw new ParameterException("Student for update cannot be null.");
            if (userId == null || !Guid.TryParse(userId, out Guid subjectGuid))
                throw new ParameterException("UserID is null or invalid.");

            var student = MapStudent.MapStudentDTOToStudentDBModel(studentDTO, userId);

            _studentRepository.UpdateStudent(student);

            IEnumerable<string> toCreateTutoringRelationship = studentDTO.TutoringSubjects.Select(x => x.ID);

            var tutoringSubjects = _subjectRepository.GetSubjectsStudentIsTutoring(userId).ToList();
            if (tutoringSubjects != null && tutoringSubjects.Count != 0)
            {
                var subjectIdsFromDatabase = tutoringSubjects.Select(x => x.SubjectID);
                var subjectIdsFromDTO = studentDTO.TutoringSubjects.Select(x => x.ID);
                toCreateTutoringRelationship = subjectIdsFromDTO.Where(x => !subjectIdsFromDatabase.Contains(x));
                var toDeleteTutoringRelationship = subjectIdsFromDatabase.Where(x => !subjectIdsFromDTO.Contains(x));

                foreach (var subId in toDeleteTutoringRelationship)
                {
                    _studentRepository.DeleteTutoringRelationship(userId, subId);
                }
            }

            foreach (var subjectId in toCreateTutoringRelationship)
            {
                _studentRepository.MergeTutoringRelationship(userId, subjectId);
            }

        }

        public IEnumerable<StudentListItemDTO> GetStudentsTutoringSubject(string subjectid, string loggedInUserId)
        {
            if (subjectid == null || loggedInUserId == null)
                throw new ParameterException("Id parameters cannot be null");
            if (!Guid.TryParse(subjectid, out Guid subjectGuid) || !Guid.TryParse(loggedInUserId, out Guid userGuid))
                throw new ParameterException("Ids must be GUIDs");
            var students = _studentRepository.GetStudentsTutoringSubjectByID(subjectid);
            var studentListItemDtos = students.Select(x => MapStudent.MapStudentDBModelToStudentListItemDTO(x));
            studentListItemDtos = studentListItemDtos.Where(x => x.Id != loggedInUserId);
            return studentListItemDtos;
        }

        public IEnumerable<StudentListItemDTO> GetStudentFromStudyBuddySearch(StudyBuddySearchDTO searchParams, string loggedInUserId)
        {
            if (searchParams == null)
                throw new ParameterException("Search parameters cannot be null");
            if (searchParams.GoodInDiscipline < 0 || searchParams.GoodInDiscipline > Enum.GetValues(typeof(SubjectType)).Length)
                throw new ParameterException("Discipline value is invalid");
            if (searchParams.ComletedWithGrade < 0 || searchParams.ComletedWithGrade > 5)
                throw new ParameterException("Grade value is invalid");
            if (searchParams.SubjectID != "null" && !Guid.TryParse(searchParams.SubjectID, out Guid guid))
                throw new ParameterException("Subject ID is invalid");
            if (searchParams.IsAlreadyCompleted && searchParams.IsCurrentlyEnrolledTo)
                throw new ParameterException("Search parameters are invalid, already completed and currently enrolled to cant have true value at the same time.");

            List<Student> results = new List<Student>();

            if (!(searchParams.SubjectID == null && searchParams.GoodInDiscipline == 0))
            {
                if (searchParams.SubjectID != null)
                {
                    if (!searchParams.IsAlreadyCompleted && !searchParams.IsCurrentlyEnrolledTo && !searchParams.IsCommonCourse)
                        results = _studentRepository.GetStudentsEnrolledToSubject(searchParams.SubjectID).ToList();

                    string currentSemester = SemesterManager.GetCurrentSemester();

                    if (searchParams.IsCommonCourse)
                    {
                        results = _studentRepository.GetStudentsEnrolledToSubjectAndHavingCurrentlyCommonCourse(loggedInUserId, searchParams.SubjectID, currentSemester).ToList();
                    }

                    if (searchParams.IsCurrentlyEnrolledTo)
                    {
                        if (results.Count != 0)
                        {
                            results = GetStudentsEnrolledToSubjectCurrently(searchParams.IsAttendingToAnotherTeacher, loggedInUserId, currentSemester, searchParams.SubjectID, results);
                        }
                        else if (searchParams.IsAttendingToAnotherTeacher)
                            results = _studentRepository.GetStudentsCurrentlyEnrolledToSubjectWithStudentButHavingAnotherCurseTeacher(loggedInUserId, searchParams.SubjectID, currentSemester).ToList();
                        else
                            results = _studentRepository.GetStudentsEnrolledToSubjectInSemester(searchParams.SubjectID, currentSemester).ToList();
                    }
                    if (searchParams.IsAlreadyCompleted)
                    {
                        if (results.Count != 0)
                        {
                            results = GetStudentsAlreadyCompletedSubjectFromStudentList(searchParams.ComletedWithGrade, searchParams.SubjectID, results);
                        }
                        else if (searchParams.ComletedWithGrade != 0)
                            results = _studentRepository.GetStudentsCompletedSubjectWithGrade(searchParams.SubjectID, searchParams.ComletedWithGrade).ToList();
                        else
                            results = _studentRepository.GetStudentsCompletedSubject(searchParams.SubjectID).ToList();
                    }

                }
                if (searchParams.GoodInDiscipline != 0)
                {
                    double betterThanAvg = 3.5;
                    if (results.Count() != 0)
                        results = GetStudentsGoodInDisciplineFromStudentList(results, searchParams.GoodInDiscipline, betterThanAvg);
                    else
                        results = _studentRepository.GetStudentsGoodInDiscipline(searchParams.GoodInDiscipline, betterThanAvg).ToList();
                }


            }
            return results.Select(x => MapStudent.MapStudentDBModelToStudentListItemDTO(x));
        }

        public List<Student> GetStudentsGoodInDisciplineFromStudentList(List<Student> students, int discipline, double betterThanAvg)
        {
            var filteredStudents = new List<Student>();
            for (int i = 0; i < students.Count(); i++)
            {
                double avg = _studentRepository.GetStudentGradeAverageInDiscipline(students[i].UserID, discipline);
                if (avg >= betterThanAvg)
                    filteredStudents.Add(students[i]);
            }
            return filteredStudents;
        }

        /// <summary>
        /// Gets list and filter out the students who doesnt meet the criteria.
        /// </summary>
        /// <param name="havingAnotherTeacher">Filtering parameter.</param>
        /// <param name="students">Set of students to filter.</param>
        /// <returns></returns>
        public List<Student> GetStudentsEnrolledToSubjectCurrently(bool havingAnotherTeacher, string userId, string semester, string subjectId, List<Student> students)
        {
            List<Student> filteredStudents = new List<Student>();
            IEnumerable<Student> studentsCurrentlyEnrolledToSubject;
            if (havingAnotherTeacher)
                studentsCurrentlyEnrolledToSubject = _studentRepository.GetStudentsCurrentlyEnrolledToSubjectWithStudentButHavingAnotherCurseTeacher(userId, subjectId, semester);
            else
                studentsCurrentlyEnrolledToSubject = _studentRepository.GetStudentsEnrolledToSubjectInSemester(subjectId, semester);
            foreach (var stud in studentsCurrentlyEnrolledToSubject)
            {
                if (students.Select(x => x.UserID).Contains(stud.UserID))
                    filteredStudents.Add(stud);
            }
            return filteredStudents;
        }

        public List<Student> GetStudentsAlreadyCompletedSubjectFromStudentList(int grade, string subjectId, List<Student> students)
        {
            List<Student> filteredStudents = new List<Student>();
            var studIds = students.Select(x => x.UserID);
            IEnumerable<Student> queriedStudents;
            if (grade == 0)
                queriedStudents = _studentRepository.GetStudentsCompletedSubject(subjectId);
            else
                queriedStudents = _studentRepository.GetStudentsCompletedSubjectWithGrade(subjectId, grade);
            foreach (var stud in queriedStudents)
            {
                if (studIds.Contains(stud.UserID))
                    filteredStudents.Add(stud);
            }
            return filteredStudents;
        }

    }
}

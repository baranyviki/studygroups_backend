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

namespace StudyGroups.Services
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

            IEnumerable<string> toCreateTutoringRelationship = studentDTO.TutoringSubjects.Select(x => x.SubjectID);

            var tutoringSubjects = _subjectRepository.GetSubjectsStudentIsTutoring(userId).ToList();
            if (tutoringSubjects != null && tutoringSubjects.Count != 0)
            {
                var subjectIdsFromDatabase = tutoringSubjects.Select(x => x.SubjectID);
                var subjectIdsFromDTO = studentDTO.TutoringSubjects.Select(x => x.SubjectID);
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
    }
}

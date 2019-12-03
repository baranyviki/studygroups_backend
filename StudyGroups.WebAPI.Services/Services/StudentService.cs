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

namespace StudyGroups.Services
{
    public class StudentService : IStudentService
    {
        IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            this._studentRepository = studentRepository;
        }

        public List<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester)
        {
            var studentDBList = _studentRepository.GetStudentsAttendedToSubject(subjectID, semester);
            List<StudentListItemDTO> studentDTOs = new List<StudentListItemDTO>();
            studentDTOs.AddRange(studentDBList.Select(x => MapStudents.MapStudentDBModelToStudentListItemDTO(x)));
            return studentDTOs;
        }

        public List<StudentListItemDTO> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade)
        {
            throw new NotImplementedException();
        }

        public StudentDTO GetStudentDetails(string userID)
        {
            var currentStudent = _studentRepository.FindStudentByUserID(userID);
            if (currentStudent == null)
                throw new AuthenticationException("Requested student does not exists");
            StudentDTO studentDTO = MapStudents.MapStudentDBModelToStudentDTO(currentStudent);
            return studentDTO;
        }

        public List<StudentListItemDTO> GetStudentFromStudyGroupSearch(StudyGroupSearchDTO searchParams, string loggedInUserId)
        {
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
                filteredStudents = filteredStudents.Where(x => !filteredOutStudents.Select(y=>y.UserID).Contains(x.UserID));
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

            var filteredStudentListDtos = filteredStudents.Select(x => MapStudents.MapStudentDBModelToStudentListItemDTO(x)).ToList();
            return filteredStudentListDtos;

        }



    }
}

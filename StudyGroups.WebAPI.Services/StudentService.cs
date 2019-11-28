using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.Services
{
    public class StudentService : IStudentService
    {
        IStudentRepository studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public List<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester)
        {
            var studentDBList = studentRepository.GetStudentsAttendedToSubject(subjectID, semester);
            List<StudentListItemDTO> studentDTOs = new List<StudentListItemDTO>();
            studentDTOs.AddRange(studentDBList.Select(x => MapStudents.MapStudentDBModelToStudentDTO(x)));
            return studentDTOs;
        }

        public List<StudentListItemDTO> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade)
        {
            throw new NotImplementedException();
        }
    }
}

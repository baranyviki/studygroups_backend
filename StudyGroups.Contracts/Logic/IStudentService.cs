using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Contracts.Logic
{
    public interface IStudentService
    {
        List<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester);
        List<StudentListItemDTO> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade);
        StudentDTO GetStudentDetails(string username);
        List<StudentListItemDTO> GetStudentFromStudyGroupSearch(StudyGroupSearchDTO searchParams,string userId);
    }
}

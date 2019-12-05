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
        StudentDTO GetStudentDetails(string username);
        List<StudentListItemDTO> GetStudentFromStudyGroupSearch(StudyGroupSearchDTO searchParams,string userId);
        void UpdateStudentAndTutoringRelationShips(StudentDTO studentDTO,string userId);
    }
}

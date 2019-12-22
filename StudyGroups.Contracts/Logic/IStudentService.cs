using StudyGroups.DTOmodels;
using StudyGroups.WebAPI.Models;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Logic
{
    public interface IStudentService
    {
        IEnumerable<StudentListItemDTO> GetStudentsAttendedToSubject(string subjectID, string semester);
        StudentDTO GetStudentDetails(string username);
        IEnumerable<StudentListItemDTO> GetStudentFromStudyGroupSearch(StudyGroupSearchDTO searchParams, string userId);
        void UpdateStudentAndTutoringRelationShips(StudentDTO studentDTO, string userId);
        IEnumerable<StudentListItemDTO> GetStudentsTutoringSubject(string id, string loggedInUserId);
        IEnumerable<StudentListItemDTO> GetStudentFromStudyBuddySearch(StudyBuddySearchDTO searchParams, string loggedInUserId);

        double[] GetSemesterAverages();
    }
}

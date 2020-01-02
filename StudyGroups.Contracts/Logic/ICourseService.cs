using StudyGroups.WebAPI.Models;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Logic
{
    public interface ICourseService
    {
        IEnumerable<GeneralSelectionItem> GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(string userID);
    }
}

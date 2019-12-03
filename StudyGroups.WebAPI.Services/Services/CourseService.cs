using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudyGroups.WebAPI.Services.Services
{
    public class CourseService : ICourseService
    {
        ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public IEnumerable<GeneralSelectionItem> GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(string userID)
        {
            string currentSemester = SemesterManager.GetCurrentSemester();
            var subjects = _courseRepository.FindLabourCoursesWithSubjectStudentCurrentlyEnrolledTo(userID, currentSemester);
            var subjectSelectionItems = subjects.Select(x => MapCourse.MapCourseProjectionToGeneralSelectionItem(x));
            return subjectSelectionItems;
        }
    }
}

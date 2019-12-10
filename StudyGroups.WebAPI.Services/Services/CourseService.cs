using StudyGroups.Contracts.Logic;
using StudyGroups.Contracts.Repository;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Mapping;
using StudyGroups.WebAPI.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public IEnumerable<GeneralSelectionItem> GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(string userID)
        {
            if (userID == null || !Guid.TryParse(userID, out Guid userGUID))
            {
                throw new ParameterException("UserID is invalid");
            }
            string currentSemester = SemesterManager.GetCurrentSemester();
            var subjects = _courseRepository.FindLabourCoursesWithSubjectStudentCurrentlyEnrolledTo(userID, currentSemester);
            var subjectSelectionItems = subjects.Select(x => MapCourse.MapCourseProjectionToGeneralSelectionItem(x));
            return subjectSelectionItems;
        }
    }
}

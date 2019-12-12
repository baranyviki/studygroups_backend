using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyGroups.Contracts.Repository
{
    public interface ICourseRepository : IBaseRepository<Course>
    {
        IEnumerable<CourseSubjectCode> GetAllCoursesWithTheirSubjectsInSemester(string semester);

        /// <summary>
        /// Creates a :BELONGS_TO relationship between the id given course and subject.  
        /// </summary>
        /// <param name="courseID">Neptun's unique course identifier</param>
        /// <param name="subjectID">Neptun's unique subject identifier</param>
        void CreateBelongsToRelationshipToSubject(Guid courseID, Guid subjectID);

        /// <summary>
        /// Gets a course in a semester, that belongs to the given subject and has the given courseCode 
        /// </summary>
        /// <param name="courseCode">Neptun's unique course identifier</param>
        /// <param name="subjectCode">Neptun's unique subject identifier</param>
        /// <param name="semester">Semester, in which course was started. Format: YYYY/YY/S</param>
        /// <returns></returns>
        CourseIDSubjectIDProjection GetCourseWithSubject(string courseCode, string subjectCode, string semester);

        Course CreateCourseBelongingToSubject(Course course, string subjectCode);

        IEnumerable<CourseCodeSubjectNameProjection> FindLabourCoursesWithSubjectStudentCurrentlyEnrolledTo(string username, string currentSemester);

    }
}

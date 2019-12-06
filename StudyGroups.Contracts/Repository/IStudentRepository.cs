using StudyGroups.Data.DAL.DAOs;
using System;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Repository
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        /// <summary>
        /// Gets students having a specific subject in a specific semester. 
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        IEnumerable<Student> GetStudentsAttendedToSubject(string subjectID, string semester);

        /// <summary>
        /// Gets students who had been comleted a specific subject, with a better grade then specified.
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="semester"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        IEnumerable<Student> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade);

        void CreateAttendsToRelationShipWithCourse(Guid userID, Guid courseID);

        Student CreateUserStudent(Student student);

        Student FindStudentByUserID(string userID);

        Student FindStudentByUserName(string userName);

        void UpdateStudent(Student student);

        void MergeTutoringRelationship(string userId, string subjectId);

        void DeleteTutoringRelationship(string userId, string subjectId);
        
        void CreateEnrolledToRelationShipWithSubjectAndGrade(Guid userID, Guid subjectID, string semester, int grade);

        void CreateEnrolledToRelationShipWithSubject(Guid userID, Guid subjectID, string semester);

        IEnumerable<Student> GetStudentsHavingCommonPracticalCoursesInCurrentSemester(string userId, string searchCourseId, string currentSemester);

        IEnumerable<Student> GetStudentsAttendingToCourseInCurrentSemester(string userId, string searchCourseId, string currentSemester);

        double GetStudentGradeAverage(string userId);

        int GetStudentSemesterCount(string userId);

        IEnumerable<Student> GetStudentsTutoringSubjectByID(string subjectId);
    }
}

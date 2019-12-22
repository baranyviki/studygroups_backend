using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
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


        /// <summary>
        /// Gets given student's grade average in given discipline
        /// </summary>
        /// <param name="userID">user unique identifier</param>
        /// <param name="goodInDiscipline">which discipline</param>
        /// <returns></returns>
        double GetStudentGradeAverageInDiscipline(string userID, int goodInDiscipline);
        IEnumerable<Student> GetStudentsGoodInDiscipline(int goodInDiscipline, double betterThanAvg);
        IEnumerable<Student> GetStudentsEnrolledToSubject(string subjectID);
        IEnumerable<Student> GetStudentsEnrolledToSubjectAndHavingCurrentlyCommonCourse(string userID, string subjectId, string currentSemester);
        IEnumerable<Student> GetStudentsCurrentlyEnrolledToSubjectWithStudentButHavingAnotherCurseTeacher(string userId, string subjectId, string semester);
        IEnumerable<Student> GetStudentsEnrolledToSubjectInSemester(string subjectId, string semester);
        IEnumerable<Student> GetStudentsCompletedSubject(string subjectID);
        IEnumerable<Student> GetStudentsCompletedSubjectWithGrade(string subjectID, int comletedWithGrade);

        IEnumerable<SemesterAverageGrouping> GetSemesterAverageGroupings();
    }
}

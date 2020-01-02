using StudyGroups.Data.DAL.DAOs;
using System;

namespace StudyGroups.Contracts.Repository
{
    public interface ITeacherRepository : IBaseRepository<Teacher>
    {
        void CreateTeachesRelationshipWithCourseIDs(Guid teacherID, Guid courseID);
        void CreateTeachesRelationshipWithCourseParams(string subjectCode, string courseCode, string semester, string teacherName);
    }
}

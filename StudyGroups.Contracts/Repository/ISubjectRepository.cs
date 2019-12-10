using StudyGroups.Data.DAL.DAOs;
using System.Collections.Generic;

namespace StudyGroups.Contracts.Repository
{
    public interface ISubjectRepository : IBaseRepository<Subject>
    {
        Subject FindSubjectBySubjectCode(string subjectCode);
        IEnumerable<Subject> GetSubjectsStudentHasPassed(string userID);
        IEnumerable<Subject> GetSubjectsStudentIsTutoring(string userId);
        Subject GetSubjectById(string subjectId);
        Subject UpdateSubject(Subject subject);
    }
}
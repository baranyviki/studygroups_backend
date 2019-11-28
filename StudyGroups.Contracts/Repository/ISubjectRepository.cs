using StudyGroups.Data.DAL.DAOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudyGroups.Contracts.Repository
{
    public interface ISubjectRepository : IBaseRepository<Subject>
    {
        Subject FindSubjectBySubjectCode(string subjectCode);

    }
}

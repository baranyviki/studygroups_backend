using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.DataAccessLayer.DAOs;
using StudyGroups.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace StudyGroups.Data.Repository
{
    public class SubjectRepository :  BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

    }
}

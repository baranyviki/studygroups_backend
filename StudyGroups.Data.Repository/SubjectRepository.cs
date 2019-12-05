using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace StudyGroups.Data.Repository
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

        public Subject FindSubjectBySubjectCode(string subjectCode)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("subjectCode", subjectCode);
                string query = $@"MATCH (node:Subject) WHERE node.SubjectCode = $subjectCode RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<Subject>();
            }

        }

        public IEnumerable<Subject> GetSubjectsStudentHasPassed(string userID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID);
                string query = $@"MATCH(n: User) -[r: ENROLLED_TO]->(s: Subject)
                        WHERE n.UserID = $userId AND r.Grade > 1 RETURN s";
                var result = session.Run(query, parameters);
                return result.Map<Subject>();
            }
        }
        
        public IEnumerable<Subject> GetSubjectsStudentIsTutoring(string userId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId);
                string query = $@"MATCH (n:User)-[:TUTORING]->(s:Subject)
                                  WHERE n.UserID=$userId RETURN s";
                var result = session.Run(query, parameters);
                return result.Map<Subject>();
            }
        }
    }
}

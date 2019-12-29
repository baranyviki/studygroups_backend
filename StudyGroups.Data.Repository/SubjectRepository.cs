using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

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
                return result.SingleOrDefault().Map<Subject>();
            }

        }

        public Subject GetSubjectById(string subjectId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("subjectId", subjectId);
                string query = $@"MATCH (node:Subject) WHERE node.SubjectID = $subjectId RETURN node";
                var result = session.Run(query, parameters);
                var res = result.ToList();
                if (res.Count == 0)
                    throw new NodeNotExistsException("Subject with given id does not exists");
                return res.SingleOrDefault().Map<Subject>();
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

        public Subject UpdateSubject(Subject subject)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("subjectId", subject.SubjectID).WithEntity("props", subject);
                string query = $@"MATCH (n:Subject)
                                  WHERE n.SubjectID=$subjectId SET n += $props
                                  RETURN n";

                var result = session.Run(query, parameters);
                var summary = result.Summary;
                if (summary.Notifications.Select(x => x.Description).Contains("Error"))
                {
                    throw new NodeNotExistsException("Node to be updated does not exists.");
                }
                return result.SingleOrDefault().Map<Subject>();
            }
        }


    }
}

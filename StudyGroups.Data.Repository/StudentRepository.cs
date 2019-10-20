using Neo4j.Driver.V1;
using StudyGroups.DataAccessLayer.DAOs;
using StudyGroups.Contracts.Repository;
using System.Collections.Generic;
using System.Linq;
using Neo4jMapper;
using System;

namespace StudyGroups.Repository
{
    public class StudentRepository : BaseRepository<StudentDBModel>, IStudentRepository
    {
        public StudentRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

        public IEnumerable<StudentDBModel> GetStudentsAttendedToSubject(int subjectID, string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query = String.Format(
                          @"MATCH (stud:Student)-[r:ENROLLED_TO]->(sub:Subject)
                            WHERE sub.SubjectID={0} AND r.Semester='{1}'
                            RETURN stud"
                        , subjectID
                        , semester
                    );
                var result = session.Run(query);
                var students = result.ToList().Map<StudentDBModel>();
                return students;
            }
        }

        public IEnumerable<StudentDBModel> GetStudentsAttendedToSubjectWithGrade(int subjectID, string semester, int grade)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query = String.Format(
                          @"MATCH (stud:Student)-[r:ENROLLED_TO]->(sub:Subject)
                            WHERE sub.SubjectID={0} AND r.Semester='{1}' AND r.Grade > {2}
                            RETURN stud"
                        , subjectID
                        , semester
                        , grade
                    );
                var result = session.Run(query);
                var students = result.ToList().Map<StudentDBModel>();
                return students;
            }
        }
    }
}

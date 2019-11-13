using Neo4j.Driver.V1;
using StudyGroups.Contracts.Repository;
using System.Collections.Generic;
using System.Linq;
using Neo4jMapper;
using System;
using StudyGroups.DataAccessLayer.DAOs;

namespace StudyGroups.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

        public IEnumerable<Student> GetStudentsAttendedToSubject(int subjectID, string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query = $@"MATCH (stud:Student)-[r:ENROLLED_TO]->(sub:Subject)
                            WHERE sub.SubjectID={subjectID} AND r.Semester='{semester}'
                            RETURN stud";
                var result = session.Run(query);
                var students = result.ToList().Map<Student>();
                return students;
            }
        }

        public IEnumerable<Student> GetStudentsAttendedToSubjectWithGrade(int subjectID, string semester, int grade)
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
                var students = result.ToList().Map<Student>();
                return students;
            }
        }


    }
}

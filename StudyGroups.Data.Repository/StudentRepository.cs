using Neo4j.Driver.V1;
using StudyGroups.Contracts.Repository;
using System.Collections.Generic;
using System.Linq;
using Neo4jMapper;
using System;
using StudyGroups.Data.DAL.DAOs;

namespace StudyGroups.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

        public void CreateAttendsToRelationShipWithCourse(Guid userID, Guid courseID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID.ToString())
                                                      .WithValue("courseId", courseID.ToString());
                string query = @"MATCH (stud :Student { UserID: $userId}) 
                                 MATCH (course :Course { CourseID: $courseId })
                                 MERGE (stud)-[:ATTENDS]->(course)";
                var result = session.Run(query, parameters);
            }
        }

        public void CreateEnrolledToRelationShipWithSubjectAndGrade(Guid userID, Guid subjectID, string semester, int grade)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID.ToString())
                                                      .WithValue("subjectId", subjectID.ToString())
                                                      .WithValue("semester", semester)
                                              .WithValue("grade", grade);

                string query = @"MATCH (stud:Student { UserID: $userId})
                                 MATCH (subj: Subject { SubjectID: $subjectId})
                                 MERGE (stud)-[r: ENROLLED_TO {Semester: $semester}]->(subj)
                                 ON CREATE set r.Grade= $grade
                                 ON MATCH set r.Grade= $grade";
                var result = session.Run(query, parameters);
            }
        }
        public void CreateEnrolledToRelationShipWithSubject(Guid userID, Guid subjectID, string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID.ToString())
                                                      .WithValue("subjectId", subjectID.ToString())
                                                      .WithValue("semester", semester);
                
                string query = @"MATCH (stud:Student { UserID: $userId})
                                 MATCH (subj: Subject { SubjectID: $subjectId})
                                 MERGE (stud)-[r: ENROLLED_TO {Semester: $semester}]->(subj)";
                var result = session.Run(query, parameters);
            }
        }

        public Student CreateUserStudent(Student student)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithEntity("newNode", student);
                string query = $@"CREATE (node :User:Student  $newNode ) RETURN node";
                var result = session.Run(query, parameters);
                var stud = result.Single().Map<Student>();
                return stud;
            }
        }

        public IEnumerable<Student> GetStudentsAttendedToSubject(string subjectID, string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query = $@"MATCH (stud:Student)-[r:ENROLLED_TO]->(sub:Subject)
                            WHERE sub.SubjectID='{subjectID}' AND r.Semester='{semester}'
                            RETURN stud";
                var result = session.Run(query);
                var students = result.Map<Student>();
                return students;
            }
        }

        public IEnumerable<Student> GetStudentsAttendedToSubjectWithGrade(string subjectID, string semester, int grade)
        {
            using (var session = Neo4jDriver.Session())
            {
                string query = String.Format(
                          @"MATCH (stud:Student)-[r:ENROLLED_TO]->(sub:Subject)
                            WHERE sub.SubjectID='{0}' AND r.Semester='{1}' AND r.Grade > {2}
                            RETURN stud"
                        , subjectID
                        , semester
                        , grade
                    );
                var result = session.Run(query);
                var students = result.Map<Student>();
                return students;
            }
        }

        public Student FindStudentByUserName(string userName)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("username", userName);
                string query = $@"MATCH (node:Student) WHERE node.UserName = $username RETURN node";
                var result = session.Run(query, parameters);
                return result.Single().Map<Student>();
            }
        }


    }
}

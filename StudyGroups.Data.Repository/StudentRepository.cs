using Neo4j.Driver.V1;
using StudyGroups.Contracts.Repository;
using System.Collections.Generic;
using System.Linq;
using Neo4jMapper;
using System;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;

namespace StudyGroups.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(IDriver neo4jDriver) : base(neo4jDriver)
        { }

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

        public void UpdateStudent(Student student)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", student.UserID)
                                                      .WithValue("firstName",student.FirstName)
                                                      .WithValue("lastName",student.LastName)
                                                      .WithValue("email",student.Email)
                                                      .WithValue("messenger",student.MessengerName)
                                                      .WithValue("instagram",student.InstagramName)
                                                      .WithValue("neptun",student.NeptunCode);

                string query = @"MATCH(n: User)
                                 WHERE n.UserID = $userId
                                 SET n.FirstName = $firstName,n.LastName = $lastName, n.Email =$email,
                                 n.MessengerName = $messenger, n.InstagramName = $instagram, n.NeptunCode=$neptun";
                var result = session.Run(query, parameters);
            }
        }

        public void MergeTutoringRelationship(string userId, string subjectId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId).WithValue("subjectId",subjectId);
                string query = $@"MATCH (u:User),(s:Subject)
                                  WHERE u.UserID = $userId and s.SubjectID=$subjectId
                                  MERGE(u) -[r: TUTORING]->(s)";
                var result = session.Run(query, parameters);
            }
        }

        public void DeleteTutoringRelationship(string userId, string subjectId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId).WithValue("subjectId", subjectId);
                string query = $@"MATCH (u:User)-[r:TUTORING]->(s:Subject)
                                  WHERE u.UserID=$userId AND s.SubjectID=$subjectId DELETE r";
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

        public Student FindStudentByUserName(string userName)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("username", userName);
                string query = $@"MATCH (node:Student) WHERE node.UserName = $username RETURN node";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return null;
                return resultList.Single().Map<Student>();
            }
        }

        public Student FindStudentByUserID(string userID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userID);
                string query = $@"MATCH (node:Student) WHERE node.UserID = $userId RETURN node";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return null;
                return resultList.Single().Map<Student>();
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
                
        public IEnumerable<Student> GetStudentsHavingCommonPracticalCoursesInCurrentSemester(string userId, string searchCourseId, string currentSemester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId)
                                                      .WithValue("courseId", searchCourseId)
                                                      .WithValue("semester", currentSemester);
                string query = $@"MATCH (u1:Student)-[:ATTENDS]->(c1:Course)<-[:ATTENDS]-(u2:Student)
                                  ,(u1)-[:ATTENDS]->(c2:Course)<-[:ATTENDS]-(u2)
                                  WHERE u1.UserID = $userId AND c1.Semester = $semester AND c1.CourseID=$courseId
                                  AND c2.Semester=c1.Semester AND c1 <> c2  AND c2.CourseType IN([1,2])
                                  RETURN distinct u2";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return new List<Student>();
                return resultList.Map<Student>();
            }
        }

        public IEnumerable<Student> GetStudentsAttendingToCourseInCurrentSemester(string userId, string searchCourseId, string currentSemester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId)
                                                      .WithValue("courseId", searchCourseId)
                                                      .WithValue("semester", currentSemester);
                string query = $@"MATCH (u1:Student)-[:ATTENDS]->(c1:Course)<-[:ATTENDS]-(u2:Student)
                                 WHERE u1.UserID = $userId AND c1.Semester = $semester 
                                 AND c1.CourseID = $courseId 
                                 RETURN u2";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return new List<Student>();
                return resultList.Map<Student>();
            }

        }

        public double GetStudentGradeAverage(string userId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId);
                string query = $@"MATCH (u1:Student)-[r:ENROLLED_TO]->(:Subject)
                                  WHERE u1.UserID=$userId
                                  AND r.Grade is not null
                                  RETURN avg(distinct r.Grade) as avg";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return 0.0;
                return resultList.Single().Map<double>();
            }
        }

        public int GetStudentSemesterCount(string userId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userId);
                string query = $@"MATCH (u1:Student)-[r:ENROLLED_TO]->(:Subject)
                                WHERE u1.UserID = $userId
                                RETURN count(distinct r.Semester) as cnt";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return 0;
                return resultList.Single().Map<int>();
            }
        }

        public IEnumerable<Student> GetStudentsTutoringSubjectByID(string subjectId)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("subjectId", subjectId);
                string query = $@"MATCH (u:User)-[r:TUTORING]->(s:Subject)
                                  WHERE s.SubjectID=$subjectId return u";
                var result = session.Run(query, parameters);
                var resultList = result.ToList();
                if (resultList.Count == 0)
                    return new List<Student>();
                return resultList.Map<Student>();
            }
        }
    }
}

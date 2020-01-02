using Neo4j.Driver.V1;
using Neo4jMapper;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Repository;
using System;

namespace StudyGroups.Data.Repository
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(IDriver _neo4jDriver) : base(_neo4jDriver)
        {

        }

        public void CreateTeachesRelationshipWithCourseIDs(Guid teacherID, Guid courseID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("teacherId", teacherID.ToString())
                                                      .WithValue("courseId", courseID.ToString());
                string query = @"MATCH (teacher :Teacher { TeacherID: $teacherId}) 
                                 MATCH (course  :Course  { CourseID: $courseId })
                                 MERGE (teacher)-[:TEACHES]->(course)";
                var result = session.Run(query, parameters);
            }
        }

        public void CreateTeachesRelationshipWithCourseParams(string subjectCode, string courseCode, string semester, string teacherName)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("teacherName", teacherName)
                                                      .WithValue("subjectCode", subjectCode)
                                                      .WithValue("courseCode", courseCode)
                                                      .WithValue("semester", semester);

                string query = @"MATCH (sub:Subject {SubjectCode:$subjectCode })<-[BELONGS_TO]-
                                (course: Course { CourseCode: $courseCode, Semester:$semester})
                                MATCH (teacher: Teacher { Name:$teacherName})
                                MERGE (teacher)-[r: TEACHES]->(course)";
                var result = session.Run(query, parameters);
            }
        }
    }
}

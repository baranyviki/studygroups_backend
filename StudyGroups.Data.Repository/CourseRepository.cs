using Neo4j.Driver.V1;
using Neo4jMapper;
using ServiceStack.Text;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.ConversionUtils;
using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyGroups.Data.Repository
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(IDriver neo4jDriver) : base(neo4jDriver)
        {

        }

        public void CreateBelongsToRelationshipToSubject(Guid courseID, Guid subjectID)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("courseId", courseID.ToString())
                                                      .WithValue("subjectId", subjectID.ToString());
                string query = @"MATCH (course :Course { CourseID: $courseId }) 
                                 MATCH (subject :Subject { SubjectID: $subjectId })
                                 MERGE (course)-[:BELONGS_TO]->(subject)";

                var result = session.Run(query, parameters);

            }
        }

        //creates 
        public Course CreateCourseBelongingToSubject(Course course, string subjectCode)
        {
            var parameters = new Neo4jParameters().WithValue("subjectCode", subjectCode);
            string courseLiteralMap = course.GetCypherFormattedNodeParameters();
            using (var session = Neo4jDriver.Session())
            {
                string query = $@"MATCH (sub:Subject {{SubjectCode: $subjectCode }})
                                 MERGE (sub)<-[r: BELONGS_TO]-(c: Course {{" + courseLiteralMap + @"})
                                 RETURN c";
                var result = session.Run(query, parameters);
                return result.Single().Map<Course>();
            }
        }

        public async Task<IEnumerable<CourseSubjectCode>> GetAllCoursesWithTheirSubjectsInSemesterAsync(string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("semester", semester);

                var cursor = await session.RunAsync(@"
                            MATCH (c:Course { Semester: $semester })-[:BELONGS_TO]-(s:Subject)
                            RETURN s,c", parameters);

                var courseWithSubjects = (await cursor.ToListAsync())
                  .Map((Course course, Subject subject) => new CourseSubjectCode
                  {
                      Course = course,
                      SubjectCode = subject.SubjectCode
                  });
                return courseWithSubjects;
            }
        }

        public CourseIDSubjectIDProjection GetCourseWithSubject(string courseCode, string subjectCode, string semester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("courseCode", courseCode)
                                                      .WithValue("subjectCode", subjectCode)
                                                      .WithValue("semester", semester);
                string query = @" MATCH (course:Course)-[r:BELONGS_TO]->(sub:Subject)
                                  WHERE course.Semester=$semester and course.CourseCode=$courseCode and sub.SubjectCode=$subjectCode
                                  RETURN course, sub ";

                var result = session.Run(query, parameters);
                var s = result.Single().Map((Course course, Subject subject) => new CourseIDSubjectIDProjection
                {
                    CourseID = Guid.Parse(course.CourseID),
                    SubjectID = Guid.Parse(subject.SubjectID)
                });
                return s;
            }

        }

        public IEnumerable<CourseCodeSubjectNameProjection> FindLabourCoursesWithSubjectStudentCurrentlyEnrolledTo(string userid, string currentSemester)
        {
            using (var session = Neo4jDriver.Session())
            {
                var parameters = new Neo4jParameters().WithValue("userId", userid)
                                                      .WithValue("semester", currentSemester);
                string query = $@"MATCH (u:User)-[r:ATTENDS]->(c:Course)
                                  MATCH (c)-[:BELONGS_TO]->(s:Subject)
                                  WHERE u.UserID = $userId AND c.Semester = $semester AND (c.CourseType=1 OR c.CourseType=2) 
                                  RETURN c.CourseID as CourseID, c.CourseCode +' - '+ s.Name+' ('+s.SubjectCode+')' as CourseCodeWithSubjectName";
                var result = session.Run(query, parameters);
                return result.Map((string CourseID, string CourseCodeWithSubjectName) =>
                new CourseCodeSubjectNameProjection
                {
                    CourseID = CourseID,
                    CourseCodeWithSubjectName = CourseCodeWithSubjectName
                }).ToList();
            }
        }
    }
}
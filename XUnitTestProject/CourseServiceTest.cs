using Moq;
using StudyGroups.Contracts.Repository;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Services.Exceptions;
using StudyGroups.WebAPI.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestProject
{
    public class CourseServiceTest
    {
        private readonly CourseService _courseService;
        private readonly Mock<ICourseRepository> _courseRepository;

        public CourseServiceTest()
        {
            _courseRepository = new Mock<ICourseRepository>();
            _courseService = new CourseService(_courseRepository.Object);
        }

        [Fact]
        public void GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester_CalledWithNull_Throws()
        {
            string userid = null;
            Assert.Throws<ParameterException>(() => _courseService.GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(userid));
        }


        [Fact]
        public void GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester_CalledWithInvalidID_Throws()
        {
            string userid = "some string value";

            Assert.Throws<ParameterException>(() => _courseService.GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(userid));
        }

        [Fact]
        public void GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester_CalledWithValidUserId_ReturnsExactItems()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            _courseRepository.Setup(x => x.FindLabourCoursesWithSubjectStudentCurrentlyEnrolledTo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<CourseCodeSubjectNameProjection> {
                new CourseCodeSubjectNameProjection{ CourseID = guid1 , CourseCodeWithSubjectName="LA_2 - Databases"},
                new CourseCodeSubjectNameProjection{ CourseID = guid2 , CourseCodeWithSubjectName="EA_0 - Programming"},
                });

            var res = _courseService.GetAllLabourCoursesWithSubjectStudentEnrolledToCurrentSemester(guid1);

            Assert.Equal(2, res.Count());
            Assert.Equal(guid1, res.First().ID);
            Assert.Equal(guid2, res.ElementAt(1).ID);
        }
    }
}

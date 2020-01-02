using StudyGroups.Data.DAL.DAOs;
using StudyGroups.Data.DAL.ProjectionModels;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Mapping;
using System;
using Xunit;

namespace XUnitTestProject
{
    public class MapExtensionTest
    {
        [Fact]
        public void MapCourseExportToCourseSubjectCode_WhenPassedEmpty_ReturnsEmptyCourseSubjectCode()
        {
            var export = new CourseExportModel();

            var mapped = MapCourse.MapCourseExportToCourseSubjectCode(export, null);

            Assert.NotNull(mapped);
            Assert.Null(mapped.Course.Semester);
        }


        [Fact]
        public void MapCourseExportToCourseSubjectCode_WhenPassedWithAllValues_ReturnsCourseSubjectCodeWithAllValues()
        {
            var export = new CourseExportModel
            {
                ClassHours = "a",
                ClassSchedule = "a",
                CourseCode = "a",
                CourseType = "Elmélet",
                SubjectCode = "a",
                SubjectName = "a",
                TeacherName = "a",
                WaitingList = "a"
            };

            var mapped = MapCourse.MapCourseExportToCourseSubjectCode(export, "semester");

            Assert.Equal("a", mapped.SubjectCode);
            Assert.Equal("a", mapped.Course.CourseCode);
            Assert.Null(mapped.Course.CourseID);
            Assert.Equal(0, mapped.Course.CourseType);
            Assert.Equal("semester", mapped.Course.Semester);
        }


        [Fact]
        public void MapCourseExportToCourseSubjectCode_WhenPassedWithAllValues_ReturnsCourseSubjectCodeType()
        {
            var export = new CourseExportModel();

            var mapped = MapCourse.MapCourseExportToCourseSubjectCode(export, "a");

            Assert.IsType<CourseSubjectCode>(mapped);
        }

        [Fact]
        public void MapCourseProjectionToGeneralSelectionItem_WhenPassedValidProjectionModel_ReturnsValidSelectionItem()
        {
            string name = "Name";
            string guid = Guid.NewGuid().ToString();

            var projection = new CourseCodeSubjectNameProjection { CourseCodeWithSubjectName = name, CourseID = guid };
            var mapped = MapCourse.MapCourseProjectionToGeneralSelectionItem(projection);
            Assert.Equal(name, mapped.DisplayName);
            Assert.Equal(guid, mapped.ID);
        }


        [Fact]
        public void MapCourseProjectionToGeneralSelectionItem_WhenPassedValidProjectionModelID_ReturnsValidSelectionItemID()
        {
            var projection = new CourseCodeSubjectNameProjection { CourseCodeWithSubjectName = "Name", CourseID = Guid.NewGuid().ToString() };
            var mapped = MapCourse.MapCourseProjectionToGeneralSelectionItem(projection);

            Assert.True(Guid.TryParse(mapped.ID, out Guid result));

        }
        [Fact]
        public void MapStudentDBModelToStudentListItemDTO_StudentPassed_ReturnsCorrectListItem()
        {
            string id = Guid.NewGuid().ToString();
            string firstName = "First";
            string lastName = "Last";
            string email = "email";

            var student = new Student { UserID = id, Email = email, FirstName = firstName, LastName = lastName };

            var mapped = MapStudent.MapStudentDBModelToStudentListItemDTO(student);

            Assert.Equal(id, mapped.Id);
            Assert.Equal(firstName + " " + lastName, mapped.Name);
            Assert.Equal(email, mapped.Email);
        }

        [Fact]
        public void MapUserToUserManageListItem_WhenNullUserPassed_ReturnsNullListItem()
        {
            User usr = null;

            var listItem = MapUser.MapUserToUserManageListItem(usr);

            Assert.Null(listItem);
        }
    }
}

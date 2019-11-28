using StudyGroups.Data.DAL.DAOs;
using StudyGroups.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Services.Mapping
{
    public static class MapCourse
    {

        internal static CourseSubjectCode MapCourseExportToCourseSubjectCode(CourseExportModel courseExportModel, string semester)
        {
            Course course = new Course();
            CourseType type;
            if (courseExportModel.CourseType == "Elmélet" || courseExportModel.CourseType == "Theoretical")
                type = CourseType.Theoretical;
            else if (courseExportModel.CourseType == "Labor" || courseExportModel.CourseType == "Labour")
                type = CourseType.Labour;
            else
                type = CourseType.Labour;

            course.CourseCode = courseExportModel.CourseCode;
            course.Semester = semester;
            course.CourseType = (int)type;
            
            return new CourseSubjectCode{Course=course,SubjectCode=courseExportModel.SubjectCode };
        }



    }
}

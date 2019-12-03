using StudyGroups.Data.DAL.DAOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.Data.DAL.ConversionUtils
{
    public static class CourseExtensions
    {
        public static string GetCypherFormattedNodeParameters(this Course course)
        {
            string crs =
             $" CourseCode:'{course.CourseCode}', Semester:'{course.Semester}', CourseType:{course.CourseType} ";
            return crs;

        }
    }
}

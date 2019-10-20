using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.DataAccessLayer.DAOs
{
    public enum CourseType
    {
        Theoretical, Labour, Practical, Seminar, ELearning, Special, ExamCourse
    }

    public class CourseDBModel
    {
        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string Semester { get; set; }
        public CourseType CourseType { get; set; }
    }
}

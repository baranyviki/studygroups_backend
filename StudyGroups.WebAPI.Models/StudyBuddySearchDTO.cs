using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyGroups.WebAPI.Models
{
    public class StudyBuddySearchDTO
    {
        [FromQuery(Name = "sub")]
        public string SubjectControl { get; set; }
        [FromQuery(Name = "common")]
        public bool CommonCourseControl { get; set; }
        [FromQuery(Name = "curr")]
        public bool CurrentlyEnrolledControl { get; set; }
        [FromQuery(Name = "teacher")]
        public bool AnotherTeacherControl { get; set; }
        [FromQuery(Name = "completed")]
        public bool CompletedControl { get; set; }
        [FromQuery(Name = "grade")]
        public int GradeControl { get; set; }
        [FromQuery(Name = "disc")]
        public int DisciplinesControl { get; set; }

    }
}

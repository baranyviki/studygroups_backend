using Microsoft.AspNetCore.Mvc;

namespace StudyGroups.WebAPI.Models
{
    public class StudyBuddySearchDTO
    {
        [FromQuery(Name = "sub")]
        public string SubjectID { get; set; }
        [FromQuery(Name = "common")]
        public bool IsCommonCourse { get; set; }
        [FromQuery(Name = "curr")]
        public bool IsCurrentlyEnrolledTo { get; set; }
        [FromQuery(Name = "teacher")]
        public bool IsAttendingToAnotherTeacher { get; set; }
        [FromQuery(Name = "completed")]
        public bool IsAlreadyCompleted { get; set; }
        [FromQuery(Name = "grade")]
        public int ComletedWithGrade { get; set; }
        [FromQuery(Name = "disc")]
        public int GoodInDiscipline { get; set; }

    }
}

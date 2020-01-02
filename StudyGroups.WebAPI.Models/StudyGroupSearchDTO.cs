namespace StudyGroups.WebAPI.Models
{
    public class StudyGroupSearchDTO
    {
        public string CourseID { get; set; }
        public bool IsSameGradeAverage { get; set; }
        public bool IsSameCompletedSemesters { get; set; }
        public bool IsHavingOtherCourseInCommonCurrently { get; set; }
    }
}

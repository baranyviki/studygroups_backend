namespace StudyGroups.Data.DAL.DAOs
{
    public enum CourseType
    {
        Theoretical, Labour, Practical, Seminar, ELearning, Special, ExamCourse
    }

    public class Course
    {
        public string CourseID { get; set; }
        public string CourseCode { get; set; }
        public string Semester { get; set; }
        public int CourseType { get; set; }
    }
}

using StudyGroups.WebAPI.Models;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Utils
{
    public enum GradeTypes
    {
        Fail = 1, Pass = 2, Statisfactory = 3, Good = 4, Excellent = 5
    }

    /// <summary>
    /// Extension methods for GradeBookExports
    /// </summary>
    public static class GradeBookExportExtensions
    {
        public static GradeTypes? GetGrade(this GradeBookExportModel gradeBookExportModel)
        {

            var s = gradeBookExportModel.Grade.Replace("Dr. ", "Dr");
            //bool isHungarian = gradeBookExportModel.Completed == "Igen" || gradeBookExportModel.Completed == "Nem";
            var dataElements = s.Split(". ");
            string grade = dataElements.Last().Split(" ").FirstOrDefault();


            if (gradeBookExportModel.Completed == "No" || gradeBookExportModel.Completed == "Nem")
            {
                if (string.IsNullOrEmpty(grade) && string.IsNullOrEmpty(gradeBookExportModel.Sign))
                {
                    return null;
                }
                else
                {
                    return GradeTypes.Fail;
                }
            }
            if (grade == "Fail" || grade == "Elégtelen")
            {
                return GradeTypes.Fail;
            }
            else if (grade == "Pass" || grade == "Elégséges")
            {
                return GradeTypes.Pass;
            }
            else if (grade == "Statisfactory" || grade == "Közepes")
            {
                return GradeTypes.Statisfactory;
            }
            else if (grade == "Good" || grade == "Jó")
            {
                return GradeTypes.Good;
            }
            else if (grade == "Excellent" || grade == "Jeles")
            {
                return GradeTypes.Excellent;
            }
            else
            {
                return null;
            }
        }

    }
}

using System;

namespace StudyGroups.WebAPI.Services.Utils
{
    public class SemesterManager
    {
        public static string GetCurrentSemester()
        {
            var currentdate = DateTime.Today;
            if (currentdate.Month >= 2 && currentdate.Month < 9)
            {
                //tavaszi felev
                return $"{currentdate.Year - 1 }/{currentdate.Year.ToString().Substring(0, 2) }/2";
            }
            else //oszi felev
            {
                return "2019/20/1";

                //return $"{currentdate.Year }/{(currentdate.Year + 1).ToString().Substring(2, 2)}/1";
            }

        }


    }
}

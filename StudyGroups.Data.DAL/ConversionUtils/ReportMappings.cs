using StudyGroups.Data.DAL.ProjectionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudyGroups.Data.DAL.ConversionUtils
{
    public static class ReportMappings
    {
        public static double[] MapSemesterAverageGroupingsToDoubleArray(IEnumerable<SemesterAverageGrouping> semesterAverages)
        {
            double[] array = new double[7];
            var list = semesterAverages.Where(x => x.SemesterCnt < 7).ToList();
                                    
            for (int i = 0; i < list.Count(); i++)
            {
                array[list[i].SemesterCnt] = list[i].Average;
            }

            var listOfOldies = semesterAverages.Where(x => x.SemesterCnt > 7).ToList();
            //7th is 7 or more semesters
            if (listOfOldies.Count() > 0)
            {
                array[6] = listOfOldies.Average(x => x.Average);
            }

            return array;
        }

    }
}

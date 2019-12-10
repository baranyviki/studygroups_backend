using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Utils
{
    public static class CSVProcesser
    {
        public static IEnumerable<T> ProcessCSV<T>(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<T>();
                return records.ToList();
            }
        }


    }
}

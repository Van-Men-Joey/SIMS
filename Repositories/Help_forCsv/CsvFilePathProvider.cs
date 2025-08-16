using System;
using System.IO;

namespace SIMS.Repositories.Help_forCsv
{
    public static class CsvFilePathProvider
    {
        public static string GetPath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", fileName);
        }
    }
}

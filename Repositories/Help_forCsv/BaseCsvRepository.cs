using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SIMS.Repositories.Help_forCsv
{
    public abstract class BaseCsvRepository<T>
    {
        private readonly string _csvFilePath;
        private readonly CsvConfiguration _csvConfig;

        protected BaseCsvRepository(string csvFilePath)
        {
            _csvFilePath = csvFilePath;

            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim()
            };

            EnsureFileExists();
        }
        protected List<T> ReadAll()
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, _csvConfig);
            csv.Context.RegisterClassMap(GetClassMap());
            return csv.GetRecords<T>().ToList();
        }

        protected void WriteAll(List<T> records)
        {
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, _csvConfig);
            csv.Context.RegisterClassMap(GetClassMap());
            csv.WriteHeader<T>();
            csv.NextRecord();
            foreach (var r in records)
            {
                csv.WriteRecord(r);
                csv.NextRecord();
            }
        }
        private void EnsureFileExists()
        {
            var dir = Path.GetDirectoryName(_csvFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_csvFilePath))
            {
                using var writer = new StreamWriter(_csvFilePath);
                using var csv = new CsvWriter(writer, _csvConfig);
                csv.Context.RegisterClassMap(GetClassMap());
                csv.WriteHeader<T>();
                csv.NextRecord();
            }
        }


      

        protected abstract ClassMap<T> GetClassMap();

       
      
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public class EnrollmentStatusConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (Enum.TryParse<EnrollmentStatus>(text, out var status))
                return status;
            return EnrollmentStatus.Pending; // default fallback
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is EnrollmentStatus status)
                return status.ToString();
            return "";
        }
    }
}

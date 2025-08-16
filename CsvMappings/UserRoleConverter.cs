using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public class UserRoleConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (Enum.TryParse<UserRole>(text, out var role))
                return role;
            return UserRole.Student; // default fallback
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is UserRole role)
                return role.ToString();
            return "";
        }
    }
}

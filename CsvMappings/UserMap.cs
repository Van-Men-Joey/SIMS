using CsvHelper.Configuration;
using SIMS.Models;

namespace SIMS.CsvMappings
{
    public sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.Id);
            Map(m => m.Email);
            Map(m => m.PasswordHash);
            Map(m => m.Role).TypeConverter<UserRoleConverter>();
            Map(m => m.Name);
            Map(m => m.Description);
        }
    }
}

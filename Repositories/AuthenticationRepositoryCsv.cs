using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings; // chứa UserMap và UserRoleConverter
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Repositories.Help_forCsv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Repositories
{
    public class AuthenticationRepositoryCsv
        : BaseCsvRepository<User>, IAuthenticationRepository
    {
        public AuthenticationRepositoryCsv()
            : base(CsvFilePathProvider.GetPath("users.csv"))
        {
        }

        protected override ClassMap<User> GetClassMap()
        {
            return new UserMap();
        }

        private List<User> ReadAllUsersWithRole()
        {
            var users = new List<User>();

            using var reader = new StreamReader(CsvFilePathProvider.GetPath("users.csv"));
            using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var roleStr = csv.GetField("Role");
                User user;

                switch (roleStr)
                {
                    case "Admin":
                        user = new Admin();
                        break;
                    case "Faculty":
                        user = new Faculty();
                        break;
                    case "Student":
                        user = new Student();
                        break;
                    default:
                        throw new Exception($"Unknown Role: {roleStr}");
                }

                user.Id = csv.GetField("Id");
                user.Email = csv.GetField("Email");
                user.PasswordHash = csv.GetField("PasswordHash");
                user.Role = Enum.Parse<UserRole>(roleStr);
                user.Name = csv.GetField("Name");
                user.Description = csv.GetField("Description");

                users.Add(user);
            }

            return users;
        }

        private void WriteAllUsersWithRole(List<User> users)
        {
            using var writer = new StreamWriter(CsvFilePathProvider.GetPath("users.csv"));
            using var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });

            csv.Context.RegisterClassMap<UserMap>();
            csv.WriteHeader<User>();
            csv.NextRecord();

            foreach (var user in users)
            {
                csv.WriteRecord(user);
                csv.NextRecord();
            }
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            var users = ReadAllUsersWithRole();
            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task AddUserAsync(User user)
        {
            var users = ReadAllUsersWithRole();

            if (users.Any(u => u.Id == user.Id))
                throw new Exception("User with same Id already exists");

            if (users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("User with same Email already exists");

            users.Add(user);
            WriteAllUsersWithRole(users);
            return Task.CompletedTask;
        }
    }
}

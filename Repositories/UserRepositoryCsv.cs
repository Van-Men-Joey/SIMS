using CsvHelper;
using CsvHelper.Configuration;
using SIMS.CsvMappings;
using SIMS.Interfaces;
using SIMS.Models;
using System.Globalization;
using System.Text;



namespace SIMS.Repositories
{
    public class UserRepositoryCsv : IUserRepository
    {
        private readonly string _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "csv", "users.csv");
        private readonly CsvConfiguration _csvConfig;

        public UserRepositoryCsv()
        {
            // CsvHelper config
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Trim(),
            };

            var dir = Path.GetDirectoryName(_csvFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_csvFilePath))
            {
                using var writer = new StreamWriter(_csvFilePath, false, Encoding.UTF8);
                writer.WriteLine("Id,Email,PasswordHash,Role,Name,Description");
            }
        }

        private List<User> ReadAllUsers()
        {
            var users = new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, _csvConfig);

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


        private void WriteAllUsers(List<User> users)
        {
            try
            {
                using var writer = new StreamWriter(_csvFilePath, false, Encoding.UTF8);
                using var csv = new CsvWriter(writer, _csvConfig);

                csv.Context.RegisterClassMap<UserMap>();
                csv.WriteHeader<User>();
                csv.NextRecord();

                foreach (var user in users)
                {
                    csv.WriteRecord(user);
                    csv.NextRecord();
                }
                Console.WriteLine($"Ghi CSV thành công, tổng số user: {users.Count}");
                Console.WriteLine("CSV file path: " + _csvFilePath);

            }
            catch (Exception ex)
            {
                // Ghi log hoặc in ra console xem lỗi gì
                Console.WriteLine("Lỗi khi ghi file CSV: " + ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            var users = ReadAllUsers();
            return Task.FromResult(users.AsEnumerable());
        }

        public Task<User?> GetByIdAsync(string id)
        {
            var user = ReadAllUsers().FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = ReadAllUsers().FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task AddAsync(User user)
        {
            var users = ReadAllUsers();

            if (users.Any(u => u.Id == user.Id))
                throw new System.Exception("User with same Id already exists");

            users.Add(user);
            WriteAllUsers(users);
            Console.WriteLine("Đã gọi WriteAllUsers để ghi CSV.");
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            var users = ReadAllUsers();
            var index = users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                throw new System.Exception("User not found");

            users[index] = user;
            WriteAllUsers(users);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var users = ReadAllUsers();
            var userToRemove = users.FirstOrDefault(u => u.Id == id);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
                WriteAllUsers(users);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
        {
            var faculties = ReadAllUsers().OfType<Faculty>();
            return Task.FromResult(faculties.AsEnumerable());
        }

    }
}

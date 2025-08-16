using SIMS.Models;

namespace SIMS.Factories
{
    public static class UserFactory
    {
        public static User CreateUserByRole(string role)
        {
            return role switch
            {
                "Student" => new Student(),
                "Teacher" => new Faculty(),
                "Admin" => new Admin(),
                _ => throw new ArgumentException("Invalid role")
            };
        }
    }
}

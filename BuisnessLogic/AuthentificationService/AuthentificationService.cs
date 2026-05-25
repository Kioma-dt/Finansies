using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.AuthentificationService
{
    public class AuthentificationBCryptService 
        : IAuthentificationService
    {
        private readonly IUserRepository _userRepository;
        public AuthentificationBCryptService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> LogIn(string name, string password)
        {
            var user = await _userRepository.GetByName(name);

            if(user is null)
            {
                throw new ArgumentException($"No Such User({name})");
            }

            var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!valid)
            {
                throw new ArgumentException($"Wrong Password");
            }


            return user;
        }
        public async Task<User?> Register(string name, string password)
        {
            var existingUser = await _userRepository.GetByName(name);
            if (existingUser != null)
                throw new ArgumentException($"Such user ({name}) already exists");

            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                PasswordHash = hash
            };

            return newUser;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using BuisnessLogic.Entities;

namespace BuisnessLogic.AuthentificationService
{
    public interface IAuthentificationService
    {
        Task<User?> Register(string name, string password);
        Task<User?> LogIn(string name, string password);
    }
}

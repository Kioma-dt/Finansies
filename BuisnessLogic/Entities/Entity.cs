using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Entities
{
    public class Entity
    {
        public Guid Id { get; set; }
    }

    public class UsersEntity
        : Entity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}

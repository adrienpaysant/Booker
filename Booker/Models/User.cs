using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booker.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Type { get; set; }
    }
}

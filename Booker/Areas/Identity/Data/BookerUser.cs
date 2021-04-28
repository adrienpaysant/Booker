using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Booker.Models;
using Microsoft.AspNetCore.Identity;

namespace Booker.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BookerUser class
    public class BookerUser : IdentityUser
    {
        [PersonalData]
        public string Avatar { get; set; }
        [PersonalData]
        public bool IsAuthor { get; set; }
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }

        public string GetFullName(){
            return $"{FirstName} {LastName}";
        }
    }
}

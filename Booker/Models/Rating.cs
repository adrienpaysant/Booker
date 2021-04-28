using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booker.Areas.Identity;
namespace Booker.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string BookerUserId { get; set; }
        public int Value { get; set; }
        public string BookId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Booker.Models
{
    public class Book
    {
        [Key]
        public int ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Image { get; set; }
        public string Categories{ get; set; }
        public string BuyLink{ get; set; }
    }
}

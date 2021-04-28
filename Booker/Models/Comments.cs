using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Booker.Areas.Identity.Data;
namespace Booker.Models
{
    public class Comments
    {
        public string Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string BookerUserId { get; set; }
        [Required]
        public string BookId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
    }
}

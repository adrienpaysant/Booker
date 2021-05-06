using System;
using System.ComponentModel.DataAnnotations;
namespace Booker.Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Booker.Areas.Identity.Data;
namespace Booker.Models
{
    public class Book
    {
        [Key]
        [Required]
        [MaxLength(13)]
        [MinLength(13)]
        [RegularExpression("^[0-9]*$",ErrorMessage = "ISBN must be numeric")]
        public string ISBN { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string BookerUserId { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Editor { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Image { get; set; }
        [Required]
        [Display(Name = "Categories (separated by a comma)")]
        public string Categories { get; set; }
        [Required]
        public string BuyLink { get; set; }

        [Display(Name = "Rating /5")]
        public float? Rating { get; set; }
        public List<Comments> CommentList { get; set; }
    }
}

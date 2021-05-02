using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public List<string> CategoriesList(){
            return Categories.Split(',').Select(p => p.Trim()).ToList(); 
        }

        [Required]
        public string BuyLink { get; set; }
    }
}

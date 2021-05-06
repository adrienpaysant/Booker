using System.Collections.Generic;
using Booker.Models;

/// <summary>
/// Used to display book and its comments
/// </summary>
namespace Booker.ViewModels
{
    public class BookCommentViewModel
    {
        public Book Book { get; set; }
        public List<Comments> Comments { get; set; }
    }
}

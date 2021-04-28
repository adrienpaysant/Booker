using System;
using System.Collections.Generic;
using Booker.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Booker.ViewModels
{
    public class BookCommentViewModel
    {
        public Book Book { get; set; }
        public List<Comments> Comments { get; set; }
    }
}

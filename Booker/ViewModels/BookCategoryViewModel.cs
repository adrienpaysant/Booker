using Booker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booker.ViewModels
{
    public class BookCategoryViewModel
    {
        public List<Book> Books { get; set; }
        public SelectList Categories { get; set; }
        public string BookCategory { get; set; }
        public string SearchString { get; set; }
    }
}

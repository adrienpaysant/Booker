using Booker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Booker.ViewModels
{
    public class BookCategoryViewModel
    {
        public List<Book> Books { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Order { get; set; }
        public string BookCategory { get; set; }
        public string SearchString { get; set; }
        public string OrderString { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booker.ViewModels
{
    public class BookViewModel
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile Image { get; set; }
        public string Categories { get; set; }
        public string BuyLink { get; set; }
    }
}

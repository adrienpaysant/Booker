using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booker.Data;
using Booker.Models;
using Booker.Areas.Identity.Data;
using Booker.ViewModels;
using System.IO;


namespace Booker.Controllers
{
    enum Order: ushort
    {
        TITLE_ASC,
        TITLE_DESC,
        ConnectionLost = 100,
        OutlierReading = 200
    }

    public class BooksController: Controller
    {
        private readonly BookerContextId _context;
        private readonly UserManager<BookerUser> _userManager;
        private readonly string[] order = { "Title Ascending","Title Descending","Date Ascending","Date Descending" };

        public BooksController(BookerContextId context,UserManager<BookerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get Books page with a list of book filtered by the user via the search filters
        /// </summary>
        /// <param name="bookCategory"></param>
        /// <param name="searchString"></param>
        /// <param name="orderString"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string bookCategory,string searchString,string orderString,DateTime? fromDate,DateTime? toDate)
        {
            var books = from b in _context.Book select b;

            // Sort 
            books = orderString switch
            {
                "Title Descending" => books.OrderByDescending(b => b.Title),
                "Date Ascending" => books.OrderBy(b => b.ReleaseDate),
                "Date Descending" => books.OrderByDescending(b => b.ReleaseDate),
                _ => books.OrderBy(b => b.Title),
            };

            // Date default values
            if(!fromDate.HasValue)
                fromDate = (from b in books select b.ReleaseDate).Min();

            if(!toDate.HasValue)
                toDate = (from b in books select b.ReleaseDate).Max();

            // Filter by date
            books = books.Where(b => b.ReleaseDate >= fromDate && b.ReleaseDate <= toDate);

            // Filter with text
            if(!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s =>
                    EF.Functions.Like(s.Title,$"%{searchString}%") ||
                    EF.Functions.Like(s.Author,$"%{searchString}%") ||
                    EF.Functions.Like(s.Editor,$"%{searchString}%"));
            }

            // Filter by categories
            var booksList = await books.ToListAsync();
            if(!string.IsNullOrEmpty(bookCategory))
            {
                booksList.Clear();
                foreach(var book in books)
                    if(book.CategoriesList().Contains(bookCategory))
                        booksList.Add(book);
            }

            // Get all categories
            var categoriesList = new List<string>();
            foreach(var book in books)
                foreach(var category in book.CategoriesList())
                    if(!categoriesList.Contains(category))
                        categoriesList.Add(category);
            categoriesList.Sort();

            var bookGenreVM = new BookCategoryViewModel
            {
                Categories = new SelectList(categoriesList),
                Books = booksList,
                Order = new SelectList(order),
                FromDate = fromDate.GetValueOrDefault(),
                ToDate = toDate.GetValueOrDefault()
            };

            BookerUser user = await _userManager.GetUserAsync(User);
            if(user != null) ViewData["IsAuthor"] = user.IsAuthor.ToString();

            return View(bookGenreVM);

        }
        /// <summary>
        /// Get all the books written by the current user
        /// Get all the books written by the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MyBooks()
        {
            BookerUser user = await _userManager.GetUserAsync(User);
            var books = _context.Book.Where(b => b.BookerUserId == user.Id);

            if(books.AsEnumerable().ToList().Count < 1)
                books = _context.Book;
            // Sort 
            books = books.OrderByDescending(b => b.ReleaseDate);

            // Get all categories
            var categoriesList = new List<string>();
            foreach(var book in books)
                foreach(var category in book.CategoriesList())
                    if(!categoriesList.Contains(category))
                        categoriesList.Add(category);

            var bookGenreVM = new BookCategoryViewModel
            {
                Categories = new SelectList(categoriesList),
                Books = books.AsEnumerable().ToList(),
                Order = new SelectList(order),
                FromDate = (from b in books select b.ReleaseDate).Min(),
                ToDate = (from b in books select b.ReleaseDate).Max()
            };

            if(user != null) ViewData["IsAuthor"] = user.IsAuthor.ToString();

            return View("Index",bookGenreVM);
        }
        // GET: Books/Details/5
        /// <summary>
        /// Get books details 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            BookerUser user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();
            ViewData["IsAuthor"] = user.IsAuthor.ToString();
            try
            {
                Rating oldRating = _context.Rating.Where(r => r.BookerUserId.Equals(user.Id) && r.BookId.Equals(id)).First();
                ViewBag.rating = oldRating.Value;
            }
            catch { }
            try
            {
                List<Rating> ratingList = _context.Rating.Where(r => r.BookId.Equals(id)).ToList();
                ViewBag.ratingSum = ((float)ratingList.Sum(r => r.Value)) / ratingList.Count;
            }
            catch { }
            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.ISBN.Equals(id));
            if(book == null)
            {
                return NotFound();
            }
            List<Comments> comments = _context.Comments.Where(c =>c.BookId.Equals(id)).ToList();
            comments.Reverse();
            return View(new BookCommentViewModel() { Book = book,Comments = comments });
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Upload image to wwwroot/img and return the path
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        static private string UploadedFile(BookViewModel model)
        {
            string filePath = null;
            if(model.Image != null)
            {
                filePath = $"/img/{model.ISBN}.{model.Image.FileName.Split(".").Last()}";
                using var fileStream = new FileStream("wwwroot" + filePath,FileMode.Create);
                model.Image.CopyTo(fileStream);
            }
            else
            {
                string[] files = Directory.GetFiles("wwwroot/img/",model.ISBN + ".*",SearchOption.TopDirectoryOnly);
                if(files.Length == 1) return files[0][7..]; //take it from /img/
            }
            return filePath;
        }

        static private void DeleteImage(string ISBN)
        {
            string[] files = Directory.GetFiles("wwwroot/img/",ISBN + ".*",SearchOption.TopDirectoryOnly);
            if(files.Length == 1) System.IO.File.Delete(files[0]);
        }

        /// <summary>
        /// Change image Id when the book image is updated
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ISBN"></param>
        static private void ChangeImageId(string id,string ISBN)
        {
            string[] files = Directory.GetFiles("wwwroot/img/",id + ".*",SearchOption.TopDirectoryOnly);
            if(files.Length == 1) System.IO.File.Move(files[0],files[0].Replace(id,ISBN));
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel vm)
        {
            Book book = null;
            if(ModelState.IsValid)
            {
                book = new Book
                {
                    Author = vm.Author,
                    ISBN = vm.ISBN,
                    BuyLink = vm.BuyLink,
                    Categories = vm.Categories,
                    Description = vm.Description,
                    Editor = vm.Editor,
                    Title = vm.Title,
                    ReleaseDate = vm.ReleaseDate,
                    Image = UploadedFile(vm),
                    BookerUserId = _userManager.GetUserId(User)
                };
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        /// <summary>
        /// Rate a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id,int rating)
        {
            if(id == null)
            {
                return NotFound();
            }
            Book book = await _context.Book.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            string currentUserId = _userManager.GetUserId(User);
            Rating newRating = new Rating
            {
                BookerUserId = currentUserId,
                BookId = id,
                Value = rating
            };
            try
            {
                Rating oldRating = _context.Rating.Where(r => r.BookerUserId.Equals(currentUserId) && r.BookId.Equals(id)).First();
                _context.Rating.Remove(oldRating);
            }
            catch { }

            _context.Rating.Add(newRating);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details));
        }
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,BookViewModel vm)
        {
            Book book = null;
            if(!id.Equals(vm.ISBN))
            {
                if(id != null)
                {
                    var bookToDelete = await _context.Book.FindAsync(id);
                    _context.Book.Remove(bookToDelete);
                    await _context.SaveChangesAsync();
                    ChangeImageId(id,vm.ISBN);
                    return await Create(vm);
                }
                else
                {
                    return NotFound();
                }
            }

            if(ModelState.IsValid)
            {
                try
                {
                    book = new Book
                    {
                        Author = vm.Author,
                        ISBN = vm.ISBN,
                        BuyLink = vm.BuyLink,
                        Categories = vm.Categories,
                        Description = vm.Description,
                        Editor = vm.Editor,
                        Title = vm.Title,
                        ReleaseDate = vm.ReleaseDate,
                        Image = UploadedFile(vm),
                        BookerUserId = _userManager.GetUserId(User)
                    };
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!BookExists(book.ISBN))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.ISBN.Equals(id));

            if(book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            DeleteImage(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Create Comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(string id,string content)
        {
            Comments comment = null;
            if(ModelState.IsValid)
            {
                comment = new Comments
                {
                    BookId = id,
                    Content = content,
                    PublicationDate = DateTime.Now,
                    BookerUserId = _userManager.GetUserId(User)
                };
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details),new { id });
        }

        /// <summary>
        /// Edit comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int commentId,string id,string content)
        {
            Comments comment = null;
            if(ModelState.IsValid)
            {
                comment = new Comments
                {
                    Id = commentId,
                    BookId = id,
                    Content = content,
                    PublicationDate = DateTime.Now,
                    BookerUserId = _userManager.GetUserId(User)
                };
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details),new { id });
        }

        /// <summary>
        /// Delete Comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int? commentId,string id)
        {
            if(commentId == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(commentId);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new { id });
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.ISBN.Equals(id));
        }
    }
}

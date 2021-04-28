﻿using System;
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
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Booker.Controllers
{
    public class BooksController: Controller
    {
        private readonly BookerContextId _context;
        private readonly UserManager<BookerUser> _userManager;

        public BooksController(BookerContextId context,UserManager<BookerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookCategory,string searchString)
        {
            IQueryable<string> genreQuery = from b in _context.Book
                                            orderby b.Categories
                                            select b.Categories;

            var books = from b in _context.Book select b;

            if(!String.IsNullOrEmpty(searchString))
                books = books.Where(s => EF.Functions.Like(s.Title,$"%{searchString}%"));

            if(!string.IsNullOrEmpty(bookCategory))
                books = books.Where(x => x.Categories == bookCategory);

            var movieGenreVM = new BookGenreViewModel
            {
                Categories = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Books = await books.ToListAsync()
            };
            BookerUser user = await _userManager.GetUserAsync(User);
            if(user != null) ViewData["IsAuthor"] = user.IsAuthor.ToString();

            return View(movieGenreVM);

        }
        // GET: Books/Details/5
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
            List<Comments> comments = _context.Comments.ToList();
            comments.Reverse();
            return View(new BookCommentViewModel() { Book = book,Comments = comments});
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

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

        //Rating
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
        public async Task<IActionResult> Edit(string? id)
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
            return RedirectToAction(nameof(Details),new { id=id});
        }
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
            return RedirectToAction(nameof(Details),new { id = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId,string id)
        {
            if(commentId == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(commentId);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new { id = id });
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.ISBN.Equals(id));
        }
    }
}

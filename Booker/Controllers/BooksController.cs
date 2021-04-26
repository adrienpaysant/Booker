using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booker.Data;
using Booker.Models;
using Booker.ViewModels;
using System.Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Booker.Controllers
{
    public class BooksController: Controller
    {
        private readonly BookerContextId _context;

        public BooksController(BookerContextId context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if(book == null)
            {
                return NotFound();
            }

            return View(book);
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
                filePath = "/img/" + model.ISBN + "." + model.Image.FileName.Split(".").Last();
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

        static private void DeleteImage(int ISBN)
        {
            string[] files = Directory.GetFiles("wwwroot/img/",ISBN + ".*",SearchOption.TopDirectoryOnly);
            if(files.Length == 1) System.IO.File.Delete(files[0]);
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
                string uniqueFileName = UploadedFile(vm);
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
                    Image = uniqueFileName,
                };
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id,BookViewModel vm)
        {
            Book book = null;
            if(id != vm.ISBN)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(vm);
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
                        Image = uniqueFileName,
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
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.ISBN == id);
            
            if(book == null)
            {
                return NotFound();
            }
            DeleteImage((int)id);//cast ok because id is verified in first "if"
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ISBN == id);
        }
    }
}

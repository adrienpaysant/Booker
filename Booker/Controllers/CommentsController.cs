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
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Booker.Controllers
{
    public class CommentsController: Controller
    {
        private readonly BookerContextId _context;
        private readonly UserManager<BookerUser> _userManager;

        public CommentsController(BookerContextId context,UserManager<BookerUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string bookId,string content)
        {
            Comments comment = null;
            if(ModelState.IsValid)
            {
                comment = new Comments
                {
                    BookId = bookId,
                    Content = content,
                    PublicationDate = DateTime.Now,
                    BookerUserId = _userManager.GetUserId(User)
                };
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,string bookId,string content)
        {
            Comments comment = null;
            if(ModelState.IsValid)
            {
                comment = new Comments
                {
                    Id = id,
                    BookId = bookId,
                    Content = content,
                    PublicationDate = DateTime.Now,
                    BookerUserId = _userManager.GetUserId(User)
                };
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDDN.CrossBlog.Blog.Model;
using DDDN.CrossBlog.Blog.Routing;

namespace DDDN.CrossBlog.Blog.Areas.Dashboard.Controllers
{
	[Area("Dashboard")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class PostsController : Controller
	{
		private readonly CrossBlogContext _context;

		public PostsController(CrossBlogContext context)
		{
			_context = context;
		}

		// GET: Dashboard/Posts
		public async Task<IActionResult> Index()
		{
			return View(await _context.Posts.ToListAsync());
		}

		// GET: Dashboard/Posts/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				 .SingleOrDefaultAsync(m => m.PostId == id);
			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// GET: Dashboard/Posts/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Dashboard/Posts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("PostId,State,Created,Title")] Post post)
		{
			if (ModelState.IsValid)
			{
				post.PostId = Guid.NewGuid();
				_context.Add(post);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(post);
		}

		// GET: Dashboard/Posts/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
			if (post == null)
			{
				return NotFound();
			}
			return View(post);
		}

		// POST: Dashboard/Posts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("PostId,State,Created,Title")] Post post)
		{
			if (id != post.PostId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(post);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PostExists(post.PostId))
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
			return View(post);
		}

		// GET: Dashboard/Posts/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				 .SingleOrDefaultAsync(m => m.PostId == id);
			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// POST: Dashboard/Posts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PostExists(Guid id)
		{
			return _context.Posts.Any(e => e.PostId == id);
		}
	}
}

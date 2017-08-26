using DDDN.CrossBlog.Blog.Areas.Administration.Model;
using DDDN.CrossBlog.Blog.Model;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Areas.Administration.Controllers
{
	[Area("Administration")]
	[MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
	public class WritersController : Controller
	{
		private readonly CrossBlogContext _context;

		public WritersController(CrossBlogContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.Writers.ToListAsync());
		}

		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				 .SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			return View(writer);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(WriterView writerView)
		{
			var writer = new Writer
			{
				Name = writerView.Name,
				Mail = writerView.Mail,
				WriterId = Guid.NewGuid(),
				Password = Encoding.Unicode.GetBytes(writerView.Password),
				Salt = Encoding.Unicode.GetBytes(writerView.Name + writerView.Mail),
				State = "1",
				Created = DateTimeOffset.Now
			};

			_context.Add(writer);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers.SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			var writerView = new WriterView
			{
				WriterId = writer.WriterId,
				State = writer.State,
				Mail = writer.Mail,
				Name = writer.Name
			};

			return View(writerView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("WriterId, State, Name, Mail, Password, PasswordCompare")]WriterView writerView)
		{
			if (id != writerView.WriterId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				var writer = new Writer
				{
					Name = writerView.Name,
					Mail = writerView.Mail,
					WriterId = writerView.WriterId,
					Password = Encoding.Unicode.GetBytes(writerView.Password),
					Salt = Encoding.Unicode.GetBytes(writerView.Name + writerView.Mail),
					State = writerView.State,
				};

				try
				{
					_context.Update(writer);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!WriterExists(writer.WriterId))
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
			return View(writerView);
		}

		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var writer = await _context.Writers
				 .SingleOrDefaultAsync(m => m.WriterId == id);
			if (writer == null)
			{
				return NotFound();
			}

			return View(writer);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			var writer = await _context.Writers.SingleOrDefaultAsync(m => m.WriterId == id);
			_context.Writers.Remove(writer);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool WriterExists(Guid id)
		{
			return _context.Writers.Any(e => e.WriterId == id);
		}
	}
}

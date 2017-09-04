/*
* DDDN.CrossBlog.Blog.Areas.Administration.PostsController
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de, devdone@outlook.com)
*
* This program is free software; you can redistribute it and/or modify it under the terms of the
* GNU General Public License as published by the Free Software Foundation; version 2 of the License.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
* warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program; if not, write
* to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.Areas.Administration
{
   [Area("Administration")]
   [MiddlewareFilter(typeof(BlogCulturesMiddlewareFilter))]
   public class PostsController : Controller
   {
      private readonly CrossBlogContext _context;

      public PostsController(CrossBlogContext context)
      {
         _context = context ?? throw new ArgumentNullException(nameof(context));
      }
      /// <summary>
      /// Shows a table with posts infromations.
      /// </summary>
      /// <returns>List of post models.</returns>
      public async Task<IActionResult> Index()
      {
         var posts = await _context.Posts.AsNoTracking().ToListAsync();
         return View(posts);
      }
      /// <summary>
      /// Renders post information for post deletion.
      /// </summary>
      /// <param name="id">Post id.</param>
      /// <returns>Post model.</returns>
      public async Task<IActionResult> Delete(Guid id)
      {
         if (id.Equals(Guid.Empty))
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
      /// <summary>
      /// Delete a post and redirect to the index action.
      /// </summary>
      /// <param name="id">Post id.</param>
      /// <returns>Redirection to Index action.</returns>
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(Guid id)
      {
         var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
         _context.Posts.Remove(post);
         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }
   }
}
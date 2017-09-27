/*
DDDN.CrossBlog.Blog.Data.BusinessLayer.PostBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using DDDN.Office.Odf.Odt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class PostBusinessLayer : IPostBusinessLayer
	{
		private readonly CrossBlogContext _context;
		private RoutingConfigSection _routingConfig;

		public PostBusinessLayer(CrossBlogContext context, RoutingConfigSection routingConfigSection)
		{
			_routingConfig = routingConfigSection ?? throw new ArgumentNullException(nameof(routingConfigSection));
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<PostModel>> GetNewest(int skip, int take)
		{
			var newestPosts = await _context.Posts
				 .AsNoTracking()
				 .Include(p => p.Writer)
				 .OrderByDescending(p => p.Created)
				 .Skip(skip)
				 .Take(take)
				 .Include(pc => pc.PostCategories)
				 .ThenInclude(c => c.Category)
				 .ToListAsync();

			return newestPosts;
		}

		public async Task<IEnumerable<PostModel>> GetNewestByCategory(int skip, int take, Guid categoryId)
		{
			var newestPosts = await _context.Posts
				.AsNoTracking()
				.Where(post => _context.PostCategories.Any(pt => pt.PostId == post.PostId && pt.CategoryId == categoryId))
				.Skip(skip)
				.Take(take)
				.OrderByDescending(po => po.Created)
				.Select(post => post)
				.Include(w => w.Writer)
				.Include(pc => pc.PostCategories)
				.ThenInclude(c => c.Category)
				.ToListAsync();

			return newestPosts;
		}

		public async Task<PostModel> GetPostOrDefault(Guid postId)
		{
			return await _context.Posts
				.AsNoTracking()
				.Where(p => p.PostId == postId)
				.FirstOrDefaultAsync();
		}

		public async Task<PostModel> GetPostWithCommentsOrDefault(Guid postId)
		{
			return await _context.Posts
				.AsNoTracking()
				.Include(p => p.Comments)
				.AsNoTracking()
				.Where(p => p.PostId == postId)
				.FirstOrDefaultAsync();
		}

		public async Task<PostModel> GetWithCategories(Guid postId)
		{
			var post = await _context.Posts
				.AsNoTracking()
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId == postId);

			if (post == default(PostModel))
			{
				throw new PostNotFoundException(postId);
			}

			return post;
		}

		public async Task CommentSave(Guid postId, string personName, string commentTitle, string commentText)
		{
			var post = await _context.Posts
				.Where(p => p.PostId.Equals(postId))
				.Include(p => p.Comments)
				.FirstOrDefaultAsync();

			if (post == default(PostModel))
			{
				throw new PostNotFoundException(postId);
			}

			var comment = new CommentModel
			{
				CommentId = Guid.NewGuid(),
				Created = DateTimeOffset.Now,
				State = CommentModel.States.Visible,
				Parent = null,
				ParentId = null,
				Post = post,
				Name = personName,
				Title = commentTitle,
				Text = commentText
			};

			post.Comments.Add(comment);
			await _context.SaveChangesAsync();

		}

		public async Task Upload(IList<IFormFile> files)
		{
			List<PostModel> posts = new List<PostModel>();

			using (var sha1 = new SHA1Managed())
			{
				foreach (var file in files)
				{
					using (var fileContent = new MemoryStream())
					{
						await file.CopyToAsync(fileContent);
						var fileContentBytes = fileContent.ToArray();
						var sha1Hash = sha1.ComputeHash(fileContentBytes);

						using (IODTFile odtFile = new ODTFile(fileContent))
						{
							var odtConvert = new ODTConvert(odtFile, _routingConfig.BlogPostHtmlUrlPrefix);
							var convertedData = odtConvert.Convert(new ODTConvertSettings
							{
								FluidWidth = false
							});

							var now = DateTimeOffset.Now;

							var post = new PostModel
							{
								PostId = Guid.NewGuid(),
								State = PostModel.States.Uploaded,
								Created = now,
								Binary = fileContentBytes,
								Hash = sha1Hash,
								FirstHeader = convertedData.FirstHeader,
								FirstParagraph = convertedData.FirstParagraph,
								Html = convertedData.Html,
								Css = convertedData.Css,
								PageCssClassName = convertedData.PageCssClassName,
								Writer = _context.Writers.First(),
								Contents = new List<ContentModel>(),
								LastRenderd = now
							};

							foreach (var ec in convertedData.EmbedContent)
							{
								var content = new ContentModel
								{
									Binary = ec.Content,
									Created = now,
									Name = ec.Name,
									State = ContentModel.States.Visible,
									ContentId = ec.Id,
									Post = post
								};

								post.Contents.Add(content);
							}

							posts.Add(post);
						}
					}
				}
			}

			_context.AddRange(posts);
			await _context.SaveChangesAsync();
		}

		public async Task<(byte[] binary, string name)> GetContent(Guid contentId)
		{
			var content = await _context.Contents
				.Where(p => p.ContentId.Equals(contentId))
				.FirstOrDefaultAsync();

			if (content == default(ContentModel))
			{
				throw new PostContentNotFoundException(contentId);
			}
			else
			{
				return (content.Binary, content.Name);
			}
		}

		public async Task Delete(Guid postId)
		{
			var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId.Equals(postId));
			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();
		}

		public async Task Edit(PostViewModel postViewModel, IList<string> categoryIds)
		{
			if (postViewModel == null)
			{
				throw new ArgumentNullException(nameof(postViewModel));
			}

			if (categoryIds == null)
			{
				throw new ArgumentNullException(nameof(categoryIds));
			}

			var post = await _context.Posts
				.Include(p => p.PostCategories)
				.SingleOrDefaultAsync(m => m.PostId.Equals(postViewModel.PostId));

			if (post == default(PostModel))
			{
				throw new PostNotFoundException(postViewModel.PostId);
			}

			post.State = postViewModel.State;
			post.AlternativeTitle = postViewModel.AlternativeTitle;
			post.AlternativeTeaser = postViewModel.AlternativeTeaser;

			if (post.State == PostModel.States.Published)
			{
				if (post.FirstPublished == null)
				{
					post.FirstPublished = postViewModel.Published;
				}

				post.LastPublished = postViewModel.Published;
			}

			for (int i = post.PostCategories.Count() - 1; i >= 0; i--)
			{
				var cat = post.PostCategories.ElementAt(i);

				if (categoryIds.Contains(cat.CategoryId.ToString()))
				{
					categoryIds.Remove(cat.CategoryId.ToString());
				}
				else
				{
					post.PostCategories.Remove(cat);
				}
			}

			var newCats = new List<PostCategoryMap>();

			foreach (var catIdStr in categoryIds)
			{
				var catId = Guid.Parse(catIdStr);

				if (!post.PostCategories.Where(p => p.CategoryId.Equals(catId)).Any())
				{
					var pm = new PostCategoryMap
					{
						CategoryId = catId,
						PostId = post.PostId
					};

					newCats.Add(pm);
				}
			}

			post.PostCategories.AddRange(newCats);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PostExists(post.PostId))
				{
					throw new PostNotFoundException(post.PostId);
				}
				else
				{
					throw;
				}
			}
		}

		private bool PostExists(Guid postId)
		{
			return _context.Posts.Any(e => e.PostId.Equals(postId));
		}
	}
}

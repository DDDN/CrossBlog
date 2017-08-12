/*
* DDDN.CrossBlog.Blog.Model.CrossBlogContext
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

namespace DDDN.CrossBlog.Blog.Model
{
	using Microsoft.EntityFrameworkCore;
	using DDDN.CrossBlog.Blog.Model.Data;

	public partial class CrossBlogContext : DbContext
	{
		public CrossBlogContext(DbContextOptions<CrossBlogContext> options)
				: base(options)
		{ }

		public virtual DbSet<Blog> Blogs { get; set; }
		public virtual DbSet<Writer> Writers { get; set; }
		public virtual DbSet<WriterPostMap> WriterPosts { get; set; }
		public virtual DbSet<Post> Posts { get; set; }
		public virtual DbSet<PostKeywordMap> PostKeywords { get; set; }
		public virtual DbSet<Keyword> Keywords { get; set; }
		public virtual DbSet<Comment> Comments { get; set; }
		public virtual DbSet<Document> Documents { get; set; }
		public virtual DbSet<Content> Contents { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// WriterPost
			modelBuilder.Entity<WriterPostMap>()
				.HasKey(t => new { t.WriterId, t.PostId });

			modelBuilder.Entity<WriterPostMap>()
				 .HasOne(pt => pt.Writer)
				 .WithMany(t => t.WriterPosts)
				 .HasForeignKey(pt => pt.WriterId);

			modelBuilder.Entity<WriterPostMap>()
				.HasOne(pt => pt.Post)
				.WithMany(p => p.WriterPosts)
				.HasForeignKey(pt => pt.PostId);

			// PostKeyword
			modelBuilder.Entity<PostKeywordMap>()
				.HasKey(t => new { t.PostId, t.KeywordId });

			modelBuilder.Entity<PostKeywordMap>()
				.HasOne(pt => pt.Post)
				.WithMany(p => p.PostKeywords)
				.HasForeignKey(pt => pt.PostId);

			modelBuilder.Entity<PostKeywordMap>()
				.HasOne(pt => pt.Keyword)
				.WithMany(t => t.PostKeywords)
				.HasForeignKey(pt => pt.KeywordId);
		}
	}
}

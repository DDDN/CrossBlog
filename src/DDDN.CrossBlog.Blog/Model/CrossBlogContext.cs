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

    public class CrossBlogContext : DbContext
    {
        public CrossBlogContext(DbContextOptions<CrossBlogContext> options)
                : base(options)
        { }

        public DbSet<BlogInfo> BlogInfo { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategoryMap> PostCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Content> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PostKeyword
            modelBuilder.Entity<PostCategoryMap>()
                .HasKey(t => new { t.PostId, t.CategoryId });

            modelBuilder.Entity<PostCategoryMap>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostCategoryMap>()
                .HasOne(pt => pt.Category)
                .WithMany(t => t.PostCategories)
                .HasForeignKey(pt => pt.CategoryId);
        }
    }
}

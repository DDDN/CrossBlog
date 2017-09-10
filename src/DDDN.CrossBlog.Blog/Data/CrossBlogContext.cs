/*
DDDN.CrossBlog.Blog.Data.CrossBlogContext
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

namespace DDDN.CrossBlog.Blog.Data
{
   using DDDN.CrossBlog.Blog.Models;
   using Microsoft.EntityFrameworkCore;

   public class CrossBlogContext : DbContext
   {
      public CrossBlogContext(DbContextOptions<CrossBlogContext> options)
              : base(options)
      { }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
         optionsBuilder
            .EnableSensitiveDataLogging();

      public DbSet<BlogModel> Blog { get; set; }
      public DbSet<WriterModel> Writers { get; set; }
		public DbSet<RoleModel> Roles { get; set; }
		public DbSet<PostModel> Posts { get; set; }
      public DbSet<PostCategoryMap> PostCategories { get; set; }
      public DbSet<CategoryModel> Categories { get; set; }
      public DbSet<CommentModel> Comments { get; set; }
      public DbSet<ContentModel> Contents { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         // Post to Categories many to many
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

         // Post-Content cascade delete
         modelBuilder.Entity<PostModel>()
            .HasMany(co => co.Contents)
            .WithOne(p => p.Post)
            .OnDelete(DeleteBehavior.Cascade);

         // Post-Comments cascade delete
         modelBuilder.Entity<PostModel>()
            .HasMany(co => co.Comments)
            .WithOne(p => p.Post)
            .OnDelete(DeleteBehavior.Cascade);

         // Post-Categories cascade delete
         modelBuilder.Entity<PostModel>()
            .HasMany(co => co.PostCategories)
            .WithOne(p => p.Post)
            .OnDelete(DeleteBehavior.Cascade);
      }
   }
}

/*
DDDN.CrossBlog.Blog.Data.CrossBlogContextInitializer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DDDN.CrossBlog.Blog.Data
{
	public class CrossBlogContextInitializer
	{
		private readonly CrossBlogContext _context;
		private readonly SeedConfigSection _seedConfigSection;

		public CrossBlogContextInitializer(
			CrossBlogContext context,
			IOptions<SeedConfigSection> seedConfigSectionOptions)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));

			if (seedConfigSectionOptions == null)
			{
				throw new ArgumentNullException(nameof(seedConfigSectionOptions));
			}
			_seedConfigSection = seedConfigSectionOptions.Value ?? throw new ArgumentNullException(nameof(seedConfigSectionOptions));
		}

		public void Initialize()
		{
			if (_context.Database.EnsureCreated())
			{
				Seed();
			}
		}

		private void Seed()
		{
			var hashed = Crypto.HashWithSHA256("Qwerty!23");
			var version = typeof(Startup).GetTypeInfo().Assembly.GetName().Version.ToString();
			var now = DateTimeOffset.Now;

			var writer = new WriterModel
			{
				WriterId = Guid.NewGuid(),
				Created = now,
				Mail = _seedConfigSection.OwnerMail,
				State = WriterModel.States.Active,
				Name = _seedConfigSection.OwnerName,
				PasswordHash = hashed.hashedPassword,
				Salt = hashed.salt,
				MailConfirmed = false,
				Roles = new List<RoleModel>
				{
					{ RoleModel.Get(RoleModel.Roles.Administrator) },
					{ RoleModel.Get(RoleModel.Roles.Writer) }
				}
			};

			var blog = new BlogModel
			{
				BlogId = Guid.NewGuid(),
				Created = now,
				Name = _seedConfigSection.BlogName,
				Slogan = _seedConfigSection.BlogSlogan,
				Copyright = _seedConfigSection.Copyright,
				Version = version,
				OwnerId = writer.WriterId,
				Writers = new List<WriterModel> { { writer } },
			};

			_context.Blog.Add(blog);
			_context.SaveChanges();
		}
	}
}

﻿/*
DDDN.CrossBlog.Blog.Data.BusinessLayer.WriterBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Security;
using DDDN.Logging.Messages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class WriterBusinessLayer : IWriterBusinessLayer
	{
		private readonly CrossBlogContext _context;
		private readonly BlogInfo _blogInfo;

		public WriterBusinessLayer(
			CrossBlogContext context,
			BlogInfo blogInfo)
		{
			_context = context ?? throw new System.ArgumentNullException(nameof(context));
			_blogInfo = blogInfo ?? throw new ArgumentNullException(nameof(blogInfo));
		}

		public async Task<bool> Exists(Guid writerId)
		{
			return await _context.Writers.AnyAsync(e => e.WriterId.Equals(writerId));
		}

		public async Task<WriterModel> GetWithRoles(Guid writerId)
		{
			var writer = await _context.Writers
				.AsNoTracking()
				.Include(p => p.Roles)
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.WriterId.Equals(writerId));

			if (writer == default(WriterModel))
			{
				throw new WriterNotFoundException(writerId);
			}

			return writer;
		}
		public async Task<IEnumerable<WriterModel>> GetWithRoles()
		{
			return await _context.Writers
				.AsNoTracking()
				.Include(p => p.Roles)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<(AuthenticationResult authenticationResult, ClaimsPrincipal principal)>
			TryToAuthenticateAndGetPrincipal(string loginMail, string loginPassword)
		{
			if (string.IsNullOrWhiteSpace(loginMail))
			{
				throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(loginMail));
			}

			if (string.IsNullOrWhiteSpace(loginPassword))
			{
				throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(loginPassword));
			}

			var writer = await _context.Writers
				.Where(p => p.Mail.Equals(loginMail, StringComparison.InvariantCultureIgnoreCase))
				.Include(p => p.Roles).FirstOrDefaultAsync();

			if (writer == default(WriterModel))
			{
				return (AuthenticationResult.UserNotFound, null);
			}

			var hashedLogin = Crypto.HashWithSHA256(loginPassword, writer.Salt);

			if (hashedLogin.hashedPassword.SequenceEqual(writer.PasswordHash))
			{
				var principal = CreateClaimsPrincipal(writer);
				return (AuthenticationResult.Authenticated, principal);
			}
			else
			{
				return (AuthenticationResult.WrongPassword, null);
			}
		}

		private static ClaimsPrincipal CreateClaimsPrincipal(WriterModel writer)
		{
			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

			foreach (var role in writer.Roles)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role.Role.ToString()));
			}

			identity.AddClaim(new Claim(ClaimTypes.Name, writer.Name));
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, writer.WriterId.ToString()));

			var principal = new ClaimsPrincipal(identity);
			return principal;
		}

		public async Task Edit(WriterViewModel writerView)
		{
			if (writerView == null)
			{
				throw new ArgumentNullException(nameof(writerView));
			}

			var writer = await _context.Writers
				.Where(p => p.WriterId.Equals(writerView.WriterId))
				.Include(p => p.Roles)
				.FirstOrDefaultAsync();

			if (writer == default(WriterModel))
			{
				throw new WriterNotFoundException(writerView.WriterId);
			}
			else
			{
				writer.Name = writerView.Name;
				writer.Mail = writerView.Mail;
				writer.State = writerView.State;
				writer.AboutMe = writerView.AboutMe;
				writer.AboutMe = writerView.AboutMe;
				HandleAdministratorRole(writerView.Administrator, writer);
				await _context.SaveChangesAsync();
			}
		}

		private static void HandleAdministratorRole(bool isAdministrator, WriterModel writer)
		{
			var adminRole = writer.Roles
								.Where(p => p.Role.Equals(RoleModel.Roles.Administrator))
								.FirstOrDefault();

			if (isAdministrator && adminRole == default(RoleModel))
			{
				writer.Roles.Add(RoleModel.Get(RoleModel.Roles.Administrator));
			}
			else if (!isAdministrator && adminRole != default(RoleModel))
			{
				writer.Roles.Remove(adminRole);
			}
		}

		public async Task Create(WriterViewModel writerView)
		{
			var hashed = Crypto.HashWithSHA256(writerView.Password);

			var writer = new WriterModel
			{
				Name = writerView.Name,
				Mail = writerView.Mail,
				AboutMe = writerView.AboutMe,
				WriterId = Guid.NewGuid(),
				PasswordHash = hashed.hashedPassword,
				Salt = hashed.salt,
				State = WriterModel.States.Active,
				Created = DateTimeOffset.Now,
				EmailConfirmed = false,
				Roles = new List<RoleModel> { { RoleModel.Get(RoleModel.Roles.Writer) } }
			};

			HandleAdministratorRole(writerView.Administrator, writer);
			_context.Add(writer);
			await _context.SaveChangesAsync();
		}

		public async Task Delete(Guid writerId)
		{
			if (writerId.Equals(_blogInfo.OwnerId))
			{
				throw new CrossBlogException("Owner of the Blog cannot be deleted.");
			}

			var writer = await _context.Writers.FirstOrDefaultAsync(p => p.WriterId.Equals(writerId));

			_context.Writers.Remove(writer);
			await _context.SaveChangesAsync();
		}

		public async Task<AuthenticationResult> PasswordChange(PasswordViewModel writerPassword)
		{
			if (writerPassword == null)
			{
				throw new ArgumentNullException(nameof(writerPassword));
			}

			if (!writerPassword.New.Equals(writerPassword.Compare, StringComparison.InvariantCultureIgnoreCase))
			{
				return AuthenticationResult.PasswordNotMatch;
			}

			var writer = await _context.Writers
				.Where(p => p.WriterId.Equals(writerPassword.WriterId))
				.FirstOrDefaultAsync();

			if (writer == default(WriterModel))
			{
				return AuthenticationResult.UserNotFound;
			}

			var hashedOld = Crypto.HashWithSHA256(writerPassword.Old, writer.Salt);

			if (!hashedOld.hashedPassword.SequenceEqual(writer.PasswordHash))
			{
				return AuthenticationResult.WrongPassword;
			}

			var hashedNew = Crypto.HashWithSHA256(writerPassword.New);
			writer.PasswordHash = hashedNew.hashedPassword;
			writer.Salt = hashedNew.salt;
			await _context.SaveChangesAsync();
			return AuthenticationResult.PasswordChanged;
		}
	}
}

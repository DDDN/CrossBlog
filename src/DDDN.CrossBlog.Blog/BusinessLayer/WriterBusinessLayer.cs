/*
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
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Security;
using DDDN.Logging.Messages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public class WriterBusinessLayer : IWriterBusinessLayer
	{
		private CrossBlogContext _context;

		public WriterBusinessLayer(CrossBlogContext context)
		{
			_context = context ?? throw new System.ArgumentNullException(nameof(context));
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
				.Where(p => p.Mail.Equals(loginMail, StringComparison.CurrentCultureIgnoreCase))
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

			identity.AddClaim(new Claim(ClaimTypes.Name, writer.WriterId.ToString()));
			var principal = new ClaimsPrincipal(identity);
			return principal;
		}
	}
}

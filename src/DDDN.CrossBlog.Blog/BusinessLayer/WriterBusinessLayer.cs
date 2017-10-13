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

using DDDN.CrossBlog.Blog.Configuration;
using DDDN.CrossBlog.Blog.Data;
using DDDN.CrossBlog.Blog.Exceptions;
using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Security;
using DDDN.CrossBlog.Blog.Views.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
		private readonly AuthenticationConfigSection _configSection;
		private readonly IStringLocalizer<WriterBusinessLayer> _loc;
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context">Database context.</param>
		/// <param name="blogInfo">Singelon with general blog infos.</param>
		/// <param name="authenticationConfigSection"></param>
		/// <param name="localizer"></param>
		/// <param name="stringLocalizer"></param>
		public WriterBusinessLayer(
			CrossBlogContext context,
			BlogInfo blogInfo,
			IOptions<AuthenticationConfigSection> authenticationConfigSection,
			IStringLocalizer<WriterBusinessLayer> localizer)
		{
			_context = context ?? throw new System.ArgumentNullException(nameof(context));
			_blogInfo = blogInfo ?? throw new ArgumentNullException(nameof(blogInfo));

			if (authenticationConfigSection == null)
			{
				throw new ArgumentNullException(nameof(authenticationConfigSection));
			}
			_configSection = authenticationConfigSection.Value ?? throw new NullReferenceException("authenticationConfigSection.Value");

			_loc = localizer ?? throw new ArgumentNullException(nameof(localizer));
		}
		/// <summary>
		/// Checks if an writer exists.
		/// </summary>
		/// <param name="writerId">The writer's id.</param>
		/// <returns>Returns true if a writer with the given id exists, otherwise false.</returns>
		public async Task<bool> Exists(Guid writerId)
		{
			return await _context.Writers.AnyAsync(e => e.WriterId.Equals(writerId));
		}
		/// <summary>
		/// Returns a writer with his roles.
		/// </summary>
		/// <param name="writerId">The writer's id.</param>
		/// <returns>Writer's entity and including the roles entities.</returns>
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
		/// <summary>
		/// Returns all writers and their roles.
		/// </summary>
		/// <returns>Returns an IEnumerable with all writers and their roles.</returns>
		public async Task<IEnumerable<WriterModel>> GetWithRoles()
		{
			return await _context.Writers
				.AsNoTracking()
				.Include(p => p.Roles)
				.AsNoTracking()
				.ToListAsync();
		}
		/// <summary>
		/// Searches a writer by his mail and checks if the password salts matches.
		/// </summary>
		/// <param name="loginMail">The mail provided on user login.</param>
		/// <param name="loginPassword">The password provided on user login.</param>
		/// <returns>Returns a tuple with an <see cref="AuthenticationResult"/> value and,
		/// if successful, also with a CimesProncipal object with the writers info.</returns>
		public async Task<(AuthenticationResult authenticationResult, ClaimsPrincipal principal)>
			TryToAuthenticateAndGetPrincipal(string loginMail, string loginPassword)
		{
			if (string.IsNullOrWhiteSpace(loginMail))
			{
				throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(loginMail));
			}

			if (string.IsNullOrWhiteSpace(loginPassword))
			{
				throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(loginPassword));
			}

			var writer = await _context.Writers
				.Where(p => p.Mail.Equals(loginMail, StringComparison.InvariantCultureIgnoreCase))
				.Include(p => p.Roles).FirstOrDefaultAsync();

			if (writer == default(WriterModel))
			{
				return (AuthenticationResult.UserNotFound, null);
			}

			var (hashedPassword, salt) = Crypto.HashWithSHA256(loginPassword, writer.Salt);

			if (hashedPassword.SequenceEqual(writer.PasswordHash))
			{
				var principal = CreateClaimsPrincipal(writer);
				return (AuthenticationResult.Authenticated, principal);
			}
			else
			{
				return (AuthenticationResult.WrongPassword, null);
			}
		}
		/// <summary>
		/// Crates a <see cref="ClaimsPrincipal"/> object form <see cref="WriterModel"/> object./>
		/// </summary>
		/// <param name="writer">The writer model object.</param>
		/// <returns></returns>
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
		/// <summary>
		/// Find a wirter by his id and updates is form a writer view model.
		/// </summary>
		/// <param name="writerView">The writer view model object.</param>
		public async Task Update(WriterViewModel writerView)
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
		/// <summary>
		/// Gets the roles of an writer and and or removes the admin role depending on the isAdministrator parameter.
		/// </summary>
		/// <param name="isAdministrator">True if the writer should be administrator, otherwise false.</param>
		/// <param name="writer">The writer object.</param>
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
		/// <summary>
		/// Checks if a mail is already used by a writer.
		/// </summary>
		/// <param name="mail">The mail.</param>
		/// <returns>True if the provided mail alreadu belongs to a writer, otherwise false.</returns>
		public bool MailExist(string mail)
		{
			return _context.Writers
				.Any(p => p.Mail.Equals(mail, StringComparison.InvariantCultureIgnoreCase));
		}
		/// <summary>
		/// Creates a 
		/// </summary>
		/// <param name="writerView"></param>
		/// <returns></returns>
		public async Task Create(WriterViewModel writerView)
		{
			if (writerView == null)
			{
				throw new ArgumentNullException(nameof(writerView));
			}

			if (MailExist(writerView.Mail))
			{
				throw new CrossBlogException("Mail exists already.");
			}

			var (hashedPassword, salt) = Crypto.HashWithSHA256(writerView.Password);

			var writer = new WriterModel
			{
				Name = writerView.Name,
				Mail = writerView.Mail,
				AboutMe = writerView.AboutMe,
				WriterId = Guid.NewGuid(),
				PasswordHash = hashedPassword,
				Salt = salt,
				State = WriterModel.States.Active,
				Created = DateTimeOffset.Now,
				MailConfirmed = false,
				Roles = new List<RoleModel> { { RoleModel.Get(RoleModel.Roles.Writer) } }
			};

			HandleAdministratorRole(writerView.Administrator, writer);
			_context.Add(writer);
			await _context.SaveChangesAsync();
		}
		/// <summary>
		/// Checks if a writer determinated by his id is also the blog owner.
		/// The owner is stored in the <see cref="BlogModel"/> class.
		/// </summary>
		/// <param name="writerId">he writer's id.</param>
		/// <returns>Returns true, if a writer is the blog owner, oterwise false.</returns>
		public bool IsOwner(Guid writerId)
		{
			return writerId.Equals(_blogInfo.OwnerId);
		}
		/// <summary>
		/// Deletes a writer and all his posts and the posts related content.
		/// </summary>
		/// <param name="writerId"></param>
		/// <returns></returns>
		public async Task Delete(Guid writerId)
		{
			if (IsOwner(writerId))
			{
				throw new CrossBlogException("Owner of the Blog cannot be deleted.");
			}

			var writer = await _context.Writers
				.Include(p => p.Posts)
				.ThenInclude(p => p.Contents)
				.FirstOrDefaultAsync(p => p.WriterId.Equals(writerId));

			_context.Writers.Remove(writer);
			await _context.SaveChangesAsync();
		}
		/// <summary>
		/// Checks if the supported passwords are correct and the new one meets the complexity criteria and changes the writers password to a new one.
		/// </summary>
		/// <param name="passwordViewModel"></param>
		/// <returns></returns>
		public async Task<PasswordChangeResult> PasswordChange(PasswordViewModel passwordViewModel)
		{
			if (passwordViewModel == null)
				throw new ArgumentNullException(nameof(passwordViewModel));

			if (0 != string.Compare(passwordViewModel.New, passwordViewModel.Compare, StringComparison.InvariantCultureIgnoreCase))
			{
				return PasswordChangeResult.PasswordDoNotMatch;
			}

			var writer = await _context.Writers
			.Where(p => p.WriterId.Equals(passwordViewModel.WriterId))
			.FirstAsync();

			var (hashedPassword, salt) = Crypto.HashWithSHA256(passwordViewModel.Old, writer.Salt);

			if (!hashedPassword.SequenceEqual(writer.PasswordHash))
			{
				return PasswordChangeResult.WrongPassword;
			}

			var hashedNew = Crypto.HashWithSHA256(passwordViewModel.New);
			writer.PasswordHash = hashedNew.hashedPassword;
			writer.Salt = hashedNew.salt;
			await _context.SaveChangesAsync();

			return PasswordChangeResult.Changed;
		}

		public IList<ViewMessage> PasswordMeetsComplexity(string password)
		{
			if (password == null)
				throw new ArgumentNullException(nameof(password));

			var msgs = new List<ViewMessage>();

			var criteria = _configSection.PasswordComplexity.Split(",")
				.Select(p => int.Parse(p))
				.ToArray();

			if (criteria[0] > password.Length)
			{
				msgs.Add(new ViewMessage(String.Format(_loc["PasswordComplexityMinimalLength"], criteria[0]), ViewMessage.MsgType.Warning));
			}

			if (criteria[1] == 1 && password.All(char.IsLetterOrDigit))
			{
				msgs.Add(new ViewMessage(_loc["PasswordComplexityAlphanumeric"], ViewMessage.MsgType.Warning));
			}

			if (criteria[2] == 1 && !password.Any(char.IsUpper))
			{
				msgs.Add(new ViewMessage(_loc["PasswordComplexityUppercase"], ViewMessage.MsgType.Warning));
			}

			if (criteria[3] == 1 && !password.Any(char.IsLower))
			{
				msgs.Add(new ViewMessage(_loc["PasswordComplexityLowercase"], ViewMessage.MsgType.Warning));
			}

			if (criteria[4] == 1 && !password.Any(char.IsNumber))
			{
				msgs.Add(new ViewMessage(_loc["PasswordComplexityNumber"], ViewMessage.MsgType.Warning));
			}
			return msgs;
		}
	}
}

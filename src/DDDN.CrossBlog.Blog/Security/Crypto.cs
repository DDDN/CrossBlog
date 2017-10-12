/*
DDDN.CrossBlog.Blog.Security.Crypto
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace DDDN.CrossBlog.Blog.Security
{
	public static class Crypto
	{
		public static (byte[] hashedPassword, byte[] salt) HashWithSHA256(string password, byte[] saltForPassword = null)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(password));
			}

			byte[] salt = null;

			if (saltForPassword == null)
			{
				salt = new byte[128 / 8];
				using (var rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(salt);
				}
			}
			else
			{
				salt = saltForPassword;
			}

			var hashed = KeyDerivation.Pbkdf2(
				 password: password,
				 salt: salt,
				 prf: KeyDerivationPrf.HMACSHA256,
				 iterationCount: 10000,
				 numBytesRequested: 256 / 8);

			return (hashed, salt);
		}
	}
}

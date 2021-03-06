﻿/*
DDDN.CrossBlog.Blog.Configuration.AuthenticationConfigSection
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

namespace DDDN.CrossBlog.Blog.Configuration
{
	public class AuthenticationConfigSection
	{
		public string Issuer { get; set; } = "http://jaskiewicz.de";
		public int SessionDurationInMinutes { get; set; } = 30;
		public string PasswordComplexity { get; set; } = "8,1,1,1,1";
	}
}

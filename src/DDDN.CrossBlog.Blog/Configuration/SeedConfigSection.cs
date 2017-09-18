/*
DDDN.CrossBlog.Blog.Configuration.SeedConfigSection
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
	public class SeedConfigSection
   {
		public string Name { get; set; } = "Your Name";
      public string Mail { get; set; } = "crossblog@devdone.net";
      public string Copyright { get; set; } = "CrossBlog, Copyright (C) 2017 Lukasz Jaskiewicz";
		public int MaxBlogTeaserLength { get; set; } = 512;
		public int MaxBlogTitleLength { get; set; } = 2048;
		
	}
}

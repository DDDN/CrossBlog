/*
DDDN.CrossBlog.Blog.Views.Models.ViewMessage
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static DDDN.CrossBlog.Blog.Views.Models.ViewMessage;

namespace DDDN.CrossBlog.Blog.Views.Models
{
	public class ViewMessage : ICollection<(string msg, IsTypeOf msgType)>
	{
		public enum IsTypeOf
		{
			None,
			Info,
			Warning,
			Error
		};

		private Collection<(string msg, IsTypeOf msgType)> Messages { get; set; } = new Collection<(string msg, IsTypeOf msgType)>();

		public int Count { get { return Messages.Count; } }
		public bool IsReadOnly { get { return false; } }

		public void Add(string msg, IsTypeOf msgType)
		{
			Messages.Add((msg, msgType));
		}

		public void Add((string msg, IsTypeOf msgType) item)
		{
			Messages.Add(item);
		}

		public void Clear()
		{
			Messages.Clear();
		}

		public bool Contains((string msg, IsTypeOf msgType) item)
		{
			return Messages.Contains(item);
		}

		public void CopyTo((string msg, IsTypeOf msgType)[] array, int arrayIndex)
		{
			Messages.CopyTo(array, arrayIndex);
		}

		public IEnumerator<(string msg, IsTypeOf msgType)> GetEnumerator()
		{
			return Messages.GetEnumerator();
		}

		public bool Remove((string msg, IsTypeOf msgType) item)
		{
			return Messages.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Messages.GetEnumerator();
		}
	}
}

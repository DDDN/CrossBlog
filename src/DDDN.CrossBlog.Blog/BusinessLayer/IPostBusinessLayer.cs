/*
DDDN.CrossBlog.Blog.BusinessLayer.IPostBusinessLayer
Copyright(C) 2017 Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
- This program is free software; you can redistribute it and/or modify it under the terms of the
GNU General Public License as published by the Free Software Foundation; version 2 of the License.
- This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
- You should have received a copy of the GNU General Public License along with this program; if not, write
to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.CrossBlog.Blog.Models;
using DDDN.CrossBlog.Blog.Views.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDN.CrossBlog.Blog.BusinessLayer
{
	public interface IPostBusinessLayer
	{
		Task<IEnumerable<PostModel>> GetNewest(int skip, int take);
		Task<IEnumerable<PostModel>> GetNewest(int skip, int take, params PostModel.States[] states);
		Task<IEnumerable<PostModel>> GetNewestByCategory(int skip, int take, Guid categoryId, params PostModel.States[] states);
		Task<PostModel> GetPostOrDefault(Guid postId);
		Task<PostModel> GetWithCategories(Guid postId);
		Task<PostModel> GetPostWithCommentsOrDefault(Guid postId);
		Task Upload(IList<IFormFile> files);
		Task CommentSave(Guid postId, string personName, string commentTitle, string commentText);
		Task<(byte[] binary, string name)> GetContent(string linkName);
		Task Delete(Guid postId);
		Task Edit(PostViewModel postViewModel, IList<string> categoryIds);
	}
}
﻿using Core.Models.Blogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository.Blogs
{
    public interface IBlogCategoryRepository
    {
        Task<IReadOnlyList<BlogCategory>> ListAsync();

        Task<IReadOnlyList<BlogCategory>> ListAsync(int sourceCatId);

        Task<IReadOnlyList<BlogCategory>> ListAsync(string langId);

        Task<IReadOnlyList<BlogCategory>> ListAsync(int sourceCatId, string langId);

        Task<BlogCategory> ModelAsync(int id);

        Task<BlogCategory> ModelAsync(int sourceCatId, string langId, string name);

        Task<bool> AddAsync(BlogCategory category);

        Task<bool> UpdateAsync(BlogCategory category);

        Task<bool> RemoveAsync(BlogCategory category);

        Task<bool> SaveChangesAsync();
    }
}

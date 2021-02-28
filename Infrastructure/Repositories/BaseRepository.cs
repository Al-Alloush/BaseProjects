﻿using Core.Interfaces.Repository;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseModels
    {
        private readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync()
        {
            var models = await _context.Set<T>().ToListAsync();
            return models;
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync(int id)
        {
            var models = await _context.Set<T>().Where(x => x.Id == id).ToListAsync();
            return models;
        }

        public virtual async Task<T> ModelAsync(int id)
        {
            var model = await _context.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
            return model;
        }

        public virtual async Task<int> CountAsync(int id)
        {
            var counts = await _context.Set<T>().Where(x=>x.Id == id).CountAsync();
            return counts;
        }

        public async Task<bool> AddAsync(T model)
        {
            try
            {
                EntityEntry<T> result = await _context.Set<T>().AddAsync(model);
                if (result.State.ToString() == "Added")
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T model)
        {
            try
            {
                _context.Set<T>().Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(T model)
        {
            try
            {
                EntityEntry<T> result = _context.Remove(model);
                if (result.State.ToString() == "Deleted")
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                // if _context.SaveChangesAsync() bigger than 0 (if success equel 1)  return true (Success Save changes)
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        
    }
}

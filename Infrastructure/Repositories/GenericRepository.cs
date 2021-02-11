﻿using Core.Interfaces.Repository;
using Core.Specifications;
using Infrastructure.Data;
using Infrastructure.SpecEvaluators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<T> ModelDetailsAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

        public async Task<bool> AddAsync(T model)
        {
            EntityEntry<T> result = await _context.Set<T>().AddAsync(model);
            if (result.State.ToString() == "Added")
                return true;
            else
                return false;

        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(T model)
        {
            EntityEntry<T> result =  _context.Remove(model) ;
            if (result.State.ToString() == "added")
                return true;
            else
                return false;
        }
    }
}

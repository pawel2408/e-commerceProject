using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;
        public GenericRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        
        public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
        // we call this, we pass our "spec" we know what it contains: "where clouse (x => x.Id == id) (what we want to get with id)", as well as include expressions "AddInclude(x => x.ProductType); AddInclude(x => x.ProductBrand);" all from  ProductsWithTypesAndBrandsSpecification.cs
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
            // and we apply this specification here, look below ApplySpecification declaration ;)
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
            // We hit GetQuery method in the SpecificationEvaluator and the job of this list is to return IQueryable<T> and we pasisng DBSet "_context.Set<T>()" which is going to be the product DB set and we olso passing the specyfication "spec" to our  SpecificationEvaluator.cs, go there!

            // So when we return from SpecificationEvaluator.cs we have As.Quueryable() with this expressions that we can pass to our database

            // FirstOrDefaultAsync(); this is a place where we actually execute the queries and return the data from the database. Now we go to our ProductController.cs go there!
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
    }
}
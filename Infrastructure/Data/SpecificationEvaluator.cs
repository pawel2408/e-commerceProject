using System.Linq;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    // there is no differenc between <TEntity> and <T> is just a name for better understanding what type we expecting there.
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        // We passing IQueryable<TEntity which is DBSet of type product
        {
            var query = inputQuery;

            if(spec.Criteria != null)
            // we check if criteria is not equal to null
            {
                query = query.Where(spec.Criteria);
                // where id is equal to the product id
                // get me a product where the product is whatever we've specified as this criteria.
                // this could be written as (p => p.ProductTypeId == id) but we replace it with spec.Criteria.
            }

            if(spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            
            if(spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if(spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            // this method is aggregating our "include expressions" which contain product type and the product brand, ant them we are retun the query, which mean we go back to our GenericRepository go there!

            return query;
        }
    }
}
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.RepositoryInterfaces;
using E_Commerce.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace E_Commerce.Infrastructure.Data.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : SharedBase
	{
		#region Fields

		private E_CommerceContext _e_CommerceContext;
		private DbSet<T> _dbSet;
		#endregion

		#region Constructor

		public GenericRepository(E_CommerceContext e_CommerceContext)
		{
			_e_CommerceContext = e_CommerceContext;
			_dbSet = e_CommerceContext.Set<T>();
		}

		#endregion

		#region ICommandRepository
		public void Delete(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			_dbSet.Remove(entity);
		}

		public async Task<T> GetById(int id)
		{
			return await _dbSet.FindAsync(id);

		}

		public void Insert(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			_dbSet.Add(entity);
		}

		public void AddRange(List<T> entities)
		{
			if (entities == null)
			{
				throw new ArgumentNullException("entities");
			}
			_dbSet.AddRange(entities);

		}

        public void UpdateRange(List<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            _dbSet.UpdateRange(entities);

        }

        public void RemoveRange(List<T> entities)
		{
			if (entities == null)
			{
				throw new ArgumentNullException("entities");
			}
			_dbSet.RemoveRange(entities);
		}

		public void Update(T entity)
		{

			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			// _authenticationContext.Entry<T>(entity).State = EntityState.Modified;
			_dbSet.Update(entity);
		}

		public IQueryable<T> GetByExpression(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includedEntities = null)
		{
			IQueryable<T> query = _dbSet;
			if (expression != null)
				query = _dbSet.Where(expression);

			if (includedEntities != null)
				foreach (var includedEntity in includedEntities)
				{
					query = query.Include(includedEntity);
				}

			if (orderBy != null)
				query = query = orderBy(query);

			return query;
		}

        public IQueryable<T> GetByExpressionAsNoTracking(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includedEntities = null)
        {
            IQueryable<T> query = _dbSet;
            if (expression != null)
                query = _dbSet.Where(expression);

            if (includedEntities != null)
                foreach (var includedEntity in includedEntities)
                {
                    if (includedEntity != null)
                    {
                        query = query.Include(includedEntity);
                    }
                }

            if (orderBy != null)
                query = query = orderBy(query);

            return query.AsNoTracking();
        }


        public async Task<List<T>> ToListAsync(IQueryable<T> entities)
		{
			try
			{
				return await entities.ToListAsync();
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	
		public async Task<T> FirstOrDefaultAsync(IQueryable<T> entities)
		{
			try
			{
				return await entities.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public IQueryable<T> Include(IQueryable<T> queryable, Expression<Func<T, object>> includedEntity)
		{
			return queryable.Include(includedEntity);
		}

		public void Include(ref IQueryable<T> queryable, Expression<Func<T, object>> includedEntity)
		{
			queryable = queryable.Include(includedEntity);
		}
	
		public void IncludeThenInclude<TThenInclude>(ref IQueryable<T> queryable, Expression<Func<T, TThenInclude>> includedEntity, Expression<Func<TThenInclude, object>> thenIncludedEntity)
		{
			queryable = queryable.Include(includedEntity).ThenInclude(thenIncludedEntity);
		}
		#endregion
	}
}

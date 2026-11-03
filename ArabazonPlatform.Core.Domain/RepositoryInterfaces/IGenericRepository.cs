using ArabazonPlatform.Domain.Domains;
using System.Linq.Expressions;

namespace ArabazonPlatform.Domain.RepositoryInterfaces
{
	public interface IGenericRepository<T> where T : SharedBase
	{
		Task<T> GetById(int id);
		void Insert(T entity);
		void Update(T entity);
		void Delete(T entity);
		IQueryable<T> GetByExpression(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includedEntities = null);
        IQueryable<T> GetByExpressionAsNoTracking(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includedEntities = null);

        Task<List<T>> ToListAsync(IQueryable<T> queryable);
		Task<T> FirstOrDefaultAsync(IQueryable<T> queryable);
		IQueryable<T> Include(IQueryable<T> queryable, Expression<Func<T, object>> includedEntity);
		void Include(ref IQueryable<T> queryable, Expression<Func<T, object>> includedEntity);
		void AddRange(List<T> entities);
        void UpdateRange(List<T> entities);
        void RemoveRange(List<T> entities);
	}
}

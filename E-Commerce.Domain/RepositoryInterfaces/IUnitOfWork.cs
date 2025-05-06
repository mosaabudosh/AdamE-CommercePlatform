

using E_Commerce.Domain.Domains;

namespace E_Commerce.Domain.RepositoryInterfaces
{
	public interface IUnitOfWork
	{
		IGenericRepository<T> GetRepository<T>() where T : SharedBase;
		Task CommitChanges();
	}
}

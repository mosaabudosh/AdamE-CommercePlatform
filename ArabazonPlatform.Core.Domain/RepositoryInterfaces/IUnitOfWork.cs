

using ArabazonPlatform.Domain.Domains;

namespace ArabazonPlatform.Domain.RepositoryInterfaces
{
	public interface IUnitOfWork
	{
		IGenericRepository<T> GetRepository<T>() where T : SharedBase;
		Task CommitChanges();
	}
}

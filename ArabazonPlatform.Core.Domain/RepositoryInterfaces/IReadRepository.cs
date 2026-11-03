

using ArabazonPlatform.Domain.Domains;

namespace ArabazonPlatform.Domain.RepositoryInterfaces
{
	public interface IReadRepository
	{
		Task<List<T>> GetByView<T>(string viewName) where T : SharedBase;
		Task<List<T>> GetByStoredProcedure<T>(string spName, Dictionary<string, object> parameters) where T : SharedBase;
	}
}

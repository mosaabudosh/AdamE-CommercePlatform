

using E_Commerce.Domain.Domains;
using E_Commerce.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.Infrastructure.Data.Repository
{
	public class ReadRepository : IReadRepository
	{
		#region Fields

		//private DatabaseEngine _databaseEngine;
		string _connectionString;

		#endregion

		#region Constructor

		public ReadRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		#endregion

		#region IReadRepository      
		public async Task<List<T>> GetByStoredProcedure<T>(string spName, Dictionary<string, object> parameters) where T : SharedBase
		{
			//using (_databaseEngine = new DatabaseEngine(_connectionString))
			//{
			//	DapperParameters dapperParameters = null;
			//	if (parameters != null && parameters.Count > 0)
			//	{
			//		dapperParameters = new DapperParameters();
			//		foreach (var parameter in parameters)
			//		{
			//			dapperParameters.AddParameter(parameter.Key, parameter.Value);
			//		}

			//	}

			//	IEnumerable<T> result = await _databaseEngine.ExecuteDapperSpAsync<T>(spName, dapperParameters: dapperParameters ?? null);

			//	return result.ToList();
			//}
			return new List<T>(); 
		}

		public async Task<List<T>> GetByView<T>(string viewName) where T : SharedBase
		{
			//StringBuilder queryBuilder = new StringBuilder();
			//queryBuilder.Append("Select * From ");
			//queryBuilder.Append(typeof(T).FullName);

			//IEnumerable<T> result = await _databaseEngine.ExecuteDapperQueryAsync<T>(viewName);

			//return result.ToList();

			return new List<T>();
		}

		#endregion
	}
}

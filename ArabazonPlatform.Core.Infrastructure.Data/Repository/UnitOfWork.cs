using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Infrastructure.Data.Context;

namespace ArabazonPlatform.Infrastructure.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        #region Fields
        ArabazonPlatformContext _arabazonPlatformContext;
        Dictionary<Type, object> _repositoryDictionary;
        #endregion

        #region Constructor
        public UnitOfWork(ArabazonPlatformContext arabazonPlatformContext)
        {
            _arabazonPlatformContext = arabazonPlatformContext;
            _repositoryDictionary = new Dictionary<Type, object>();
        }

        #endregion

        #region Methods

        public IGenericRepository<T> GetRepository<T>() where T : SharedBase
        {
            GenericRepository<T> genericRepository;

            genericRepository = GetRepositoryIfExist<T>();

            if (genericRepository == null)
            {
                genericRepository = new GenericRepository<T>(_arabazonPlatformContext);
                AddRepositoryToDictionary(genericRepository);
            }

            return genericRepository;
        }

        private GenericRepository<T> GetRepositoryIfExist<T>() where T : SharedBase
        {
            if (_repositoryDictionary.ContainsKey(typeof(T)))
                return (GenericRepository<T>)_repositoryDictionary[typeof(T)];
            else
                return null;
        }

        private void AddRepositoryToDictionary<T>(GenericRepository<T> genericRepository) where T : SharedBase
        {
            _repositoryDictionary.Add(typeof(T), genericRepository);
        }

        #endregion

        #region IUnitOfWork

        public async Task CommitChanges()
        {
            await _arabazonPlatformContext.SaveChangesAsync();
        } 
        #endregion
    }
}

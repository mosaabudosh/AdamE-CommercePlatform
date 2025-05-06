using E_Commerce.Domain.Domains;
using E_Commerce.Domain.RepositoryInterfaces;
using E_Commerce.Infrastructure.Data.Context;

namespace E_Commerce.Infrastructure.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        #region Fields
        E_CommerceContext _e_CommerceContext;
        Dictionary<Type, object> _repositoryDictionary;
        #endregion

        #region Constructor
        public UnitOfWork(E_CommerceContext e_CommerceContext)
        {
            _e_CommerceContext = e_CommerceContext;
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
                genericRepository = new GenericRepository<T>(_e_CommerceContext);
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
            await _e_CommerceContext.SaveChangesAsync();
        }

        #endregion
    }
}

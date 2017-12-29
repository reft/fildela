using Fildela.Data.Database.DataLayer;
using Fildela.Data.Storage.Services;

namespace Fildela.Data.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly DataLayer DB;
        protected readonly CloudStorageServices Storage;

        protected RepositoryBase(DataLayer db, CloudStorageServices storage)
        {
            DB = db;
            Storage = storage;
        }
    }
}

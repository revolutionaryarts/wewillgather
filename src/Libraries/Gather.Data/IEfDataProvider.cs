using Gather.Core.Data;

namespace Gather.Data
{
    public interface IEfDataProvider : IDataProvider
    {
        void InitConnectionFactory();

        void SetDatabaseInitializer();
    }
}
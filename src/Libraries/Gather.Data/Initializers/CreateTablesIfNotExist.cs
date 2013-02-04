using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;

namespace Gather.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {

        private readonly string[] _customCommands;

        public CreateTablesIfNotExist(string[] customCommands)
        {
            _customCommands = customCommands;
        }

        public void InitializeDatabase(TContext context)
        {
            bool databaseExists;

            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                databaseExists = context.Database.Exists();
            }

            if (databaseExists)
            {
                // Check to see if tables are already created
                int numberOfTables = 0;
                foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                    numberOfTables = t1;

                if (numberOfTables == 0)
                {
                    // Create all database tables
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);
                    context.SaveChanges();

                    if (_customCommands != null && _customCommands.Length > 0)
                    {
                        foreach (var command in _customCommands)
                            context.Database.ExecuteSqlCommand(command);
                    }
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }

    }
}
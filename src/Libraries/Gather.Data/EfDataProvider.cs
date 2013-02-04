using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Gather.Data.Initializers;

namespace Gather.Data
{
    public class EfDataProvider : IEfDataProvider
    {
        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = new SqlConnectionFactory();
        }

        public void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        public void SetDatabaseInitializer()
        {
            var customCommands = new List<string>();
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<GatherObjectContext>(customCommands.ToArray());
            Database.SetInitializer(initializer);
            //Database.SetInitializer(new CreateDatabaseIfNotExists<GatherObjectContext>());
            //Database.SetInitializer(new CreateDatabaseIfNotExistsInitializer());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<GatherObjectContext, DbMigrationsConfiguration<GatherObjectContext>>());
        }

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                return new string[0];
            }

            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            while (true)
            {
                string lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
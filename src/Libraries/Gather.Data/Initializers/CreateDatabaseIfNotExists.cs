using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Gather.Data.Initializers
{
    public class CreateDatabaseIfNotExistsInitializer : CreateDatabaseIfNotExists<GatherObjectContext>
    {
        protected override void Seed(GatherObjectContext context)
        {
            base.Seed(context);

            var customCommands = new List<string>();
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.StoredProcedures.sql"), false));

            if (customCommands.Count > 0)
            {
                foreach (var command in customCommands)
                    context.Database.ExecuteSqlCommand(command);
            }
        }

        private IEnumerable<string> ParseCommands(string filePath, bool throwExceptionIfNonExists)
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

        private string ReadNextStatementFromStream(StreamReader reader)
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
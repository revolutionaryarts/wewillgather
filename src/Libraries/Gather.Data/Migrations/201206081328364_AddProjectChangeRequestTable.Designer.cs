// <auto-generated />
namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class AddProjectChangeRequestTable : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201206081328364_AddProjectChangeRequestTable"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}
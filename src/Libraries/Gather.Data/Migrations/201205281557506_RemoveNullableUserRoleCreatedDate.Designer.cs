// <auto-generated />
namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class RemoveNullableUserRoleCreatedDate : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201205281557506_RemoveNullableUserRoleCreatedDate"; }
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
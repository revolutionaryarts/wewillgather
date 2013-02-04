namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMediaFileTypeField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Media", "FileTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Media", "FileTypeId");
        }
    }
}

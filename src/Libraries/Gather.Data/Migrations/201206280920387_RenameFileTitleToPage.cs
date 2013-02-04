namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RenameFileTitleToPage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "FileTitle", c => c.String(maxLength: 100));
            DropColumn("Page", "PdfTitle");
        }
        
        public override void Down()
        {
            AddColumn("Page", "PdfTitle", c => c.String(maxLength: 100));
            DropColumn("Page", "FileTitle");
        }
    }
}

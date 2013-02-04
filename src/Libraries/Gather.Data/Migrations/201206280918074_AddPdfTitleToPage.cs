namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPdfTitleToPage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "PdfTitle", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("Page", "PdfTitle");
        }
    }
}

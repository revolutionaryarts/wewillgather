namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddLocationSeoNameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Location", "SeoName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Location", "SeoName");
        }
    }
}

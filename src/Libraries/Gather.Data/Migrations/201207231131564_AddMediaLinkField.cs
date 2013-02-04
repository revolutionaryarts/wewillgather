namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMediaLinkField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Media", "Link", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Media", "Link");
        }
    }
}

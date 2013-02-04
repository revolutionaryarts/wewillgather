namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSiteOwner : DbMigration
    {
        public override void Up()
        {
            AddColumn("SiteOwner", "TwitterAccessToken", c => c.String());
            AddColumn("SiteOwner", "TwitterAccessTokenSecret", c => c.String());
            DropColumn("SiteOwner", "TwitterUsername");
            DropColumn("SiteOwner", "TwitterPassword");
        }
        
        public override void Down()
        {
            AddColumn("SiteOwner", "TwitterPassword", c => c.String());
            AddColumn("SiteOwner", "TwitterUsername", c => c.String());
            DropColumn("SiteOwner", "TwitterAccessTokenSecret");
            DropColumn("SiteOwner", "TwitterAccessToken");
        }
    }
}

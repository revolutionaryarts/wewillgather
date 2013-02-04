namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSiteOwnerTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner");
            DropIndex("MessageQueue", new[] { "SiteOwner_Id" });
            DropColumn("MessageQueue", "SiteOwner_Id");
            DropTable("SiteOwner");
        }
        
        public override void Down()
        {
            CreateTable(
                "SiteOwner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                        Host = c.String(nullable: false, maxLength: 255),
                        Port = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        EnableSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        TwitterAccessToken = c.String(),
                        TwitterAccessTokenSecret = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("MessageQueue", "SiteOwner_Id", c => c.Int());
            CreateIndex("MessageQueue", "SiteOwner_Id");
            AddForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner", "Id");
        }
    }
}

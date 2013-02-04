namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectOwner : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "EmailAccountId", "EmailAccount");
            DropIndex("MessageQueue", new[] { "EmailAccountId" });
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
                        TwitterUsername = c.String(),
                        TwitterPassword = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("MessageQueue", "SiteOwner_Id", c => c.Int());
            AddForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner", "Id");
            CreateIndex("MessageQueue", "SiteOwner_Id");
            DropTable("EmailAccount");
        }
        
        public override void Down()
        {
            CreateTable(
                "EmailAccount",
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
                    })
                .PrimaryKey(t => t.Id);
            
            DropIndex("MessageQueue", new[] { "SiteOwner_Id" });
            DropForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner");
            DropColumn("MessageQueue", "SiteOwner_Id");
            DropTable("SiteOwner");
            CreateIndex("MessageQueue", "EmailAccountId");
            AddForeignKey("MessageQueue", "EmailAccountId", "EmailAccount", "Id");
        }
    }
}

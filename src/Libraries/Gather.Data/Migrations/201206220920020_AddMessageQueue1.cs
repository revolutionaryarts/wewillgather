namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageQueue1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "MessageQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Priority = c.Int(nullable: false),
                        Subject = c.String(),
                        ShortBody = c.String(maxLength: 140),
                        Body = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        SentTries = c.Int(nullable: false),
                        SentOn = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                        User_Id = c.Int(),
                        SiteOwner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User", t => t.User_Id)
                .ForeignKey("SiteOwner", t => t.SiteOwner_Id)
                .Index(t => t.User_Id)
                .Index(t => t.SiteOwner_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("MessageQueue", new[] { "SiteOwner_Id" });
            DropIndex("MessageQueue", new[] { "User_Id" });
            DropForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner");
            DropForeignKey("MessageQueue", "User_Id", "User");
            DropTable("MessageQueue");
        }
    }
}

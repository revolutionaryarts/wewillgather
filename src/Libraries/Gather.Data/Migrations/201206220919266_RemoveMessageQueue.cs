namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMessageQueue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "User_Id", "User");
            DropForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner");
            DropIndex("MessageQueue", new[] { "User_Id" });
            DropIndex("MessageQueue", new[] { "SiteOwner_Id" });
            DropTable("MessageQueue");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("MessageQueue", "SiteOwner_Id");
            CreateIndex("MessageQueue", "User_Id");
            AddForeignKey("MessageQueue", "SiteOwner_Id", "SiteOwner", "Id");
            AddForeignKey("MessageQueue", "User_Id", "User", "Id");
        }
    }
}

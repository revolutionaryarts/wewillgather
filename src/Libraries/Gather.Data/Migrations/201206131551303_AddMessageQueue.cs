namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageQueue : DbMigration
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
                        Body = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        SentTries = c.Int(nullable: false),
                        SentOn = c.DateTime(),
                        To_Id = c.Int(),
                        From_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User", t => t.To_Id)
                .ForeignKey("User", t => t.From_Id)
                .Index(t => t.To_Id)
                .Index(t => t.From_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("MessageQueue", new[] { "From_Id" });
            DropIndex("MessageQueue", new[] { "To_Id" });
            DropForeignKey("MessageQueue", "From_Id", "User");
            DropForeignKey("MessageQueue", "To_Id", "User");
            DropTable("MessageQueue");
        }
    }
}

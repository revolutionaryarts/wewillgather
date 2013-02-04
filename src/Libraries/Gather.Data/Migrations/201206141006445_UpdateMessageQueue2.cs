namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueue2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "ToUser_Id", "User");
            DropIndex("MessageQueue", new[] { "ToUser_Id" });
            AddColumn("MessageQueue", "UserId", c => c.Int(nullable: false));
            AddForeignKey("MessageQueue", "UserId", "User", "Id", cascadeDelete: true);
            CreateIndex("MessageQueue", "UserId");
            DropColumn("MessageQueue", "ToUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("MessageQueue", "ToUser_Id", c => c.Int());
            DropIndex("MessageQueue", new[] { "UserId" });
            DropForeignKey("MessageQueue", "UserId", "User");
            DropColumn("MessageQueue", "UserId");
            CreateIndex("MessageQueue", "ToUser_Id");
            AddForeignKey("MessageQueue", "ToUser_Id", "User", "Id");
        }
    }
}

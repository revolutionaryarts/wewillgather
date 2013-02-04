namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TryToFixMessageQueue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "User_Id", "User");
            DropIndex("MessageQueue", new[] { "User_Id" });
            AddForeignKey("MessageQueue", "User_Id", "User", "Id", cascadeDelete: true);
            CreateIndex("MessageQueue", "User_Id");
        }
        
        public override void Down()
        {
            DropIndex("MessageQueue", new[] { "User_Id" });
            DropForeignKey("MessageQueue", "User_Id", "User");
            CreateIndex("MessageQueue", "User_Id");
            AddForeignKey("MessageQueue", "User_Id", "User", "Id");
        }
    }
}

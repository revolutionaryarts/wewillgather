namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "To_Id", "User");
            DropForeignKey("MessageQueue", "From_Id", "User");
            DropIndex("MessageQueue", new[] { "To_Id" });
            DropIndex("MessageQueue", new[] { "From_Id" });
            AddColumn("MessageQueue", "ToUser_Id", c => c.Int());
            AddColumn("MessageQueue", "FromUser_Id", c => c.Int());
            AddForeignKey("MessageQueue", "ToUser_Id", "User", "Id");
            AddForeignKey("MessageQueue", "FromUser_Id", "User", "Id");
            CreateIndex("MessageQueue", "ToUser_Id");
            CreateIndex("MessageQueue", "FromUser_Id");
            DropColumn("MessageQueue", "To_Id");
            DropColumn("MessageQueue", "From_Id");
        }
        
        public override void Down()
        {
            AddColumn("MessageQueue", "From_Id", c => c.Int());
            AddColumn("MessageQueue", "To_Id", c => c.Int());
            DropIndex("MessageQueue", new[] { "FromUser_Id" });
            DropIndex("MessageQueue", new[] { "ToUser_Id" });
            DropForeignKey("MessageQueue", "FromUser_Id", "User");
            DropForeignKey("MessageQueue", "ToUser_Id", "User");
            DropColumn("MessageQueue", "FromUser_Id");
            DropColumn("MessageQueue", "ToUser_Id");
            CreateIndex("MessageQueue", "From_Id");
            CreateIndex("MessageQueue", "To_Id");
            AddForeignKey("MessageQueue", "From_Id", "User", "Id");
            AddForeignKey("MessageQueue", "To_Id", "User", "Id");
        }
    }
}

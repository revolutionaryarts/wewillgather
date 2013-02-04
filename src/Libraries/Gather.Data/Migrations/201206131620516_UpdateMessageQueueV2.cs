namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueueV2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "FromUser_Id", "User");
            DropIndex("MessageQueue", new[] { "FromUser_Id" });
            DropColumn("MessageQueue", "FromUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("MessageQueue", "FromUser_Id", c => c.Int());
            CreateIndex("MessageQueue", "FromUser_Id");
            AddForeignKey("MessageQueue", "FromUser_Id", "User", "Id");
        }
    }
}

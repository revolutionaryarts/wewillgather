namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueue1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("MessageQueue", "EmailAccountId", "EmailAccount");
            DropIndex("MessageQueue", new[] { "EmailAccountId" });
            AlterColumn("MessageQueue", "EmailAccountId", c => c.Int());
            AddForeignKey("MessageQueue", "EmailAccountId", "EmailAccount", "Id");
            CreateIndex("MessageQueue", "EmailAccountId");
        }
        
        public override void Down()
        {
            DropIndex("MessageQueue", new[] { "EmailAccountId" });
            DropForeignKey("MessageQueue", "EmailAccountId", "EmailAccount");
            AlterColumn("MessageQueue", "EmailAccountId", c => c.Int(nullable: false));
            CreateIndex("MessageQueue", "EmailAccountId");
            AddForeignKey("MessageQueue", "EmailAccountId", "EmailAccount", "Id", cascadeDelete: true);
        }
    }
}

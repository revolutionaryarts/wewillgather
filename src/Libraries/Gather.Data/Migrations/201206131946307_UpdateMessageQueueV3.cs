namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueueV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("MessageQueue", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("MessageQueue", "Deleted");
        }
    }
}

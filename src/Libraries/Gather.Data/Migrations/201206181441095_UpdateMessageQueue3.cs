namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueue3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("MessageQueue", "ShortBody", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("MessageQueue", "ShortBody");
        }
    }
}

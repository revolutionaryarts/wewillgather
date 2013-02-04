namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueue4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("MessageQueue", "ShortBody", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            AlterColumn("MessageQueue", "ShortBody", c => c.String());
        }
    }
}

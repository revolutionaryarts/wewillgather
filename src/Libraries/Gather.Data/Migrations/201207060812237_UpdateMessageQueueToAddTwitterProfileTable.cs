namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueueToAddTwitterProfileTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("MessageQueue", "TwitterProfile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("MessageQueue", "TwitterProfile");
        }
    }
}

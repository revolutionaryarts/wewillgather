namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMessageQueueToAddTwitterNameTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("MessageQueue", "TwitterUsername", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("MessageQueue", "TwitterUsername");
        }
    }
}

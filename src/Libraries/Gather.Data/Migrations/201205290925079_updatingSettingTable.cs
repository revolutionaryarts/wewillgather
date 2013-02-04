namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class updatingSettingTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("Setting", "LastModifiedBy", c => c.Int());
            AddColumn("Setting", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Setting", "LastModifiedDate");
            DropColumn("Setting", "LastModifiedBy");
        }
    }
}

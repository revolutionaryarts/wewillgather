namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProjectAddressName : DbMigration
    {
        public override void Up()
        {
            DropColumn("Project", "AddressName");
        }
        
        public override void Down()
        {
            AddColumn("Project", "AddressName", c => c.String());
        }
    }
}

namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectAddressNameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "AddressName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Project", "AddressName");
        }
    }
}

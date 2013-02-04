namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserContactUsFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "ContactUsBio", c => c.String());
            AddColumn("User", "ShowOnContactUs", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("User", "ShowOnContactUs");
            DropColumn("User", "ContactUsBio");
        }
    }
}

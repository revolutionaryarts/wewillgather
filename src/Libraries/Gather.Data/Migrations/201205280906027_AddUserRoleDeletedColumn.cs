namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRoleDeletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("UserRole", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("UserRole", "Deleted");
        }
    }
}

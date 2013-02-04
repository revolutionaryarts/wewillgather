namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNullableUserRoleCreatedDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("UserRole", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("UserRole", "CreatedDate", c => c.DateTime());
        }
    }
}

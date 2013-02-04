namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRoleCreatedByFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("UserRole", "CreatedBy", c => c.Int());
            AddColumn("UserRole", "CreatedDate", c => c.DateTime());
            AddColumn("UserRole", "LastModifiedBy", c => c.Int());
            AddColumn("UserRole", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("UserRole", "LastModifiedDate");
            DropColumn("UserRole", "LastModifiedBy");
            DropColumn("UserRole", "CreatedDate");
            DropColumn("UserRole", "CreatedBy");
        }
    }
}

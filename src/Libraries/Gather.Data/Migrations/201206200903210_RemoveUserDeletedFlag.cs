namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserDeletedFlag : DbMigration
    {
        public override void Up()
        {
            DropColumn("User", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("User", "Deleted", c => c.Boolean(nullable: false));
        }
    }
}

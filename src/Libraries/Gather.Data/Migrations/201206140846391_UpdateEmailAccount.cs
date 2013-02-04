namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEmailAccount : DbMigration
    {
        public override void Up()
        {
            DropColumn("EmailAccount", "Active");
            DropColumn("EmailAccount", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("EmailAccount", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("EmailAccount", "Active", c => c.Boolean(nullable: false));
        }
    }
}

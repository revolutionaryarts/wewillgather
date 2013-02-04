namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToMedia : DbMigration
    {
        public override void Up()
        {
            AddColumn("Media", "Name", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("Media", "Name");
        }
    }
}

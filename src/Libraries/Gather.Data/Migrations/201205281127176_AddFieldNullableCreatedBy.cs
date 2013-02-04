namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldNullableCreatedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "CreatedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("Page", "CreatedBy");
        }
    }
}

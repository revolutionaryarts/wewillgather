namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class NullableColumnLocation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Location", "LastModifiedBy", c => c.Int());
            AlterColumn("Location", "LastModifiedDate", c => c.DateTime());
            AlterColumn("Postcode", "LastModifiedBy", c => c.Int());
            AlterColumn("Postcode", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("Postcode", "LastModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("Postcode", "LastModifiedBy", c => c.Int(nullable: false));
            AlterColumn("Location", "LastModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("Location", "LastModifiedBy", c => c.Int(nullable: false));
        }
    }
}

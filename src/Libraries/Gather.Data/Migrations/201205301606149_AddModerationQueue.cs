namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddModerationQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ModerationQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModeratedBy = c.Int(nullable: false),
                        ModeratedDate = c.DateTime(nullable: false),
                        RequestTypeId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        Notes = c.String(),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("ModerationQueue");
        }
    }
}

namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogLevelId = c.Int(nullable: false),
                        ShortMessage = c.String(nullable: false),
                        FullMessage = c.String(),
                        IpAddress = c.String(maxLength: 200),
                        UserId = c.Int(),
                        PageUrl = c.String(),
                        ReferrerUrl = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("Log", new[] { "UserId" });
            DropForeignKey("Log", "UserId", "User");
            DropTable("Log");
        }
    }
}

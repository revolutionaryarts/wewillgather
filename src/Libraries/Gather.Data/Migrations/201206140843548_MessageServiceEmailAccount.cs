namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MessageServiceEmailAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EmailAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                        Host = c.String(nullable: false, maxLength: 255),
                        Port = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        EnableSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("MessageQueue", "EmailAccountId", c => c.Int(nullable: false));
            AddForeignKey("MessageQueue", "EmailAccountId", "EmailAccount", "Id", cascadeDelete: true);
            CreateIndex("MessageQueue", "EmailAccountId");
        }
        
        public override void Down()
        {
            DropIndex("MessageQueue", new[] { "EmailAccountId" });
            DropForeignKey("MessageQueue", "EmailAccountId", "EmailAccount");
            DropColumn("MessageQueue", "EmailAccountId");
            DropTable("EmailAccount");
        }
    }
}

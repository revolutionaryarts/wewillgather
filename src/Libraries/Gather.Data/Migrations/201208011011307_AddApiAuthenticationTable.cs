namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddApiAuthenticationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ApiAuthentication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.Int(),
                        LastModifiedDate = c.DateTime(),
                        NameOfApplication = c.String(maxLength: 200),
                        Description = c.String(maxLength: 400),
                        WebsiteAddress = c.String(maxLength: 200),
                        SecretKey = c.String(),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ApiUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User", t => t.ApiUser_Id)
                .Index(t => t.ApiUser_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("ApiAuthentication", new[] { "ApiUser_Id" });
            DropForeignKey("ApiAuthentication", "ApiUser_Id", "User");
            DropTable("ApiAuthentication");
        }
    }
}

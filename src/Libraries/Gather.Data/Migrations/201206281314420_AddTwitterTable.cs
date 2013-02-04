namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddTwitterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Tweet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(maxLength: 200),
                        TwitterProfile = c.String(maxLength: 1000),
                        TwitterId = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Tweet");
        }
    }
}

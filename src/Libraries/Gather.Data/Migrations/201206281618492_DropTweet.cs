namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DropTweet : DbMigration
    {
        public override void Up()
        {
            DropTable("Tweet");
        }
        
        public override void Down()
        {
            CreateTable(
                "Tweet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(maxLength: 200),
                        TwitterId = c.Long(nullable: false),
                        TwitterProfile = c.String(maxLength: 1000),
                        UserName = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}

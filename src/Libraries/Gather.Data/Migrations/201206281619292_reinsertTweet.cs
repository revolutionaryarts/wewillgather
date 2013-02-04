namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class reinsertTweet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Tweet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        Text = c.String(maxLength: 200),
                        TwitterId = c.Long(nullable: false),
                        TwitterProfile = c.String(maxLength: 1000),
                        UserName = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Tweet");
        }
    }
}

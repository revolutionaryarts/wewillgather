namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class idOnTweetsChangedToULong : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Tweet", "TwitterId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Tweet", "TwitterId", c => c.String(maxLength: 1000));
        }
    }
}

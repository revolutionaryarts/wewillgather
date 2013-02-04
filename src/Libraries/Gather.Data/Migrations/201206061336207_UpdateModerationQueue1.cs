namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModerationQueue1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ModerationQueue", "ModeratedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("ModerationQueue", "ModeratedDate", c => c.DateTime(nullable: false));
        }
    }
}

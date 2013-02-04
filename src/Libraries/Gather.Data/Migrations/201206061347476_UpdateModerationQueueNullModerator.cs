namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModerationQueueNullModerator : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ModerationQueue", "ModeratedBy", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("ModerationQueue", "ModeratedBy", c => c.Int(nullable: false));
        }
    }
}

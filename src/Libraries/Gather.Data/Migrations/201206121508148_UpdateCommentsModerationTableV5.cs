namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentsModerationTableV5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("ModerationQueue", "Deleted", c => c.Boolean(nullable: false));
            DropColumn("ProjectCommentModeration", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("ProjectCommentModeration", "Deleted", c => c.Boolean(nullable: false));
            DropColumn("ModerationQueue", "Deleted");
        }
    }
}

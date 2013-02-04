namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentsModerationTableV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProjectCommentModeration", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProjectCommentModeration", "Deleted");
        }
    }
}

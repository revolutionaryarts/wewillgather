namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentsModerationTableV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Comment", "ModeratedBy", c => c.Int());
            AddColumn("Comment", "ModeratedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Comment", "ModeratedDate");
            DropColumn("Comment", "ModeratedBy");
        }
    }
}

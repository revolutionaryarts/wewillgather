namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentsModerationTableV3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Comment", "LastModifiedBy", c => c.Int());
            AlterColumn("Comment", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("Comment", "LastModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("Comment", "LastModifiedBy", c => c.Int(nullable: false));
        }
    }
}

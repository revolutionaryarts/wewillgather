namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectWithdrawalModerationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("Moderation_ProjectWithdrawal", "Reason", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("Moderation_ProjectWithdrawal", "Reason");
        }
    }
}

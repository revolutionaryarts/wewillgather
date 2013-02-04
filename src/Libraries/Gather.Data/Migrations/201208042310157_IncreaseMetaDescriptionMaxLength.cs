namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseMetaDescriptionMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Page", "MetaDescription", c => c.String(maxLength: 400));
            AlterColumn("SuccessStory", "MetaDescription", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            AlterColumn("SuccessStory", "MetaDescription", c => c.String(maxLength: 150));
            AlterColumn("Page", "MetaDescription", c => c.String(maxLength: 150));
        }
    }
}

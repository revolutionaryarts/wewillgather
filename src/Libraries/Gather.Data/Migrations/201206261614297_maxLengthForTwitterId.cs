namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class maxLengthForTwitterId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Project", "TwitterProfile", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("Project", "TwitterProfile", c => c.String());
        }
    }
}

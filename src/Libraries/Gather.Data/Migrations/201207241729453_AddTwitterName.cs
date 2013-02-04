namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddTwitterName : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tweet", "TwitterName", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("Tweet", "TwitterName");
        }
    }
}

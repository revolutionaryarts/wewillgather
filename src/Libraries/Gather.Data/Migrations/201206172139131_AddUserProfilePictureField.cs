namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfilePictureField : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "ProfilePicture", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("User", "ProfilePicture");
        }
    }
}

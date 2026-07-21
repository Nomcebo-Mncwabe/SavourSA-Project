namespace SavourSA_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Bio", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Bio");
        }
    }
}

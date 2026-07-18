namespace SavourSA_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavouriteTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        FavouriteId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        RecipeId = c.Int(nullable: false),
                        DateSaved = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FavouriteId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RecipeId);
            
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "Province", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favourites", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Favourites", "RecipeId", "dbo.Recipes");
            DropIndex("dbo.Favourites", new[] { "RecipeId" });
            DropIndex("dbo.Favourites", new[] { "UserId" });
            AlterColumn("dbo.AspNetUsers", "Province", c => c.String());
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false));
            DropTable("dbo.Favourites");
        }
    }
}

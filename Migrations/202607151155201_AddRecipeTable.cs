namespace SavourSA_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecipeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        RecipeId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false),
                        Ingredients = c.String(nullable: false),
                        Instructions = c.String(nullable: false),
                        PrepTime = c.Int(nullable: false),
                        CookTime = c.Int(nullable: false),
                        Servings = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RecipeId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Recipes", new[] { "UserId" });
            DropTable("dbo.Recipes");
        }
    }
}

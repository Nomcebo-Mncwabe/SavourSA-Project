namespace SavourSA_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        RecipeId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        ShortDescription = c.String(maxLength: 250),
                        Ingredients = c.String(nullable: false),
                        Instructions = c.String(nullable: false),
                        ImageUrl = c.String(),
                        Category = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                    })
                .PrimaryKey(t => t.RecipeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recipes");
        }
    }
}

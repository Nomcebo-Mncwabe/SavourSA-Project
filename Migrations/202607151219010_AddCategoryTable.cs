namespace SavourSA_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            AddColumn("dbo.Recipes", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Recipes", "CategoryId");
            AddForeignKey("dbo.Recipes", "CategoryId", "dbo.Categories", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Recipes", new[] { "CategoryId" });
            DropColumn("dbo.Recipes", "CategoryId");
            DropTable("dbo.Categories");
        }
    }
}

namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoStockOutProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockOutProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StockOutId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.StockOuts", t => t.StockOutId, cascadeDelete: true)
                .Index(t => t.StockOutId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StockOuts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComplainId = c.Int(nullable: false),
                        StockIssueDateTime = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Complains", t => t.ComplainId, cascadeDelete: true)
                .Index(t => t.ComplainId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockOutProducts", "StockOutId", "dbo.StockOuts");
            DropForeignKey("dbo.StockOuts", "ComplainId", "dbo.Complains");
            DropForeignKey("dbo.StockOutProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.StockOuts", new[] { "ComplainId" });
            DropIndex("dbo.StockOutProducts", new[] { "ProductId" });
            DropIndex("dbo.StockOutProducts", new[] { "StockOutId" });
            DropTable("dbo.StockOuts");
            DropTable("dbo.StockOutProducts");
        }
    }
}

namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoStockInProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockInProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StockInId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.StockIns", t => t.StockInId, cascadeDelete: true)
                .Index(t => t.StockInId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StockIns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseOrderId = c.Int(nullable: false),
                        InventoryDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrderId, cascadeDelete: true)
                .Index(t => t.PurchaseOrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockInProducts", "StockInId", "dbo.StockIns");
            DropForeignKey("dbo.StockIns", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.StockInProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.StockIns", new[] { "PurchaseOrderId" });
            DropIndex("dbo.StockInProducts", new[] { "ProductId" });
            DropIndex("dbo.StockInProducts", new[] { "StockInId" });
            DropTable("dbo.StockIns");
            DropTable("dbo.StockInProducts");
        }
    }
}

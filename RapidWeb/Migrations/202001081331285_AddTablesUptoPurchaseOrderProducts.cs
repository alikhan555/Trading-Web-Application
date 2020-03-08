namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoPurchaseOrderProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchaseOrderProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseOrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrderId, cascadeDelete: true)
                .Index(t => t.PurchaseOrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseRequestId = c.Int(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false, precision: 0),
                        VendorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseRequests", t => t.PurchaseRequestId, cascadeDelete: true)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.PurchaseRequestId)
                .Index(t => t.VendorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrderProducts", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.PurchaseOrders", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.PurchaseOrders", "PurchaseRequestId", "dbo.PurchaseRequests");
            DropForeignKey("dbo.PurchaseOrderProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.PurchaseOrders", new[] { "VendorId" });
            DropIndex("dbo.PurchaseOrders", new[] { "PurchaseRequestId" });
            DropIndex("dbo.PurchaseOrderProducts", new[] { "ProductId" });
            DropIndex("dbo.PurchaseOrderProducts", new[] { "PurchaseOrderId" });
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.PurchaseOrderProducts");
        }
    }
}

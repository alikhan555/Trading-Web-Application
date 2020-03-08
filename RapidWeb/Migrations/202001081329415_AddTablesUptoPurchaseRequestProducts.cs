namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoPurchaseRequestProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchaseRequestProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseRequestId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.PurchaseRequests", t => t.PurchaseRequestId, cascadeDelete: true)
                .Index(t => t.PurchaseRequestId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PurchaseRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 0),
                        RequestFor = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseRequestProducts", "PurchaseRequestId", "dbo.PurchaseRequests");
            DropForeignKey("dbo.PurchaseRequestProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.PurchaseRequestProducts", new[] { "ProductId" });
            DropIndex("dbo.PurchaseRequestProducts", new[] { "PurchaseRequestId" });
            DropTable("dbo.PurchaseRequests");
            DropTable("dbo.PurchaseRequestProducts");
        }
    }
}

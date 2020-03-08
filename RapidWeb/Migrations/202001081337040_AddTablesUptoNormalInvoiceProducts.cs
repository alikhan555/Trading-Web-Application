namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoNormalInvoiceProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NormalInvoiceProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NormalInvoiceId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NormalInvoices", t => t.NormalInvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.NormalInvoiceId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.NormalInvoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        ComplainId = c.Int(nullable: false),
                        PurchaseOrderId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, unicode: false),
                        CustomerAddress = c.String(nullable: false, unicode: false),
                        CoAddress = c.String(nullable: false, unicode: false),
                        WorkNature = c.String(nullable: false, unicode: false),
                        InWords = c.String(nullable: false, unicode: false),
                        TotalAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NormalInvoiceProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.NormalInvoiceProducts", "NormalInvoiceId", "dbo.NormalInvoices");
            DropIndex("dbo.NormalInvoiceProducts", new[] { "ProductId" });
            DropIndex("dbo.NormalInvoiceProducts", new[] { "NormalInvoiceId" });
            DropTable("dbo.NormalInvoices");
            DropTable("dbo.NormalInvoiceProducts");
        }
    }
}

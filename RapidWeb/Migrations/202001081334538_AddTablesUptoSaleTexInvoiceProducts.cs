namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoSaleTexInvoiceProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesTaxInvoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        ComplainId = c.Int(nullable: false),
                        PurchaseOrderId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, unicode: false),
                        CustomerAddress = c.String(nullable: false, unicode: false),
                        PSTRNo = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        NTNNo = c.String(nullable: false, maxLength: 12, storeType: "nvarchar"),
                        CoAddress = c.String(nullable: false, unicode: false),
                        CoPSTRNo = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        CoNTNNo = c.String(nullable: false, maxLength: 12, storeType: "nvarchar"),
                        WorkNature = c.String(nullable: false, unicode: false),
                        InWords = c.String(nullable: false, unicode: false),
                        TotalExcludingSaleTex = c.Double(nullable: false),
                        SaleTexAmountPayable = c.Double(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SaleTexInvoiceProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesTaxInvoiceId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        RequiredQty = c.Int(nullable: false),
                        UnitPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.SalesTaxInvoices", t => t.SalesTaxInvoiceId, cascadeDelete: true)
                .Index(t => t.SalesTaxInvoiceId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleTexInvoiceProducts", "SalesTaxInvoiceId", "dbo.SalesTaxInvoices");
            DropForeignKey("dbo.SaleTexInvoiceProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.SaleTexInvoiceProducts", new[] { "ProductId" });
            DropIndex("dbo.SaleTexInvoiceProducts", new[] { "SalesTaxInvoiceId" });
            DropTable("dbo.SaleTexInvoiceProducts");
            DropTable("dbo.SalesTaxInvoices");
        }
    }
}

namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyNTNAndPSTRColsInSaleTaxInvoice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalesTaxInvoices", "PSTRNo", c => c.String(maxLength: 15, storeType: "nvarchar"));
            AlterColumn("dbo.SalesTaxInvoices", "NTNNo", c => c.String(maxLength: 12, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesTaxInvoices", "NTNNo", c => c.String(nullable: false, maxLength: 12, storeType: "nvarchar"));
            AlterColumn("dbo.SalesTaxInvoices", "PSTRNo", c => c.String(nullable: false, maxLength: 15, storeType: "nvarchar"));
        }
    }
}

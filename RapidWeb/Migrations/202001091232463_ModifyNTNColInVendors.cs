namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyNTNColInVendors : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Vendors", "NTNNo", c => c.String(maxLength: 12, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vendors", "NTNNo", c => c.String(nullable: false, maxLength: 12, storeType: "nvarchar"));
        }
    }
}

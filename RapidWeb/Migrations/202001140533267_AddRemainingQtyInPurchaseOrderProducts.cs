namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemainingQtyInPurchaseOrderProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrderProducts", "RemainingQty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrderProducts", "RemainingQty");
        }
    }
}

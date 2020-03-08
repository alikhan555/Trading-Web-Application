namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemainingQtyInPurchaseRequestProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseRequestProducts", "RemainingQty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseRequestProducts", "RemainingQty");
        }
    }
}

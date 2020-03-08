namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSampleQuantityColInProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "SampleQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "SampleQuantity");
        }
    }
}

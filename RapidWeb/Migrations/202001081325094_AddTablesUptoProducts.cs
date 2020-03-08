namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, unicode: false),
                        SerialNo = c.String(nullable: false, unicode: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        Cost = c.Double(nullable: false),
                        LastOutCost = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, unicode: false),
                        PackageType = c.String(nullable: false, unicode: false),
                        Maintenance = c.Boolean(nullable: false),
                        Installation = c.Boolean(nullable: false),
                        Development = c.Boolean(nullable: false),
                        Troubleshooting = c.Boolean(nullable: false),
                        Networking = c.Boolean(nullable: false),
                        Other = c.Boolean(nullable: false),
                        PackageDetails = c.String(nullable: false, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        City = c.String(nullable: false, unicode: false),
                        PaymentType = c.String(nullable: false, unicode: false),
                        NTNNo = c.String(nullable: false, maxLength: 12, storeType: "nvarchar"),
                        Contact = c.String(nullable: false, maxLength: 17, storeType: "nvarchar"),
                        Address = c.String(nullable: false, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Vendors");
            DropTable("dbo.Services");
            DropTable("dbo.Products");
        }
    }
}

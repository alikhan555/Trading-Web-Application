namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoClients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RefCode = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 35, storeType: "nvarchar"),
                        CityId = c.Int(nullable: false),
                        Address = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Area = c.String(nullable: false, unicode: false),
                        ContactPersonName = c.String(nullable: false, unicode: false),
                        ContactNo = c.String(nullable: false, maxLength: 17, storeType: "nvarchar"),
                        ContactLandLine = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        PackageId = c.Int(nullable: false),
                        Email = c.String(nullable: false, unicode: false),
                        OnDays = c.Int(nullable: false),
                        Timing = c.String(unicode: false),
                        CreditDays = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceCities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.Packages", t => t.PackageId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.PackageId);
            
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, unicode: false),
                        Description = c.String(nullable: false, unicode: false),
                        Detail = c.String(nullable: false, unicode: false),
                        Job = c.String(nullable: false, unicode: false),
                        CustomizeDetail = c.String(nullable: false, unicode: false),
                        ReferenceNote = c.String(nullable: false, unicode: false),
                        Price = c.Double(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "PackageId", "dbo.Packages");
            DropForeignKey("dbo.Clients", "CityId", "dbo.ServiceCities");
            DropIndex("dbo.Clients", new[] { "PackageId" });
            DropIndex("dbo.Clients", new[] { "CityId" });
            DropTable("dbo.Packages");
            DropTable("dbo.Clients");
        }
    }
}

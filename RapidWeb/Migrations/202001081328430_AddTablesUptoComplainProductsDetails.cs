namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoComplainProductsDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ComplainProductsDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComplainId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Qty = c.Int(nullable: false),
                        Cost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Complains", t => t.ComplainId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ComplainId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Complains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ContactPerson = c.String(nullable: false, unicode: false),
                        CreationDateTime = c.DateTime(nullable: false, precision: 0),
                        CompletionDateTime = c.DateTime(nullable: false, precision: 0),
                        ComplainReference = c.String(nullable: false, unicode: false),
                        ComplainType = c.String(nullable: false, unicode: false),
                        ProgressReport = c.String(unicode: false),
                        EmployeeId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        ComplainNature = c.String(nullable: false, unicode: false),
                        IsApproved = c.Boolean(nullable: false),
                        Description = c.String(nullable: false, unicode: false),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ServiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ComplainProductsDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ComplainProductsDetails", "ComplainId", "dbo.Complains");
            DropForeignKey("dbo.Complains", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Complains", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Complains", "ClientId", "dbo.Clients");
            DropIndex("dbo.Complains", new[] { "ServiceId" });
            DropIndex("dbo.Complains", new[] { "EmployeeId" });
            DropIndex("dbo.Complains", new[] { "ClientId" });
            DropIndex("dbo.ComplainProductsDetails", new[] { "ProductId" });
            DropIndex("dbo.ComplainProductsDetails", new[] { "ComplainId" });
            DropTable("dbo.Complains");
            DropTable("dbo.ComplainProductsDetails");
        }
    }
}

namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoEmployees : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 35, storeType: "nvarchar"),
                        Guardian = c.String(nullable: false, maxLength: 35, storeType: "nvarchar"),
                        Address = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        CNIC = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        ContactMobile = c.String(nullable: false, maxLength: 17, storeType: "nvarchar"),
                        ContactHome = c.String(nullable: false, maxLength: 17, storeType: "nvarchar"),
                        Designation = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Dob = c.DateTime(nullable: false, precision: 0),
                        Gender = c.String(nullable: false, maxLength: 6, storeType: "nvarchar"),
                        Qualification = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        DepartmentId = c.Int(nullable: false),
                        Experience = c.String(nullable: false, maxLength: 40, storeType: "nvarchar"),
                        DateOfJoining = c.DateTime(nullable: false, precision: 0),
                        Salary = c.Double(nullable: false),
                        CityId = c.Int(nullable: false),
                        HouseRent = c.Double(nullable: false),
                        TransportAllowance = c.Double(nullable: false),
                        UtilityAllowance = c.Double(nullable: false),
                        BonusAllowance = c.Double(nullable: false),
                        OtherBenefits = c.Double(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        BankAccountDetail = c.String(nullable: false, unicode: false),
                        Note = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceCities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.ServiceCities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        District = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Employees", "CityId", "dbo.ServiceCities");
            DropIndex("dbo.Employees", new[] { "CityId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropTable("dbo.ServiceCities");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
        }
    }
}

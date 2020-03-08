namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoEmployeeSalaries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeSalaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayrollId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        BasicSalary = c.Double(nullable: false),
                        HousingAllowance = c.Double(nullable: false),
                        UtilityAllowance = c.Double(nullable: false),
                        TransportAllowance = c.Double(nullable: false),
                        BonusAllowance = c.Double(nullable: false),
                        OtherAllowance = c.Double(nullable: false),
                        TaxDeduction = c.Double(nullable: false),
                        OtherDeduction = c.Double(nullable: false),
                        Remarks = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Payrolls", t => t.PayrollId, cascadeDelete: true)
                .Index(t => t.PayrollId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Payrolls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeSalaries", "PayrollId", "dbo.Payrolls");
            DropForeignKey("dbo.Payrolls", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.EmployeeSalaries", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Payrolls", new[] { "DepartmentId" });
            DropIndex("dbo.EmployeeSalaries", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeSalaries", new[] { "PayrollId" });
            DropTable("dbo.Payrolls");
            DropTable("dbo.EmployeeSalaries");
        }
    }
}

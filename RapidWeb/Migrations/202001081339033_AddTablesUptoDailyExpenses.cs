namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTablesUptoDailyExpenses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DailyExpenses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExpenseVoucherDate = c.DateTime(nullable: false, precision: 0),
                        EmployeeId = c.Int(nullable: false),
                        InAccountOf = c.String(nullable: false, unicode: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DailyExpenses", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.DailyExpenses", new[] { "EmployeeId" });
            DropTable("dbo.DailyExpenses");
        }
    }
}
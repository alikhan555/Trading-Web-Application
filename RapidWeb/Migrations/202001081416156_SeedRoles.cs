namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedRoles : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Roles(id,Name) VALUES(1,'Master User')");
            Sql("INSERT INTO Roles(id,Name) VALUES(2,'Admin User')");
            Sql("INSERT INTO Roles(id,Name) VALUES(3,'Data Operator')");
            Sql("INSERT INTO Roles(id,Name) VALUES(4,'HR')");
            Sql("INSERT INTO Roles(id,Name) VALUES(5,'Inventory')");
            Sql("INSERT INTO Roles(id,Name) VALUES(6,'RPT User')");
            Sql("INSERT INTO Roles(id,Name) VALUES(7,'Accounts')");
        }
        
        public override void Down()
        {
        }
    }
}

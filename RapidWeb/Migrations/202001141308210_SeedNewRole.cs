namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedNewRole : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Roles(id,Name) VALUES(8,'Activity')");
        }
        
        public override void Down()
        {

        }
    }
}

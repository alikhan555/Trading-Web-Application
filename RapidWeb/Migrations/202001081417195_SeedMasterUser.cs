namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedMasterUser : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Users(username,password,roleId) VALUES('Muhammad Ali', 'ali123', 1)");
        }
        
        public override void Down()
        {
        }
    }
}

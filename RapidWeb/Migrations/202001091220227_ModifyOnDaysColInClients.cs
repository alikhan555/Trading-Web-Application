namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyOnDaysColInClients : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "ContactLandLine", c => c.String(nullable: false, maxLength: 12, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "ContactLandLine", c => c.String(nullable: false, maxLength: 20, storeType: "nvarchar"));
        }
    }
}

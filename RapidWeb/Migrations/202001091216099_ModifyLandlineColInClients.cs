namespace RapidWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyLandlineColInClients : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "OnDays", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "OnDays", c => c.Int(nullable: false));
        }
    }
}

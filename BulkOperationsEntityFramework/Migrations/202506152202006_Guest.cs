namespace BulkOperationsEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Guest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Arkiv.Guest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Arkiv.Guest");
        }
    }
}

namespace BulkOperationsEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArchivedUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Archive.ArchivedUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Archive.ArchivedUser");
        }
    }
}

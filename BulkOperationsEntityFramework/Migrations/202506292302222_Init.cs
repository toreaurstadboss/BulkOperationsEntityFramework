namespace BulkOperationsEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Archive.Arkivertbrukere",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Arkiv.Gjester",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Brukere",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 255),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Jubileer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sesjoner",
                c => new
                    {
                        Key = c.Guid(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ExpiresAt = c.DateTime(),
                        IpAddress = c.String(maxLength: 255),
                        UserAgent = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sesjoner");
            DropTable("dbo.Jubileer");
            DropTable("dbo.Brukere");
            DropTable("Arkiv.Gjester");
            DropTable("Archive.Arkivertbrukere");
        }
    }
}

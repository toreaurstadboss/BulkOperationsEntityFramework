namespace BulkOperationsEntityFramework.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SessionsAndGuests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Arkiv.Guests",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Email = c.String(maxLength: 255),
                    FirstName = c.String(maxLength: 255),
                    LastName = c.String(maxLength: 255),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "Archive.ArchivedUsers",
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
                "dbo.Sessions",
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
            DropTable("dbo.Sessions");
            DropTable("Archive.ArchivedUsers");
            DropTable("Arkiv.Guests");
        }
    }
}

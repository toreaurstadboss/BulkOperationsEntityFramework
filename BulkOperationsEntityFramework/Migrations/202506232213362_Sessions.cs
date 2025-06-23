namespace BulkOperationsEntityFramework.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Sessions : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}

namespace BulkOperationsEntityFramework.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MaxStringLengthSetTo255 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 255));
            AlterColumn("dbo.Users", "FirstName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Users", "LastName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Users", "PhoneNumber", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.Users", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Users", "LastName", c => c.String());
            AlterColumn("dbo.Users", "FirstName", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
        }
    }
}

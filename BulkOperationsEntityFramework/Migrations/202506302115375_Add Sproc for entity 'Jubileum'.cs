namespace BulkOperationsEntityFramework.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddSprocforentityJubileum : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.Jubileum_Insert",
                p => new
                {
                    Date = p.DateTime(),
                    Description = p.String(maxLength: 255),
                },
                body:
                    @"INSERT [dbo].[Jubileer]([Date], [Description])
                      VALUES (@Date, @Description)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Jubileer]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Jubileer] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );

            CreateStoredProcedure(
                "dbo.Jubileum_Update",
                p => new
                {
                    Id = p.Int(),
                    Date = p.DateTime(),
                    Description = p.String(maxLength: 255),
                },
                body:
                    @"UPDATE [dbo].[Jubileer]
                      SET [Date] = @Date, [Description] = @Description
                      WHERE ([Id] = @Id)"
            );

            CreateStoredProcedure(
                "dbo.Jubileum_Delete",
                p => new
                {
                    Id = p.Int(),
                },
                body:
                    @"DELETE [dbo].[Jubileer]
                      WHERE ([Id] = @Id)"
            );

        }

        public override void Down()
        {
            DropStoredProcedure("dbo.Jubileum_Delete");
            DropStoredProcedure("dbo.Jubileum_Update");
            DropStoredProcedure("dbo.Jubileum_Insert");
        }
    }
}

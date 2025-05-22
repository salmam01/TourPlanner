using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.Migrations
{
    public partial class AddFullTextSearchSupportOnTours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add tsvector column for full-text search
            migrationBuilder.Sql(@"
                ALTER TABLE ""Tours""
                ADD COLUMN ""SearchVector"" tsvector GENERATED ALWAYS AS (
                    to_tsvector('english', coalesce(""Name"", '') || ' ' || coalesce(""Description"", '') || ' ' || coalesce(""From"", '') || ' ' || coalesce(""To"", '') || ' ' || coalesce(""TransportType"", ''))
                ) STORED;
            ");

            // Create GIN index on tsvector column
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_Tours_SearchVector"" ON ""Tours"" USING GIN (""SearchVector"");
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            // Drop the GIN index and the tsvector column
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Tours_SearchVector"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Tours"" DROP COLUMN IF EXISTS ""SearchVector"";");
        }
    }
}
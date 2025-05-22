using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.Migrations
{
    public partial class AddFullTextSearchSupportOnTourLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add tsvector column for full-text search
            migrationBuilder.Sql(@"
                ALTER TABLE ""TourLogs""
                ADD COLUMN ""SearchVector"" tsvector GENERATED ALWAYS AS (
                    to_tsvector('english', 
                        coalesce(""Comment"", '') || ' ' ||
                        coalesce(""Difficulty""::text, '') || ' ' ||
                        coalesce(""Rating""::text, '') || ' ' ||
                        coalesce(""TotalDistance""::text, '') || ' ' ||
                        coalesce(""TotalTime""::text, '')
                    )
                ) STORED;
            ");

            // Create GIN index on tsvector column
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_TourLogs_SearchVector"" ON ""TourLogs"" USING GIN (""SearchVector"");
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            // Drop the GIN index and the tsvector column
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_TourLogs_SearchVector"";");
            migrationBuilder.Sql(@"ALTER TABLE ""TourLogs"" DROP COLUMN IF EXISTS ""SearchVector"";");
        }
    }
}
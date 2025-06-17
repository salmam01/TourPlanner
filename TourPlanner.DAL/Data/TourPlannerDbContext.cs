using Microsoft.EntityFrameworkCore;
using TourPlanner.Models.Entities;


namespace TourPlanner.DAL.Data
{
    public class TourPlannerDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourAttributes> TourAttributes { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  One tour has one tour attributes
            modelBuilder.Entity<Tour>()
                .HasOne(t => t.TourAttributes)
                .WithOne()
                .HasForeignKey<TourAttributes>(ta => ta.Id);

            //  One tour has many tour logs
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.TourLogs)
                .WithOne(tl => tl.Tour)
                .HasForeignKey(tl => tl.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Full-text search vector
            modelBuilder.Entity<Tour>(t =>
            {
                t.Property<NpgsqlTypes.NpgsqlTsVector>("SearchVector")
                    .HasColumnType("tsvector")
                    .HasComputedColumnSql(
                        "to_tsvector('simple', " +
                        "coalesce(\"Name\", '') || ' ' || " +
                        "coalesce(\"Description\", '') || ' ' || " +
                        "coalesce(\"From\", '') || ' ' || " +
                        "coalesce(\"To\", '') || ' ' || " +
                        "coalesce(\"TransportType\", '') || ' ' || " +
                        "coalesce(\"Distance\"::text, '')" +
                        ")",
                        stored: true
                    );
                t.HasIndex("SearchVector").HasMethod("GIN");
            });

            modelBuilder.Entity<TourLog>(tl =>
            {
                tl.Property<NpgsqlTypes.NpgsqlTsVector>("SearchVector")
                    .HasColumnType("tsvector")
                    .HasComputedColumnSql(
                        "to_tsvector('simple', " +
                        "coalesce(\"Difficulty\"::text, '') || ' ' || " +
                        "coalesce(\"Rating\"::text, '') || ' ' || " +
                        "coalesce(\"Comment\", '') || ' ' || " +
                        "coalesce(\"TotalDistance\"::text, '')" +
                        ")",
                        stored: true
                    );
                tl.HasIndex("SearchVector").HasMethod("GIN");
            });
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AoIS_course_work.Models
{
    public partial class aoisContext : DbContext
    {
        public aoisContext()
        {
        }

        public aoisContext(DbContextOptions<aoisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Film> Films { get; set; }
        public virtual DbSet<FilmGenre> FilmGenres { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=Gato1_otaG990;database=aois", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            modelBuilder.Entity<Film>(entity =>
            {
                entity.ToTable("film");

                entity.HasIndex(e => e.OriginalName, "original_name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.FilmId).HasColumnName("film_id");

                entity.Property(e => e.AddedDate)
                    .HasColumnType("date")
                    .HasColumnName("added_date")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Director)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("director")
                    .HasDefaultValueSql("'Не указан'");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("duration")
                    .HasDefaultValueSql("'Не указана'");

                entity.Property(e => e.OriginalName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("original_name");

                entity.Property(e => e.Rating)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("rating")
                    .HasDefaultValueSql("'Не указан'");

                entity.Property(e => e.ReleaseYear)
                    .HasColumnType("year")
                    .HasColumnName("release_year")
                    .HasDefaultValueSql("'1970'");

                entity.Property(e => e.TranslatedName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("translatedName");
            });

            modelBuilder.Entity<FilmGenre>(entity =>
            {
                entity.HasKey(e => new { e.IdFilm, e.IdGenre })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("film_genres");

                entity.HasIndex(e => e.IdGenre, "fk_Film_genres_Genre1_idx");

                entity.Property(e => e.IdFilm).HasColumnName("id_film");

                entity.Property(e => e.IdGenre).HasColumnName("id_genre");

                entity.HasOne(d => d.IdFilmNavigation)
                    .WithMany(p => p.FilmGenres)
                    .HasForeignKey(d => d.IdFilm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Film_genres_Film");

                entity.HasOne(d => d.IdGenreNavigation)
                    .WithMany(p => p.FilmGenres)
                    .HasForeignKey(d => d.IdGenre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Film_genres_Genre1");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genre");

                entity.HasIndex(e => e.GenreName, "genre_name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.GenreId).HasColumnName("genre_id");

                entity.Property(e => e.GenreName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("genre_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

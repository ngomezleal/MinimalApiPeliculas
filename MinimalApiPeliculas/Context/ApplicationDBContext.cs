using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().Property(p => p.Nombre).HasMaxLength(50);
            modelBuilder.Entity<Actor>().Property(p => p.Nombre).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(p => p.Foto).IsUnicode();

            modelBuilder.Entity<Pelicula>().Property(p=> p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p=> p.Poster).IsUnicode();
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
    }
}
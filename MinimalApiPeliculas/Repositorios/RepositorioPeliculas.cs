using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Context;
using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor, 
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDto paginacionDto)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDto).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GeneroPeliculas).ThenInclude(gp => gp.Genero)
                .Include(p => p.ActoresPeliculas.OrderBy(p=> p.Orden)).ThenInclude(ap => ap.Actor)
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }

        public async Task AsignarGeneros(int peliculaId, List<int> generosIds)
        {
            var pelicula = await context.Peliculas
                .Include(gp=> gp.GeneroPeliculas)
                .FirstOrDefaultAsync(p => p.Id == peliculaId);

            if (pelicula is null)
                throw new ArgumentNullException($"No se encuentra el id {peliculaId}");

            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula() { GeneroId = generoId }); //Proyeccion
            pelicula.GeneroPeliculas = mapper.Map(generosPeliculas, pelicula.GeneroPeliculas); //Segregacion, edita, elimina
            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int peliculaId, List<ActorPelicula> actorPeliculas)
        {
            for (int i = 1; i <= actorPeliculas.Count; i++)
                actorPeliculas[i - 1].Orden = i;

            var pelicula = await context.Peliculas.Include(p => p.ActoresPeliculas)
                .FirstOrDefaultAsync(p => p.Id == peliculaId);

            if (pelicula is null)
                throw new ArgumentException($"No existe la pelicula con el id {peliculaId}");

            pelicula.ActoresPeliculas = mapper.Map(actorPeliculas, pelicula.ActoresPeliculas);
            await context.SaveChangesAsync();
        }
    }
}
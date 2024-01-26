using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int peliculaId, List<ActorPelicula> actorPeliculas);
        Task AsignarGeneros(int peliculaId, List<int> generosIds);
        Task<int> Crear(Pelicula pelicula);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDto paginacionDto);
    }
}
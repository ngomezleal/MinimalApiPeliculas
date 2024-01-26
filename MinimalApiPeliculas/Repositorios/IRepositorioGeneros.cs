
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<List<Genero>> ObtenerTodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<int> Crear(Genero genero);
        Task<bool> Existe(int id);
        Task<List<int>> Existe(List<int> ids);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
    }
}
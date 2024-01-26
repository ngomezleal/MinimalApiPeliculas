using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioComentarios
    {
        Task Actualizar(Comentario comentario);
        Task<int> Crear(Comentario comentario);
        Task<int> Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Comentario?> ObtenerPorId(int id);
        Task<List<Comentario>> ObtenerTodos(int peliculaId);
    }
}
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Context;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDBContext applicationDBContext;

        public RepositorioComentarios(ApplicationDBContext applicationDBContext)
        {
            this.applicationDBContext = applicationDBContext;
        }

        public async Task<List<Comentario>> ObtenerTodos(int peliculaId)
        {
            return await applicationDBContext.Comentarios.Where(p => p.PeliculaId == peliculaId).ToListAsync();
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await applicationDBContext.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> Crear(Comentario comentario)
        {
            applicationDBContext.Add(comentario);
            await applicationDBContext.SaveChangesAsync();
            return comentario.Id;
        }

        public async Task Actualizar(Comentario comentario)
        {
            applicationDBContext.Update(comentario);
            await applicationDBContext.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await applicationDBContext.Comentarios.AnyAsync(c => c.Id == id);
        }

        public async Task<int> Eliminar(int id)
        {
            return await applicationDBContext.Comentarios.Where(c => c.Id == id).ExecuteDeleteAsync();
        }
    }
}
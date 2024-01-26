using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Context;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly ApplicationDBContext context;

        public RepositorioGeneros(ApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            return await context.Generos.OrderBy(g => g.Nombre).ToListAsync();
        }

        public async Task<int> Crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }

        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(g => g.Id == id);
        }

        public async Task<List<int>> Existe(List<int> ids)
        {
            return await context.Generos.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(g => g.Id == id).ExecuteDeleteAsync();
        }
    }
}
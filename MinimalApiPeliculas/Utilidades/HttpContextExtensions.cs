using Microsoft.EntityFrameworkCore;

namespace MinimalApiPeliculas.Utilidades
{
    public static class HttpContextExtensions
    {
        public static async Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext context, IQueryable<T> queryable)
        {
            if(context is null)
                throw new ArgumentNullException(nameof(context));

            var cantidad = await queryable.CountAsync();
            context.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
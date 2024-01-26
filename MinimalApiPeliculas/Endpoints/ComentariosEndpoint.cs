using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class ComentariosEndpoint
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
            .Tag("comentarios-tag")
            .SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear);
            group.MapPut("/{id:int}", Actualizar);
            group.MapDelete("/{id:int}", Eliminar);
            return group;
        }

        static async Task<Results<Ok<List<ComentarioDto>>, NotFound>> ObtenerTodos(int peliculaId, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            var existe = await repositorioPeliculas.Existe(peliculaId);
            if (!existe)
                return TypedResults.NotFound();

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentarioDto = mapper.Map<List<ComentarioDto>>(comentarios);
            return TypedResults.Ok(comentarioDto);
        }

        static async Task<Results<Ok<ComentarioDto>, NotFound>> ObtenerPorId(int peliculaId, int id, IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {
            var comentario = await repositorioComentarios.ObtenerPorId(id);
            if (comentario is null)
                return TypedResults.NotFound();

            var comentarioDto = mapper.Map<ComentarioDto>(comentario);
            return TypedResults.Ok(comentarioDto);
        }

        static async Task<Results<Created<ComentarioDto>, NotFound>> Crear(int peliculaId, CrearComentarioDto crearComentarioDto,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {

            var existe = await repositorioPeliculas.Existe(peliculaId);
            if (!existe)
                return TypedResults.NotFound();

            var comentario = mapper.Map<Comentario>(crearComentarioDto);
            comentario.PeliculaId = peliculaId;

            var id = await repositorioComentarios.Crear(comentario);

            var comentarioDto = mapper.Map<ComentarioDto>(comentario);
            comentarioDto.Id = id;
            await outputCacheStore.EvictByTagAsync("comentarios-tag", default);
            return TypedResults.Created($"/comentarios/{id}", comentarioDto);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int peliculaId, int id, CrearComentarioDto crearComentarioDto,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper, 
            IOutputCacheStore outputCacheStore)
        { 
            if (!await repositorioPeliculas.Existe(peliculaId))
                return TypedResults.NotFound();

            if (!await repositorioComentarios.Existe(id))
                return TypedResults.NotFound();

            var comentarioActualizar = mapper.Map<Comentario>(crearComentarioDto);
            comentarioActualizar.Id = id;
            comentarioActualizar.PeliculaId = peliculaId;
            await repositorioComentarios.Actualizar(comentarioActualizar);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Eliminar(int peliculaId, int id, IRepositorioComentarios repositorioComentarios,
            IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioComentarios.Existe(id))
                return TypedResults.NotFound();

            await repositorioComentarios.Eliminar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
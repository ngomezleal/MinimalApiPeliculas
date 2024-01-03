using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class GenerosEndpoint
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("/", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/{id:int}", BorrarGenero);
            return group;
        }

        #region Metodos Nombrados

        static async Task<Ok<List<GeneroDto>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();
            var generosDtos = mapper.Map<List<GeneroDto>>(generos);
            return TypedResults.Ok(generosDtos);
        }

        static async Task<Results<Ok<GeneroDto>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
                return TypedResults.NotFound();

            var generoDto = mapper.Map<GeneroDto>(genero);
            return TypedResults.Ok(generoDto);
        }

        static async Task<Created<GeneroDto>> CrearGenero(CrearGeneroDto crearGeneroDto, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDto);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiar caché

            var generoDto = mapper.Map<GeneroDto>(genero);
            return TypedResults.Created($"/generos/{id}", generoDto);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDto crearGeneroDto, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
                return TypedResults.NotFound();

            var genero = mapper.Map<Genero>(crearGeneroDto);
            genero.Id = id;

            await repositorioGeneros.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiar caché
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
                return TypedResults.NotFound();

            await repositorioGeneros.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiar caché
            return TypedResults.NoContent();
        }

        #endregion
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class PeliculasEndpoint
    {
        private static readonly string contenedor = "peliculas";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", CrearPelicula).DisableAntiforgery();
            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros);
            group.MapPost("/{id:int}/asignaractores", AsignarActores);
            group.MapPut("/{id:int}", ActualizarPelicula).DisableAntiforgery();
            group.MapDelete("/{id:int}", EliminarPelicula);
            return group;
        }

        static async Task<Ok<List<PeliculaDto>>> ObtenerTodos(IRepositorioPeliculas repositorioPeliculas, IMapper mapper, 
            int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacionDto = new PaginacionDto
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina,
            };

            var peliculas = await repositorioPeliculas.ObtenerTodos(paginacionDto);
            var peliculaDto = mapper.Map<List<PeliculaDto>>(peliculas);
            return TypedResults.Ok(peliculaDto);
        }

        static async Task<Results<Ok<PeliculaDto>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            var existe = await repositorioPeliculas.Existe(id);
            if (!existe)
                return TypedResults.NotFound();

            var pelicula = await repositorioPeliculas.ObtenerPorId(id);
            var peliculaDto = mapper.Map<PeliculaDto>(pelicula);
            return TypedResults.Ok(peliculaDto);
        }

        static async Task<Created<PeliculaDto>> CrearPelicula([FromForm] CrearPeliculaDto crearPeliculaDto,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDto);
            if (crearPeliculaDto.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDto.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorioPeliculas.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            var peliculaDto = mapper.Map<PeliculaDto>(pelicula);
            peliculaDto.Id = id;
            return TypedResults.Created($"/peliculas/{id}", peliculaDto);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarPelicula(int id, [FromForm] CrearPeliculaDto crearPeliculaDto,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            var pelicula = await repositorioPeliculas.ObtenerPorId(id);
            if (pelicula is null)
                return TypedResults.NotFound();

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDto);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = pelicula.Poster;

            if (crearPeliculaDto.Poster is not null)
            {
                string url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDto.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorioPeliculas.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> EliminarPelicula(int id, IAlmacenadorArchivos almacenadorArchivos,
            IRepositorioPeliculas repositorioPeliculas, IOutputCacheStore outputCacheStore)
        {
            var pelicula = await repositorioPeliculas.ObtenerPorId(id);
            if (pelicula is null)
                return TypedResults.NotFound();

            await repositorioPeliculas.Eliminar(id);
            await almacenadorArchivos.Borrar(pelicula.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioPeliculas.Existe(id))
                return TypedResults.NotFound();

            var generosExistentes = new List<int>();
            if (generosIds.Count != 0)
                generosExistentes = await repositorioGeneros.Existe(generosIds);

            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"Los generos de id {string.Join(",", generosNoExistentes)} no existen");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDto> asignarActorPeliculaDtos,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id))
                return TypedResults.NotFound();

            var actoresExistentes = new List<int>();
            var actoresIds = asignarActorPeliculaDtos.Select(a => a.ActorId).ToList();

            if (actoresIds.Count != 0)
                actoresExistentes = await repositorioActores.Existe(actoresIds);

            if (actoresExistentes.Count != asignarActorPeliculaDtos.Count)
            {
                //Algunos de los actores enviados no son validos
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresNoExistentes)} no existen");
            }

            var actores = mapper.Map<List<ActorPelicula>>(asignarActorPeliculaDtos);
            await repositorioPeliculas.AsignarActores(id, actores);

            return TypedResults.NoContent();
        }
    }
}
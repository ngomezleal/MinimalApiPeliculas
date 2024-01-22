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
    }
}
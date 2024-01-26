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
    public static class ActoresEndpoint
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            //Endpoints
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("ObtenerPorNombre/{nombre}", ObtenerPorNombre);
            group.MapPost("/", CrearActor).DisableAntiforgery();
            group.MapPut("/{id:int}", ActualizarActor).DisableAntiforgery();
            group.MapDelete("/{id:int}", BorrarActor);
            return group;
        }

        static async Task<Ok<List<ActorDto>>> ObtenerTodos(IRepositorioActores repositorioActores, IMapper mapper,
            int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDto
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina,
            };
            var actores = await repositorioActores.ObtenerTodos(paginacion);
            var actoresDto = mapper.Map<List<ActorDto>>(actores);
            return TypedResults.Ok(actoresDto);
        }

        static async Task<Results<Ok<ActorDto>, NotFound>> ObtenerPorId(int id, IRepositorioActores repositorioActores, IMapper mapper)
        {
            var actor = await repositorioActores.ObtenerPorId(id);
            if (actor is null)
                return TypedResults.NotFound();

            var actorDto = mapper.Map<ActorDto>(actor);
            return TypedResults.Ok(actorDto);
        }

        static async Task<Ok<List<ActorDto>>> ObtenerPorNombre(string nombre, IRepositorioActores repositorioActores, IMapper mapper)
        {
            var actores = await repositorioActores.ObtenerPorNombre(nombre);
            var actoresDto = mapper.Map<List<ActorDto>>(actores);
            return TypedResults.Ok(actoresDto);
        }

        static async Task<Created<ActorDto>> CrearActor([FromForm] CrearActorDto crearActorDto, IRepositorioActores repositorioActores,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            var actor = mapper.Map<Actor>(crearActorDto);
            if (crearActorDto.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDto.Foto);
                actor.Foto = url;
            }

            var id = await repositorioActores.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var autorDto = mapper.Map<ActorDto>(actor);
            return TypedResults.Created($"/actores/{id}", autorDto);
        }

        static async Task<Results<NotFound, NoContent>> ActualizarActor(int id, [FromForm] CrearActorDto crearActorDto, IRepositorioActores repositorioActores,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actor = await repositorioActores.ObtenerPorId(id);
            if (actor is null)
                return TypedResults.NotFound();

            var actorMapped = mapper.Map<Actor>(crearActorDto);
            actorMapped.Id = id;
            actorMapped.Foto = actor.Foto;

            if (crearActorDto.Foto is not null)
                actorMapped.Foto = await almacenadorArchivos.Editar(actor.Foto, contenedor, crearActorDto.Foto);

            await repositorioActores.Actualizar(id, actorMapped);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> BorrarActor(int id, IRepositorioActores repositorioActores,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actor = await repositorioActores.ObtenerPorId(id);
            if (actor is null)
                return TypedResults.NotFound();

            await repositorioActores.Borrar(id);
            await almacenadorArchivos.Borrar(actor.Foto, contenedor);
            return TypedResults.NoContent();
        }
    }
}
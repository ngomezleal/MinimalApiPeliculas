using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Endpoints;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Utilidades
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CrearGeneroDto, Genero>().ReverseMap();
            CreateMap<Genero, GeneroDto>().ReverseMap();
            CreateMap<CrearActorDto, Actor>().ReverseMap()
                .ForMember(a => a.Foto, options => options.Ignore()); //No hará conversion de IFormFile a String
            CreateMap<Actor, ActorDto>().ReverseMap();
            CreateMap<CrearPeliculaDto, Pelicula>().ReverseMap()
                .ForMember(p => p.Poster, options => options.Ignore());
            CreateMap<Pelicula, PeliculaDto>()
                .ForMember(p => p.GeneroPeliculas, entidad => entidad.MapFrom(p =>
                    p.GeneroPeliculas.Select(gp => new GeneroDto
                    {
                        Id = gp.GeneroId,
                        Nombre = gp.Genero.Nombre
                    })))
                .ForMember(p => p.ActoresPeliculas, entidad => entidad.MapFrom(p =>
                    p.ActoresPeliculas.Select(ap => new ActorPeliculaDto
                    {
                        Id = ap.ActorId,
                        Nombre = ap.Actor.Nombre,
                        Personaje = ap.Personaje
                    })))
                .ReverseMap();
            CreateMap<CrearComentarioDto, Comentario>().ReverseMap();
            CreateMap<Comentario, ComentarioDto>().ReverseMap();
            CreateMap<AsignarActorPeliculaDto, ActorPelicula>().ReverseMap();
        }
    }
}
using AutoMapper;
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
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
        }
    }
}
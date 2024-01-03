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
        }
    }
}
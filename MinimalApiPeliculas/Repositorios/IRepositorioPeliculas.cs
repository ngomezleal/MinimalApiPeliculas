﻿using MinimalApiPeliculas.Dtos;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task<int> Crear(Pelicula pelicula);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDto paginacionDto);
    }
}
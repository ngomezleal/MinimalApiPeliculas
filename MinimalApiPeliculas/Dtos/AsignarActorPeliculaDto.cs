﻿namespace MinimalApiPeliculas.Dtos
{
    public class AsignarActorPeliculaDto
    {
        public int ActorId { get; set; }
        public string Personaje { get; set; } = null!;
    }
}
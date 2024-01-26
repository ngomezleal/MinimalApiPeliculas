namespace MinimalApiPeliculas.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string? Poster { get; set; }
        public List<ComentarioDto> Comentarios { get; set; } = new List<ComentarioDto>();
        public List<GeneroDto> GeneroPeliculas { get; set; } = new List<GeneroDto>();
        public List<ActorPeliculaDto> ActoresPeliculas { get; set; } = new List<ActorPeliculaDto>();
    }
}
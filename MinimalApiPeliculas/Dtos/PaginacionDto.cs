namespace MinimalApiPeliculas.Dtos
{
    public class PaginacionDto
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina = 10;
        private readonly int cantidadMaxRecordsPorPagina = 50;

        public int RecordsPorPagina {
            get {
                return recordsPorPagina;
            }
            set {
                recordsPorPagina = (value > cantidadMaxRecordsPorPagina) ? cantidadMaxRecordsPorPagina : value;
            }
        }
    }
}
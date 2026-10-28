namespace VanderCake.Api.Models;

public class EventoAlteracao
{
    public long Id { get; set; }
    public string Tipo { get; set; } = "";
    public int? VendedorId { get; set; }
    public DateTime CriadoEm { get; set; }
}

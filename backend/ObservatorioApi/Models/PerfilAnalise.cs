namespace ObservatorioApi.Models;

public class PerfilAnalise
{
    public int Id { get; set; }
    public string Resumo { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
    public int DeputadoId { get; set; }

    public Deputado Deputado { get; set; } = null!;
}

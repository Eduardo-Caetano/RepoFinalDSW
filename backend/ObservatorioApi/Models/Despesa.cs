namespace ObservatorioApi.Models;

public class Despesa
{
    public int Id { get; set; }
    public string TipoDespesa { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateOnly DataDoc { get; set; }
    public int DeputadoId { get; set; }

    public Deputado Deputado { get; set; } = null!;
}

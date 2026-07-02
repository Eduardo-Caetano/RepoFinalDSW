namespace ObservatorioApi.Models;

public class Deputado
{
    public int Id { get; set; }
    public int IdCamara { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeEleitoral { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
    public string SiglaUf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UrlFoto { get; set; } = string.Empty;

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    public PerfilAnalise? PerfilAnalise { get; set; }
}

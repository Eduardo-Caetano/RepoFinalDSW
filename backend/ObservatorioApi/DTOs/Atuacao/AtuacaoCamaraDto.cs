namespace ObservatorioApi.DTOs.Atuacao;

public class AtuacaoCamaraDto
{
    public int PropostasAutoria { get; set; }
    public int PropostasRelatadas { get; set; }
    public int Votacoes { get; set; }
    public int Discursos { get; set; }
    public PresencaParlamentarDto Presencas { get; set; } = new();
    public List<string> ComissoesTitular { get; set; } = [];
    public List<string> ComissoesSuplente { get; set; } = [];
}

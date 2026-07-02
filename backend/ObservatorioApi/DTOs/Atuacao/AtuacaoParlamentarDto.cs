namespace ObservatorioApi.DTOs.Atuacao;

public class AtuacaoParlamentarDto
{
    public int Propostas { get; set; }
    public int PropostasAutoria { get; set; }
    public int PropostasRelatadas { get; set; }
    public int Votacoes { get; set; }
    public int Discursos { get; set; }
    public int PresencasPlenario { get; set; }
    public int AusenciasJustificadasPlenario { get; set; }
    public int AusenciasNaoJustificadasPlenario { get; set; }
    public int PresencasComissoes { get; set; }
    public int AusenciasJustificadasComissoes { get; set; }
    public int AusenciasNaoJustificadasComissoes { get; set; }
    public List<string> ComissoesTitular { get; set; } = [];
    public List<string> ComissoesSuplente { get; set; } = [];
}

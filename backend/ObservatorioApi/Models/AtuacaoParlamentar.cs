namespace ObservatorioApi.Models;

public class AtuacaoParlamentar
{
    public int Id { get; set; }
    public int DeputadoId { get; set; }
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
    public string ComissoesTitular { get; set; } = string.Empty;
    public string ComissoesSuplente { get; set; } = string.Empty;
    public DateTime DataAtualizacao { get; set; }

    public Deputado Deputado { get; set; } = null!;
}

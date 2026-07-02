namespace ObservatorioApi.DTOs.Atuacao;

public class PresencaParlamentarDto
{
    public int PresencasPlenario { get; set; }
    public int AusenciasJustificadasPlenario { get; set; }
    public int AusenciasNaoJustificadasPlenario { get; set; }
    public int PresencasComissoes { get; set; }
    public int AusenciasJustificadasComissoes { get; set; }
    public int AusenciasNaoJustificadasComissoes { get; set; }
}

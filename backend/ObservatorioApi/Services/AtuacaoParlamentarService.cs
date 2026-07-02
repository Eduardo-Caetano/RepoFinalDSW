using ObservatorioApi.DTOs.Atuacao;

namespace ObservatorioApi.Services;

public class AtuacaoParlamentarService
{
    private readonly PresencaScrapingService _presencaScrapingService;

    public AtuacaoParlamentarService(PresencaScrapingService presencaScrapingService)
    {
        _presencaScrapingService = presencaScrapingService;
    }

    public async Task<AtuacaoParlamentarDto> ObterAtuacaoAsync(int idCamara)
    {
        var atuacaoCamara = await _presencaScrapingService.ObterAtuacaoAsync(idCamara);
        var presencas = atuacaoCamara.Presencas;

        return new AtuacaoParlamentarDto
        {
            Propostas = atuacaoCamara.PropostasAutoria,
            PropostasAutoria = atuacaoCamara.PropostasAutoria,
            PropostasRelatadas = atuacaoCamara.PropostasRelatadas,
            Votacoes = atuacaoCamara.Votacoes,
            Discursos = atuacaoCamara.Discursos,
            PresencasPlenario = presencas.PresencasPlenario,
            AusenciasJustificadasPlenario = presencas.AusenciasJustificadasPlenario,
            AusenciasNaoJustificadasPlenario = presencas.AusenciasNaoJustificadasPlenario,
            PresencasComissoes = presencas.PresencasComissoes,
            AusenciasJustificadasComissoes = presencas.AusenciasJustificadasComissoes,
            AusenciasNaoJustificadasComissoes = presencas.AusenciasNaoJustificadasComissoes,
            ComissoesTitular = atuacaoCamara.ComissoesTitular,
            ComissoesSuplente = atuacaoCamara.ComissoesSuplente
        };
    }
}

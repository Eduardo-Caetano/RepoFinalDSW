using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Data;
using ObservatorioApi.DTOs.Atuacao;
using ObservatorioApi.Models;

namespace ObservatorioApi.Services;

public class ImportacaoAtuacaoService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _context;
    private readonly PresencaScrapingService _presencaScrapingService;

    public ImportacaoAtuacaoService(AppDbContext context, PresencaScrapingService presencaScrapingService)
    {
        _context = context;
        _presencaScrapingService = presencaScrapingService;
    }

    public async Task<int> ImportarAtuacoesAsync()
    {
        var deputados = await _context.Deputados
            .Include(deputado => deputado.AtuacaoParlamentar)
            .OrderBy(deputado => deputado.Id)
            .ToListAsync();

        foreach (var deputado in deputados)
        {
            var atuacaoCamara = await _presencaScrapingService.ObterAtuacaoAsync(deputado.IdCamara);

            if (deputado.AtuacaoParlamentar is null)
            {
                deputado.AtuacaoParlamentar = new AtuacaoParlamentar
                {
                    DeputadoId = deputado.Id
                };

                _context.AtuacoesParlamentares.Add(deputado.AtuacaoParlamentar);
            }

            AtualizarAtuacao(deputado.AtuacaoParlamentar, atuacaoCamara);
        }

        await _context.SaveChangesAsync();

        return deputados.Count;
    }

    private static void AtualizarAtuacao(AtuacaoParlamentar atuacaoParlamentar, AtuacaoCamaraDto atuacaoCamara)
    {
        var presencas = atuacaoCamara.Presencas;

        atuacaoParlamentar.Propostas = atuacaoCamara.PropostasAutoria;
        atuacaoParlamentar.PropostasAutoria = atuacaoCamara.PropostasAutoria;
        atuacaoParlamentar.PropostasRelatadas = atuacaoCamara.PropostasRelatadas;
        atuacaoParlamentar.Votacoes = atuacaoCamara.Votacoes;
        atuacaoParlamentar.Discursos = atuacaoCamara.Discursos;
        atuacaoParlamentar.PresencasPlenario = presencas.PresencasPlenario;
        atuacaoParlamentar.AusenciasJustificadasPlenario = presencas.AusenciasJustificadasPlenario;
        atuacaoParlamentar.AusenciasNaoJustificadasPlenario = presencas.AusenciasNaoJustificadasPlenario;
        atuacaoParlamentar.PresencasComissoes = presencas.PresencasComissoes;
        atuacaoParlamentar.AusenciasJustificadasComissoes = presencas.AusenciasJustificadasComissoes;
        atuacaoParlamentar.AusenciasNaoJustificadasComissoes = presencas.AusenciasNaoJustificadasComissoes;
        atuacaoParlamentar.ComissoesTitular = JsonSerializer.Serialize(atuacaoCamara.ComissoesTitular, JsonOptions);
        atuacaoParlamentar.ComissoesSuplente = JsonSerializer.Serialize(atuacaoCamara.ComissoesSuplente, JsonOptions);
        atuacaoParlamentar.DataAtualizacao = DateTime.UtcNow;
    }
}

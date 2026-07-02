using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Data;
using ObservatorioApi.DTOs.Atuacao;
using ObservatorioApi.Models;
using ObservatorioApi.Services;
using System.Text.Json;

namespace ObservatorioApi.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private const int AnoAtuacaoPersistida = 2026;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _context;
    private readonly AtuacaoParlamentarService _atuacaoParlamentarService;

    public DashboardController(AppDbContext context, AtuacaoParlamentarService atuacaoParlamentarService)
    {
        _context = context;
        _atuacaoParlamentarService = atuacaoParlamentarService;
    }

    [HttpGet("atuacao/{id:int}")]
    public async Task<IActionResult> ObterAtuacaoParlamentar(int id, [FromQuery] int? ano)
    {
        var deputado = await _context.Deputados
            .AsNoTracking()
            .Include(deputado => deputado.AtuacaoParlamentar)
            .FirstOrDefaultAsync(deputado => deputado.Id == id);

        if (deputado is null)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        if (ano is not null && ano != AnoAtuacaoPersistida)
        {
            return Ok(new AtuacaoParlamentarDto());
        }

        var atuacao = deputado.AtuacaoParlamentar is not null
            ? CriarDto(deputado.AtuacaoParlamentar)
            : await _atuacaoParlamentarService.ObterAtuacaoAsync(deputado.IdCamara);

        return Ok(atuacao);
    }

    private static AtuacaoParlamentarDto CriarDto(AtuacaoParlamentar atuacaoParlamentar)
    {
        return new AtuacaoParlamentarDto
        {
            Propostas = atuacaoParlamentar.Propostas,
            PropostasAutoria = atuacaoParlamentar.PropostasAutoria,
            PropostasRelatadas = atuacaoParlamentar.PropostasRelatadas,
            Votacoes = atuacaoParlamentar.Votacoes,
            Discursos = atuacaoParlamentar.Discursos,
            PresencasPlenario = atuacaoParlamentar.PresencasPlenario,
            AusenciasJustificadasPlenario = atuacaoParlamentar.AusenciasJustificadasPlenario,
            AusenciasNaoJustificadasPlenario = atuacaoParlamentar.AusenciasNaoJustificadasPlenario,
            PresencasComissoes = atuacaoParlamentar.PresencasComissoes,
            AusenciasJustificadasComissoes = atuacaoParlamentar.AusenciasJustificadasComissoes,
            AusenciasNaoJustificadasComissoes = atuacaoParlamentar.AusenciasNaoJustificadasComissoes,
            ComissoesTitular = DesserializarLista(atuacaoParlamentar.ComissoesTitular),
            ComissoesSuplente = DesserializarLista(atuacaoParlamentar.ComissoesSuplente)
        };
    }

    private static List<string> DesserializarLista(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(texto, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }
}

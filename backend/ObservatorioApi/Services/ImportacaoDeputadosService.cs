using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Data;
using ObservatorioApi.DTOs;
using ObservatorioApi.Models;

namespace ObservatorioApi.Services;

public class ImportacaoDeputadosService
{
    private const string DeputadosScUrl = "https://dadosabertos.camara.leg.br/api/v2/deputados?siglaUf=SC";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;

    public ImportacaoDeputadosService(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public async Task<int> ImportarDeputadosScAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetFromJsonAsync<CamaraDeputadosResponse>(
            DeputadosScUrl,
            JsonOptions);

        var deputadosCamara = response?.Dados ?? new List<CamaraDeputadoDto>();
        if (deputadosCamara.Count == 0)
        {
            return 0;
        }

        var idsCamara = deputadosCamara
            .Select(deputado => deputado.Id)
            .Distinct()
            .ToList();

        var idsExistentes = await _context.Deputados
            .Where(deputado => idsCamara.Contains(deputado.IdCamara))
            .Select(deputado => deputado.IdCamara)
            .ToListAsync();

        var idsExistentesSet = idsExistentes.ToHashSet();

        var novosDeputados = deputadosCamara
            .Where(deputado => idsExistentesSet.Add(deputado.Id))
            .Select(deputado => new Deputado
            {
                IdCamara = deputado.Id,
                Nome = deputado.Nome,
                NomeEleitoral = deputado.NomeEleitoral,
                SiglaPartido = deputado.SiglaPartido,
                SiglaUf = deputado.SiglaUf,
                Email = deputado.Email,
                UrlFoto = deputado.UrlFoto
            })
            .ToList();

        if (novosDeputados.Count == 0)
        {
            return 0;
        }

        _context.Deputados.AddRange(novosDeputados);
        await _context.SaveChangesAsync();

        return novosDeputados.Count;
    }

    private class CamaraDeputadosResponse
    {
        [JsonPropertyName("dados")]
        public List<CamaraDeputadoDto> Dados { get; set; } = [];
    }
}

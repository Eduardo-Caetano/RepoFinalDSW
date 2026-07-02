using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Data;
using ObservatorioApi.DTOs;
using ObservatorioApi.Models;

namespace ObservatorioApi.Services;

public class ImportacaoDespesasService
{
    private const int AnoImportacao = 2026;
    private const int ItensPorPagina = 100;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;

    public ImportacaoDespesasService(IHttpClientFactory httpClientFactory, AppDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public async Task<int> ImportarDespesasAsync()
    {
        var deputados = await _context.Deputados
            .AsNoTracking()
            .OrderBy(deputado => deputado.Id)
            .ToListAsync();

        if (deputados.Count == 0)
        {
            return 0;
        }

        var httpClient = _httpClientFactory.CreateClient();
        var novasDespesas = new List<Despesa>();

        foreach (var deputado in deputados)
        {
            var despesasCamara = await BuscarDespesasDeputadoAsync(httpClient, deputado.IdCamara);
            if (despesasCamara.Count == 0)
            {
                continue;
            }

            var despesasExistentes = await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.DeputadoId == deputado.Id)
                .Select(despesa => new DespesaImportadaKey(
                    despesa.DeputadoId,
                    despesa.TipoDespesa,
                    despesa.Fornecedor,
                    despesa.Valor,
                    despesa.DataDoc))
                .ToListAsync();

            var chavesExistentes = despesasExistentes.ToHashSet();

            foreach (var despesaCamara in despesasCamara)
            {
                if (despesaCamara.DataDocumento is null)
                {
                    continue;
                }

                var dataDoc = DateOnly.FromDateTime(despesaCamara.DataDocumento.Value);
                var tipoDespesa = despesaCamara.TipoDespesa.Trim();
                var fornecedor = despesaCamara.Fornecedor.Trim();
                var valor = despesaCamara.ValorDocumento;
                var chave = new DespesaImportadaKey(deputado.Id, tipoDespesa, fornecedor, valor, dataDoc);

                if (!chavesExistentes.Add(chave))
                {
                    continue;
                }

                novasDespesas.Add(new Despesa
                {
                    DeputadoId = deputado.Id,
                    TipoDespesa = tipoDespesa,
                    Fornecedor = fornecedor,
                    Valor = valor,
                    DataDoc = dataDoc
                });
            }
        }

        if (novasDespesas.Count == 0)
        {
            return 0;
        }

        _context.Despesas.AddRange(novasDespesas);
        await _context.SaveChangesAsync();

        return novasDespesas.Count;
    }

    private static async Task<List<CamaraDespesaDto>> BuscarDespesasDeputadoAsync(HttpClient httpClient, int idCamara)
    {
        var despesas = new List<CamaraDespesaDto>();
        var pagina = 1;

        while (true)
        {
            var url = $"https://dadosabertos.camara.leg.br/api/v2/deputados/{idCamara}/despesas?ano={AnoImportacao}&pagina={pagina}&itens={ItensPorPagina}";
            var response = await httpClient.GetFromJsonAsync<CamaraDespesasResponse>(url, JsonOptions);
            var dados = response?.Dados ?? new List<CamaraDespesaDto>();

            if (dados.Count == 0)
            {
                break;
            }

            despesas.AddRange(dados);

            if (dados.Count < ItensPorPagina)
            {
                break;
            }

            pagina++;
        }

        return despesas;
    }

    private readonly record struct DespesaImportadaKey(
        int DeputadoId,
        string TipoDespesa,
        string Fornecedor,
        decimal Valor,
        DateOnly DataDoc);

    private class CamaraDespesasResponse
    {
        [JsonPropertyName("dados")]
        public List<CamaraDespesaDto> Dados { get; set; } = [];
    }
}

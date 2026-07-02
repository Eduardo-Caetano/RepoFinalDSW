using Microsoft.AspNetCore.Mvc;
using ObservatorioApi.Services;

namespace ObservatorioApi.Controllers;

[ApiController]
[Route("api/importacao")]
public class ImportacaoController : ControllerBase
{
    private readonly ImportacaoDeputadosService _importacaoDeputadosService;
    private readonly ImportacaoDespesasService _importacaoDespesasService;

    public ImportacaoController(
        ImportacaoDeputadosService importacaoDeputadosService,
        ImportacaoDespesasService importacaoDespesasService)
    {
        _importacaoDeputadosService = importacaoDeputadosService;
        _importacaoDespesasService = importacaoDespesasService;
    }

    [HttpPost("deputados")]
    public async Task<IActionResult> ImportarDeputados()
    {
        var quantidadeImportada = await _importacaoDeputadosService.ImportarDeputadosScAsync();

        if (quantidadeImportada == 0)
        {
            return Ok(new { mensagem = "Nenhum novo deputado encontrado" });
        }

        return Ok(new { mensagem = $"{quantidadeImportada} deputados importados com sucesso" });
    }

    [HttpPost("despesas")]
    public async Task<IActionResult> ImportarDespesas()
    {
        var quantidadeImportada = await _importacaoDespesasService.ImportarDespesasAsync();

        return Ok(new { mensagem = $"{quantidadeImportada} despesas importadas com sucesso" });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObservatorioApi.Data;
using ObservatorioApi.DTOs;
using ObservatorioApi.Models;

namespace ObservatorioApi.Controllers;

[ApiController]
[Route("api/deputados")]
public class DeputadosController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeputadosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? partido,
        [FromQuery] string? uf)
    {
        var query = _context.Deputados
            .AsNoTracking()
            .Include(deputado => deputado.Despesas)
            .Include(deputado => deputado.PerfilAnalise)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(deputado => EF.Functions.ILike(deputado.Nome, $"%{nome}%"));
        }

        if (!string.IsNullOrWhiteSpace(partido))
        {
            query = query.Where(deputado => EF.Functions.ILike(deputado.SiglaPartido, partido));
        }

        if (!string.IsNullOrWhiteSpace(uf))
        {
            query = query.Where(deputado => EF.Functions.ILike(deputado.SiglaUf, uf));
        }

        var deputados = await query
            .OrderBy(deputado => deputado.Nome)
            .ToListAsync();

        return Ok(deputados.Select(MapearDeputado));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var deputado = await _context.Deputados
            .AsNoTracking()
            .Include(deputado => deputado.Despesas)
            .Include(deputado => deputado.PerfilAnalise)
            .FirstOrDefaultAsync(deputado => deputado.Id == id);

        if (deputado is null)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        return Ok(MapearDeputado(deputado));
    }

    [HttpGet("{id:int}/despesas")]
    public async Task<IActionResult> ListarDespesas(int id)
    {
        var deputadoExiste = await _context.Deputados.AnyAsync(deputado => deputado.Id == id);
        if (!deputadoExiste)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        var despesas = await _context.Despesas
            .AsNoTracking()
            .Where(despesa => despesa.DeputadoId == id)
            .OrderByDescending(despesa => despesa.DataDoc)
            .Select(despesa => new
            {
                despesa.Id,
                despesa.TipoDespesa,
                despesa.Fornecedor,
                despesa.Valor,
                despesa.DataDoc
            })
            .ToListAsync();

        return Ok(despesas);
    }

    [HttpGet("{id:int}/dashboard-financeiro")]
    public async Task<IActionResult> ObterDashboardFinanceiro(int id)
    {
        var deputadoExiste = await _context.Deputados.AnyAsync(deputado => deputado.Id == id);
        if (!deputadoExiste)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        var despesasQuery = _context.Despesas
            .AsNoTracking()
            .Where(despesa => despesa.DeputadoId == id);

        var quantidadeDespesas = await despesasQuery.CountAsync();
        var totalGasto = await despesasQuery.SumAsync(despesa => (decimal?)despesa.Valor) ?? 0;
        var maiorDespesa = await despesasQuery.MaxAsync(despesa => (decimal?)despesa.Valor) ?? 0;

        var gastosPorMesAgrupados = await despesasQuery
            .GroupBy(despesa => despesa.DataDoc.Month)
            .Select(grupo => new
            {
                Mes = grupo.Key,
                Valor = grupo.Sum(despesa => despesa.Valor)
            })
            .ToListAsync();

        var gastosPorMesMap = gastosPorMesAgrupados.ToDictionary(item => item.Mes, item => item.Valor);
        var nomesMeses = new[] { "JAN", "FEV", "MAR", "ABR", "MAI", "JUN", "JUL", "AGO", "SET", "OUT", "NOV", "DEZ" };

        var gastosPorMes = Enumerable.Range(1, 12)
            .Select(mes => new GastoPorMesDto
            {
                Mes = nomesMeses[mes - 1],
                Valor = gastosPorMesMap.GetValueOrDefault(mes)
            })
            .ToList();

        var gastosPorCategoria = await despesasQuery
            .GroupBy(despesa => despesa.TipoDespesa)
            .Select(grupo => new GastoPorCategoriaDto
            {
                Categoria = grupo.Key,
                Valor = grupo.Sum(despesa => despesa.Valor)
            })
            .OrderByDescending(item => item.Valor)
            .ToListAsync();

        return Ok(new DashboardFinanceiroDto
        {
            TotalGasto = totalGasto,
            QuantidadeDespesas = quantidadeDespesas,
            MaiorDespesa = maiorDespesa,
            GastosPorMes = gastosPorMes,
            GastosPorCategoria = gastosPorCategoria
        });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] DeputadoRequest? request)
    {
        if (request is null)
        {
            return BadRequest(new { erro = "Dados do deputado nao informados" });
        }

        if (request.IdCamara <= 0)
        {
            return BadRequest(new { erro = "IdCamara deve ser informado" });
        }

        var idCamaraExiste = await _context.Deputados
            .AnyAsync(deputado => deputado.IdCamara == request.IdCamara);

        if (idCamaraExiste)
        {
            return BadRequest(new { erro = "Ja existe um deputado cadastrado com este IdCamara" });
        }

        var deputado = new Deputado
        {
            IdCamara = request.IdCamara,
            Nome = request.Nome,
            NomeEleitoral = request.NomeEleitoral,
            SiglaPartido = request.SiglaPartido,
            SiglaUf = request.SiglaUf,
            Email = request.Email,
            UrlFoto = request.UrlFoto
        };

        _context.Deputados.Add(deputado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = deputado.Id }, MapearDeputado(deputado));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] DeputadoRequest? request)
    {
        if (request is null)
        {
            return BadRequest(new { erro = "Dados do deputado nao informados" });
        }

        if (request.IdCamara <= 0)
        {
            return BadRequest(new { erro = "IdCamara deve ser informado" });
        }

        var deputado = await _context.Deputados.FirstOrDefaultAsync(deputado => deputado.Id == id);
        if (deputado is null)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        var idCamaraExiste = await _context.Deputados
            .AnyAsync(deputadoExistente =>
                deputadoExistente.Id != id &&
                deputadoExistente.IdCamara == request.IdCamara);

        if (idCamaraExiste)
        {
            return BadRequest(new { erro = "Ja existe outro deputado cadastrado com este IdCamara" });
        }

        deputado.IdCamara = request.IdCamara;
        deputado.Nome = request.Nome;
        deputado.NomeEleitoral = request.NomeEleitoral;
        deputado.SiglaPartido = request.SiglaPartido;
        deputado.SiglaUf = request.SiglaUf;
        deputado.Email = request.Email;
        deputado.UrlFoto = request.UrlFoto;

        await _context.SaveChangesAsync();

        return Ok(MapearDeputado(deputado));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var deputado = await _context.Deputados.FirstOrDefaultAsync(deputado => deputado.Id == id);
        if (deputado is null)
        {
            return NotFound(new { erro = "Deputado nao encontrado" });
        }

        _context.Deputados.Remove(deputado);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Deputado removido com sucesso" });
    }

    private static object MapearDeputado(Deputado deputado)
    {
        return new
        {
            deputado.Id,
            deputado.IdCamara,
            deputado.Nome,
            deputado.NomeEleitoral,
            deputado.SiglaPartido,
            deputado.SiglaUf,
            deputado.Email,
            deputado.UrlFoto,
            Despesas = deputado.Despesas.Select(despesa => new
            {
                despesa.Id,
                despesa.TipoDespesa,
                despesa.Fornecedor,
                despesa.Valor,
                despesa.DataDoc,
                despesa.DeputadoId
            }),
            PerfilAnalise = deputado.PerfilAnalise is null
                ? null
                : new
                {
                    deputado.PerfilAnalise.Id,
                    deputado.PerfilAnalise.Resumo,
                    deputado.PerfilAnalise.Observacoes,
                    deputado.PerfilAnalise.DeputadoId
                }
        };
    }
}

public class DeputadoRequest
{
    public int IdCamara { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeEleitoral { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
    public string SiglaUf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UrlFoto { get; set; } = string.Empty;
}

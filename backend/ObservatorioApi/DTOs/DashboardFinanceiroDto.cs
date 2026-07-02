namespace ObservatorioApi.DTOs;

public class DashboardFinanceiroDto
{
    public decimal TotalGasto { get; set; }
    public int QuantidadeDespesas { get; set; }
    public decimal MaiorDespesa { get; set; }
    public List<GastoPorMesDto> GastosPorMes { get; set; } = [];
    public List<GastoPorCategoriaDto> GastosPorCategoria { get; set; } = [];
}

public class GastoPorMesDto
{
    public string Mes { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class GastoPorCategoriaDto
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

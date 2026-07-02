using System.Text.Json.Serialization;

namespace ObservatorioApi.DTOs;

public class CamaraDespesaDto
{
    [JsonPropertyName("tipoDespesa")]
    public string TipoDespesa { get; set; } = string.Empty;

    [JsonPropertyName("fornecedor")]
    public string Fornecedor { get; set; } = string.Empty;

    [JsonPropertyName("valorDocumento")]
    public decimal ValorDocumento { get; set; }

    [JsonPropertyName("dataDocumento")]
    public DateTime? DataDocumento { get; set; }
}

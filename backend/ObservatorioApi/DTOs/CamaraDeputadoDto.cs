using System.Text.Json.Serialization;

namespace ObservatorioApi.DTOs;

public class CamaraDeputadoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("nomeEleitoral")]
    public string NomeEleitoral { get; set; } = string.Empty;

    [JsonPropertyName("siglaPartido")]
    public string SiglaPartido { get; set; } = string.Empty;

    [JsonPropertyName("siglaUf")]
    public string SiglaUf { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("urlFoto")]
    public string UrlFoto { get; set; } = string.Empty;
}

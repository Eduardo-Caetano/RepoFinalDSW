using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ObservatorioApi.DTOs.Atuacao;

namespace ObservatorioApi.Services;

public class PresencaScrapingService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PresencaScrapingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AtuacaoCamaraDto> ObterAtuacaoAsync(int idCamara)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ObservatorioApi/1.0");

        var atuacao = await BuscarAtuacaoNaUrlAsync(httpClient, $"https://www.camara.leg.br/deputados/{idCamara}");

        if (TemAlgumDado(atuacao.Presencas) || atuacao.PropostasAutoria > 0 || atuacao.PropostasRelatadas > 0)
        {
            return atuacao;
        }

        return await BuscarAtuacaoNaUrlAsync(httpClient, $"https://www.camara.leg.br/deputados/{idCamara}/atuacao");
    }

    public async Task<PresencaParlamentarDto> ObterPresencasAsync(int idCamara)
    {
        var atuacao = await ObterAtuacaoAsync(idCamara);
        return atuacao.Presencas;
    }

    private static async Task<AtuacaoCamaraDto> BuscarAtuacaoNaUrlAsync(HttpClient httpClient, string url)
    {
        try
        {
            var html = await httpClient.GetStringAsync(url);
            var documento = new HtmlDocument();
            documento.LoadHtml(html);

            var presencas = ExtrairPresencas(documento);

            return new AtuacaoCamaraDto
            {
                PropostasAutoria = ExtrairQuantidadeCard(documento, "propostas legislativas", "de sua autoria"),
                PropostasRelatadas = ExtrairQuantidadeCard(documento, "propostas legislativas", "relatadas"),
                Votacoes = ExtrairQuantidadeCard(documento, "votacoes nominais", "em plenario"),
                Discursos = ExtrairQuantidadeCard(documento, "discursos", "em plenario"),
                Presencas = presencas,
                ComissoesTitular = ExtrairComissoes(documento, "titular-comissoes__nome"),
                ComissoesSuplente = ExtrairComissoes(documento, "suplente-comissoes__nome")
            };
        }
        catch
        {
            return new AtuacaoCamaraDto();
        }
    }

    private static PresencaParlamentarDto ExtrairPresencas(HtmlDocument documento)
    {
        var presencas = new PresencaParlamentarDto();
        var secoes = documento.DocumentNode.SelectNodes("//section[contains(concat(' ', normalize-space(@class), ' '), ' presencas__section ') and not(@aria-hidden='true')]");

        if (secoes is null)
        {
            return presencas;
        }

        foreach (var secao in secoes)
        {
            var titulo = NormalizarTexto(secao.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' presencas__section-heading ')]")?.InnerText);

            if (string.IsNullOrWhiteSpace(titulo))
            {
                continue;
            }

            var ehPlenario = titulo.Contains("plenario", StringComparison.OrdinalIgnoreCase);
            var ehComissoes = titulo.Contains("comissoes", StringComparison.OrdinalIgnoreCase);

            if (!ehPlenario && !ehComissoes)
            {
                continue;
            }

            var dados = secao.SelectNodes(".//li[contains(concat(' ', normalize-space(@class), ' '), ' presencas__data ') and .//*[contains(concat(' ', normalize-space(@class), ' '), ' presencas__qtd ')]]");

            if (dados is null)
            {
                continue;
            }

            foreach (var dado in dados)
            {
                var label = NormalizarTexto(dado.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' presencas__label ')]")?.InnerText);
                var quantidadeTexto = dado.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' presencas__qtd ')]")?.InnerText;
                var quantidade = ExtrairNumeroInteiro(quantidadeTexto);

                // Se a Camara alterar os nomes/classes do HTML, o campo nao sera encontrado e permanecera 0.
                if (string.IsNullOrWhiteSpace(label))
                {
                    continue;
                }

                if (label.Contains("nao justificadas", StringComparison.OrdinalIgnoreCase))
                {
                    AplicarAusenciasNaoJustificadas(presencas, ehPlenario, quantidade);
                    continue;
                }

                if (label.Contains("justificadas", StringComparison.OrdinalIgnoreCase))
                {
                    AplicarAusenciasJustificadas(presencas, ehPlenario, quantidade);
                    continue;
                }

                if (label.Contains("presencas", StringComparison.OrdinalIgnoreCase))
                {
                    AplicarPresencas(presencas, ehPlenario, quantidade);
                }
            }
        }

        return presencas;
    }

    private static int ExtrairQuantidadeCard(HtmlDocument documento, string tituloCard, string tipo)
    {
        var cards = documento.DocumentNode.SelectNodes("//*[contains(concat(' ', normalize-space(@class), ' '), ' card--atuacao ')]");

        if (cards is null)
        {
            return 0;
        }

        foreach (var card in cards)
        {
            var titulo = NormalizarTexto(card.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' card-header__title ')]")?.InnerText);

            if (!titulo.Contains(tituloCard, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var itens = card.SelectNodes(".//*[contains(concat(' ', normalize-space(@class), ' '), ' atuacao__item ')]");

            if (itens is null)
            {
                continue;
            }

            foreach (var item in itens)
            {
                var label = NormalizarTexto(item.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' atuacao__tipo ')]")?.InnerText);

                if (!label.Contains(tipo, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return ExtrairNumeroInteiro(item.SelectSingleNode(".//*[contains(concat(' ', normalize-space(@class), ' '), ' atuacao__quantidade ')]")?.InnerText);
            }
        }

        return 0;
    }

    private static List<string> ExtrairComissoes(HtmlDocument documento, string classeNome)
    {
        var xpath = $"//div[contains(concat(' ', normalize-space(@class), ' '), ' titular-comissoes ')]//span[contains(concat(' ', normalize-space(@class), ' '), ' {classeNome} ') and not(ancestor::*[contains(concat(' ', normalize-space(@class), ' '), ' collapse ')])]";
        var nomes = documento.DocumentNode.SelectNodes(xpath);

        if (nomes is null)
        {
            return [];
        }

        return nomes
            .Select(no => WebUtility.HtmlDecode(no.InnerText).Trim())
            .Where(nome => !string.IsNullOrWhiteSpace(nome))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void AplicarPresencas(PresencaParlamentarDto presencas, bool ehPlenario, int quantidade)
    {
        if (ehPlenario)
        {
            presencas.PresencasPlenario = quantidade;
            return;
        }

        presencas.PresencasComissoes = quantidade;
    }

    private static void AplicarAusenciasJustificadas(PresencaParlamentarDto presencas, bool ehPlenario, int quantidade)
    {
        if (ehPlenario)
        {
            presencas.AusenciasJustificadasPlenario = quantidade;
            return;
        }

        presencas.AusenciasJustificadasComissoes = quantidade;
    }

    private static void AplicarAusenciasNaoJustificadas(PresencaParlamentarDto presencas, bool ehPlenario, int quantidade)
    {
        if (ehPlenario)
        {
            presencas.AusenciasNaoJustificadasPlenario = quantidade;
            return;
        }

        presencas.AusenciasNaoJustificadasComissoes = quantidade;
    }

    private static int ExtrairNumeroInteiro(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return 0;
        }

        var match = Regex.Match(WebUtility.HtmlDecode(texto), @"\d+");

        return match.Success && int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor)
            ? valor
            : 0;
    }

    private static string NormalizarTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        var textoDecodificado = WebUtility.HtmlDecode(texto).Replace('\u00a0', ' ').Trim();
        var normalizado = textoDecodificado.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var caractere in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(caractere);
            }
        }

        return Regex.Replace(builder.ToString(), @"\s+", " ").ToLowerInvariant();
    }

    private static bool TemAlgumDado(PresencaParlamentarDto presencas)
    {
        return presencas.PresencasPlenario > 0 ||
            presencas.AusenciasJustificadasPlenario > 0 ||
            presencas.AusenciasNaoJustificadasPlenario > 0 ||
            presencas.PresencasComissoes > 0 ||
            presencas.AusenciasJustificadasComissoes > 0 ||
            presencas.AusenciasNaoJustificadasComissoes > 0;
    }
}

import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { getDashboardFinanceiro } from '../api/dashboardApi.js'
import { getDeputadoPorId, getDespesasDeputado } from '../api/deputadoApi.js'

const formatadorMoeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const coresGrafico = ['#1f7a4d', '#4f6f52', '#2f80ed', '#9f6b22', '#6b7280', '#8a1f11', '#5b5f97', '#0f766e']

function prepararCategoriasGrafico(categorias = []) {
  const principais = categorias.slice(0, 5)
  const restante = categorias.slice(5)
  const valorOutros = restante.reduce((total, item) => total + item.valor, 0)

  if (valorOutros <= 0) {
    return principais
  }

  return [
    ...principais,
    {
      categoria: 'Outros',
      valor: valorOutros,
    },
  ]
}

function formatarMoedaAbreviada(valor) {
  if (valor >= 1000000) {
    return `R$ ${(valor / 1000000).toLocaleString('pt-BR', { maximumFractionDigits: 1 })} mi`
  }

  if (valor >= 1000) {
    return `R$ ${(valor / 1000).toLocaleString('pt-BR', { maximumFractionDigits: 0 })} mil`
  }

  return formatadorMoeda.format(valor)
}

function formatarData(data) {
  if (!data) {
    return 'Nao informada'
  }

  return new Intl.DateTimeFormat('pt-BR', { timeZone: 'UTC' }).format(new Date(data))
}

function DeputadoDetalhe() {
  const { id } = useParams()
  const [deputado, setDeputado] = useState(null)
  const [despesas, setDespesas] = useState([])
  const [dashboard, setDashboard] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  useEffect(() => {
    async function carregarDeputado() {
      try {
        const [dadosDeputado, dadosDespesas, dadosDashboard] = await Promise.all([
          getDeputadoPorId(id),
          getDespesasDeputado(id),
          getDashboardFinanceiro(id),
        ])

        setDeputado(dadosDeputado)
        setDespesas(dadosDespesas)
        setDashboard(dadosDashboard)
      } catch (error) {
        if (error.response?.status === 404) {
          setErro('Deputado nao encontrado.')
          return
        }

        setErro('Nao foi possivel carregar os dados do deputado.')
      } finally {
        setCarregando(false)
      }
    }

    carregarDeputado()
  }, [id])

  if (carregando) {
    return (
      <main className="page-shell detail-page">
        <Link className="back-link" to="/">
          Voltar
        </Link>
        <p className="status">Carregando deputado...</p>
      </main>
    )
  }

  if (erro) {
    return (
      <main className="page-shell detail-page">
        <Link className="back-link" to="/">
          Voltar
        </Link>
        <p className="status status-error">{erro}</p>
      </main>
    )
  }

  const nomeExibicao = deputado.nomeEleitoral || deputado.nome
  const camaraUrl = `https://www.camara.leg.br/deputados/${deputado.idCamara}`
  const gastosPorCategoria = prepararCategoriasGrafico(dashboard?.gastosPorCategoria)
  const gastosPorMes = dashboard?.gastosPorMes ?? []

  return (
    <main className="page-shell detail-page">
      <Link className="back-link" to="/">
        Voltar
      </Link>

      <section className="detail-card">
        <img
          className="detail-photo"
          src={deputado.urlFoto}
          alt={`Foto de ${nomeExibicao}`}
        />

        <div className="detail-content">
          <p className="eyebrow">Deputado federal</p>
          <h1>{nomeExibicao}</h1>

          <dl className="detail-list">
            <div>
              <dt>Nome</dt>
              <dd>{deputado.nome}</dd>
            </div>
            <div>
              <dt>Nome eleitoral</dt>
              <dd>{deputado.nomeEleitoral || 'Nao informado'}</dd>
            </div>
            <div>
              <dt>Partido</dt>
              <dd>{deputado.siglaPartido}</dd>
            </div>
            <div>
              <dt>UF</dt>
              <dd>{deputado.siglaUf}</dd>
            </div>
            <div>
              <dt>E-mail</dt>
              <dd>{deputado.email || 'Nao informado'}</dd>
            </div>
          </dl>

          <a className="details-button external-link" href={camaraUrl} target="_blank" rel="noreferrer">
            Pagina oficial da Camara
          </a>
        </div>
      </section>

      <section className="detail-sections">
        <article className="info-panel">
          <h2>Perfil de analise</h2>
          <p>A analise automatica deste parlamentar sera exibida futuramente.</p>
        </article>

        <article className="info-panel expenses-panel">
          <h2>Despesas parlamentares</h2>
          {despesas.length === 0 ? (
            <p>Nenhuma despesa cadastrada.</p>
          ) : (
            <div className="expenses-content">
              <section className="financial-dashboard">
                <div className="dashboard-header">
                  <h3>Dashboard Financeiro</h3>
                </div>

                <div className="metric-grid">
                  <div className="metric-card">
                    <span>Total gasto</span>
                    <strong>{formatadorMoeda.format(dashboard?.totalGasto ?? 0)}</strong>
                  </div>
                  <div className="metric-card">
                    <span>Quantidade de despesas</span>
                    <strong>{dashboard?.quantidadeDespesas ?? 0}</strong>
                  </div>
                  <div className="metric-card">
                    <span>Maior despesa</span>
                    <strong>{formatadorMoeda.format(dashboard?.maiorDespesa ?? 0)}</strong>
                  </div>
                </div>

                <div className="charts-grid">
                  <article className="chart-card">
                    <h4>Gastos por categoria</h4>
                    <div className="pie-chart-layout">
                      <div className="pie-chart-visual">
                        <ResponsiveContainer width="100%" height={340}>
                          <PieChart>
                            <Pie
                              data={gastosPorCategoria}
                              dataKey="valor"
                              nameKey="categoria"
                              innerRadius={72}
                              outerRadius={120}
                              paddingAngle={2}
                            >
                              {gastosPorCategoria.map((item, index) => (
                                <Cell key={item.categoria} fill={coresGrafico[index % coresGrafico.length]} />
                              ))}
                            </Pie>
                            <Tooltip formatter={(valor) => formatadorMoeda.format(valor)} />
                          </PieChart>
                        </ResponsiveContainer>
                      </div>

                      <ul className="pie-legend" aria-label="Legenda de gastos por categoria">
                        {gastosPorCategoria.map((item, index) => (
                          <li key={item.categoria}>
                            <span
                              className="pie-legend-color"
                              style={{ backgroundColor: coresGrafico[index % coresGrafico.length] }}
                            />
                            <span className="pie-legend-label" title={item.categoria}>
                              {item.categoria}
                            </span>
                          </li>
                        ))}
                      </ul>
                    </div>
                  </article>

                  <article className="chart-card">
                    <h4>Gastos por mes</h4>
                    <div className="bar-chart-scroll">
                      <div className="bar-chart-canvas">
                        <ResponsiveContainer width="100%" height={340}>
                          <BarChart data={gastosPorMes} barCategoryGap="28%" margin={{ top: 12, right: 18, left: 4, bottom: 4 }}>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} />
                            <XAxis dataKey="mes" interval={0} />
                            <YAxis tickFormatter={formatarMoedaAbreviada} width={78} />
                            <Tooltip formatter={(valor) => formatadorMoeda.format(valor)} />
                            <Bar dataKey="valor" fill="#1f7a4d" radius={[6, 6, 0, 0]} />
                          </BarChart>
                        </ResponsiveContainer>
                      </div>
                    </div>
                  </article>
                </div>
              </section>

              <div className="expenses-table-wrapper">
                <table className="expenses-table">
                  <thead>
                    <tr>
                      <th>Data</th>
                      <th>Tipo</th>
                      <th>Fornecedor</th>
                      <th>Valor</th>
                    </tr>
                  </thead>
                  <tbody>
                    {despesas.map((despesa) => (
                      <tr key={despesa.id}>
                        <td>{formatarData(despesa.dataDoc)}</td>
                        <td>{despesa.tipoDespesa}</td>
                        <td>{despesa.fornecedor}</td>
                        <td>{formatadorMoeda.format(despesa.valor)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </article>
      </section>
    </main>
  )
}

export default DeputadoDetalhe

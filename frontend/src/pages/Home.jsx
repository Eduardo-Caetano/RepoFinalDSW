import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getDeputados } from '../api/deputadoApi.js'

function Home() {
  const [deputados, setDeputados] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    async function carregarDeputados() {
      try {
        const dados = await getDeputados()
        setDeputados(dados)
      } catch {
        setErro('Nao foi possivel carregar os deputados.')
      } finally {
        setCarregando(false)
      }
    }

    carregarDeputados()
  }, [])

  return (
    <main className="page-shell">
      <header className="page-header">
        <div>
          <p className="eyebrow">Observatorio Parlamentar</p>
          <h1>Deputados Federais de Santa Catarina</h1>
        </div>
      </header>

      {carregando && <p className="status">Carregando deputados...</p>}
      {erro && <p className="status status-error">{erro}</p>}

      {!carregando && !erro && (
        <section className="deputados-grid" aria-label="Lista de deputados">
          {deputados.map((deputado) => (
            <article className="deputado-card" key={deputado.id}>
              <img
                className="deputado-foto"
                src={deputado.urlFoto}
                alt={`Foto de ${deputado.nomeEleitoral || deputado.nome}`}
              />

              <div className="deputado-info">
                <h2>{deputado.nomeEleitoral || deputado.nome}</h2>
                <p>{deputado.siglaPartido} - {deputado.siglaUf}</p>
              </div>

              <button
                className="details-button"
                type="button"
                onClick={() => navigate(`/deputado/${deputado.id}`)}
              >
                Ver detalhes
              </button>
            </article>
          ))}
        </section>
      )}
    </main>
  )
}

export default Home

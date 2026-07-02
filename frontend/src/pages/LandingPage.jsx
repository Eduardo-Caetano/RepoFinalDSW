import { Link } from 'react-router-dom'

const recursos = [
  'Deputados federais de SC',
  'Despesas parlamentares',
  'Atuação parlamentar',
  'Presença em plenário e comissões',
]

const sobreCards = [
  'Transparência pública',
  'Controle social',
  'Dados oficiais da Câmara',
]

const funcionalidadesFuturas = [
  'Explicação sobre o papel de deputados federais, estaduais e senadores',
  'Página educativa sobre o funcionamento do Legislativo',
  'Explicação simplificada de projetos de lei',
  'Comparações e rankings',
]

function LandingPage() {
  return (
    <main className="landing-page">
      <section className="landing-hero">
        <div className="landing-hero-content">
          <p className="eyebrow">Transparência pública</p>
          <h1>Observatório Parlamentar de Santa Catarina</h1>
          <p className="landing-subtitle">
            Um painel para acompanhar deputados federais catarinenses, reunindo despesas,
            presença, atuação parlamentar e informações públicas em uma experiência clara.
          </p>
          <p className="landing-text">
            O projeto aproxima dados abertos da Câmara dos Deputados da sociedade, fortalecendo
            o controle social e tornando a atividade parlamentar mais fácil de consultar.
          </p>

          <div className="landing-actions">
            <Link className="landing-button landing-button-primary" to="/deputados">
              Ver deputados federais de SC
            </Link>
            <a className="landing-button landing-button-secondary" href="#sobre">
              Sobre o projeto
            </a>
          </div>
        </div>
      </section>

      <section className="landing-section landing-about" id="sobre">
        <div className="landing-section-heading">
          <p className="eyebrow">Sobre</p>
          <h2>Sobre o projeto</h2>
          <p className="landing-section-text">
            O Observatório Parlamentar de Santa Catarina é um protótipo acadêmico voltado à
            organização e visualização de dados públicos sobre deputados federais catarinenses.
            A proposta é reunir, em uma única interface, informações como despesas parlamentares,
            presença em plenário, atuação em comissões e produção legislativa, tornando a consulta
            mais simples para cidadãos, estudantes e pesquisadores.
          </p>
        </div>

        <div className="landing-grid landing-grid-three">
          {sobreCards.map((card) => (
            <article className="landing-card" key={card}>
              <span className="landing-card-marker" />
              <h3>{card}</h3>
            </article>
          ))}
        </div>
      </section>

      <section className="landing-section">
        <div className="landing-section-heading">
          <p className="eyebrow">Monitoramento</p>
          <h2>O que o sistema mostra</h2>
        </div>

        <div className="landing-grid">
          {recursos.map((recurso) => (
            <article className="landing-card" key={recurso}>
              <span className="landing-card-marker" />
              <h3>{recurso}</h3>
            </article>
          ))}
        </div>
      </section>

      <section className="landing-section landing-section-muted">
        <div className="landing-section-heading">
          <p className="eyebrow">Próximas etapas</p>
          <h2>Funcionalidades futuras</h2>
        </div>

        <div className="future-list">
          {funcionalidadesFuturas.map((funcionalidade) => (
            <article className="future-item" key={funcionalidade}>
              <h3>{funcionalidade}</h3>
            </article>
          ))}
        </div>
      </section>
    </main>
  )
}

export default LandingPage

import { Link } from 'react-router-dom'

function PainelAdmin() {
  return (
    <main className="container">
      <section className="secao">
        <h1>Painel administrativo</h1>

        <p>
          Bem-vindo a area de administracao do RetroGames.
        </p>

        <div>
          <h2>Gerenciamento</h2>

          <p>
            Selecione uma opcao para administrar o sistema.
          </p>

          <ul>
            <li>
              <Link to="/admin/plataformas">
                Gerenciar plataformas
              </Link>
            </li>

            <li>
              <Link to="/admin/jogos">
                Gerenciar jogos
              </Link>
            </li>
          </ul>
        </div>
      </section>
    </main>
  )
}

export default PainelAdmin
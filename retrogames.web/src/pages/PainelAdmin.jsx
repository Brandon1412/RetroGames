import { Link } from 'react-router-dom'

function PainelAdmin() {
  return (
    <main className="container">
      <section className="secao">
        <h1>Painel administrativo</h1>

        <p>
          Bem-vindo à área de administração do RetroGames.
        </p>

        <div>
          <h2>Gerenciamento</h2>

          <p>
            Selecione uma opção para administrar o sistema.
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

            <li>
              <Link to="/admin/administradores">
                Gerenciar administradores
              </Link>
            </li>

            <li>
              <Link to="/admin/minha-senha">
                Alterar minha senha
              </Link>
            </li>
          </ul>
        </div>
      </section>
    </main>
  )
}

export default PainelAdmin

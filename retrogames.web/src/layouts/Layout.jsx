
import { Link, Outlet, useNavigate } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair,
  } = useAutenticacao()

  const navigate = useNavigate()

  async function handleSair() {
    try {
      await sair()
      navigate('/')
    } catch (erro) {
      console.error('Erro ao encerrar sessão:', erro)
    }
  }

  return (
    <div className="site">
      <header className="cabecalho">
        <Link className="marca" to="/" aria-label="RetroGames - Início">
          <span className="marca-icone">▣</span>
          <span>RETRO<span>GAMES</span></span>
        </Link>

        <nav className="navegacao">
          <Link to="/#inicio">Início</Link>
          <Link to="/#catalogo">Catálogo</Link>
        </nav>

        <div>
          <span className="status-loja">
            <span className="status-ponto"></span>
            LOJA ONLINE
          </span>

          <span style={{ marginLeft: '16px' }}>
            {carregando
              ? 'Verificando sessão...'
              : usuario
                ? `Olá, ${usuario.nome}`
                : 'Visitante'}
          </span>

          {!carregando && (
            <>
              {!estaAutenticado && (
                <Link
                  to="/login"
                  style={{ marginLeft: '16px' }}
                >
                  Entrar
                </Link>
              )}

              {estaAutenticado && ehAdministrador && (
                <Link
                  to="/admin"
                  style={{ marginLeft: '16px' }}
                >
                  Administração
                </Link>
              )}

              {estaAutenticado && (
                <button
                  type="button"
                  onClick={handleSair}
                  style={{ marginLeft: '16px', cursor: 'pointer' }}
                >
                  Sair
                </button>
              )}
            </>
          )}
        </div>
      </header>

      <Outlet />

      <footer className="rodape">
        <Link className="marca" to="/">
          <span className="marca-icone">▣</span>
          <span>RETRO<span>GAMES</span></span>
        </Link>

        <p>© 2026 RetroGames. Todos os direitos reservados.</p>

        <span>FEITO PARA QUEM AMA VIDEOGAMES</span>
      </footer>
    </div>
  )
}

export default Layout

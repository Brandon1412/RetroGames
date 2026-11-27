
import { Link, Outlet } from 'react-router-dom'

function Layout() {
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

        <span className="status-loja">
          <span className="status-ponto"></span>
          LOJA ONLINE
        </span>
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

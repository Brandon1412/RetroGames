
import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <main className="catalogo estado">
      <h1>404</h1>
      <h2>Página não encontrada</h2>
      <p>O endereço que você tentou acessar não existe.</p>

      <Link to="/" className="botao-banner">
        VOLTAR AO INÍCIO
      </Link>
    </main>
  )
}

export default NaoEncontrada

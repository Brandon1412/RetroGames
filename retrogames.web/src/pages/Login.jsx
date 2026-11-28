
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function Login() {
  const { entrar } = useAutenticacao()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()

    setErro('')
    setEnviando(true)

    try {
      await entrar(email, senha)
      navigate('/')
    } catch (error) {
      setErro(
        error.response?.data?.mensagem ||
        error.response?.data?.message ||
        'Não foi possível realizar o login. Confira seu e-mail e sua senha.'
      )
    } finally {
      setEnviando(false)
    }
  }

  return (
    <main className="login-pagina">
      <section className="login-container">
        <Link to="/" className="login-voltar">
          ← VOLTAR PARA A LOJA
        </Link>

        <div className="login-card">
          <div className="login-icone">▣</div>

          <span className="login-etiqueta">
            ÁREA ADMINISTRATIVA
          </span>

          <h1>Bem-vindo de volta!</h1>

          <p className="login-descricao">
            Entre com suas credenciais para acessar o RetroGames.
          </p>

          <form onSubmit={handleSubmit}>
            <div className="login-campo">
              <label htmlFor="email">E-MAIL</label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="seuemail@exemplo.com"
                autoComplete="username"
                required
              />
            </div>

            <div className="login-campo">
              <label htmlFor="senha">SENHA</label>
              <input
                id="senha"
                type="password"
                value={senha}
                onChange={(event) => setSenha(event.target.value)}
                placeholder="Digite sua senha"
                autoComplete="current-password"
                required
              />
            </div>

            {erro && (
              <div className="login-erro" role="alert">
                {erro}
              </div>
            )}

            <button
              className="login-botao"
              type="submit"
              disabled={enviando}
            >
              {enviando ? 'ENTRANDO...' : 'ENTRAR'}
            </button>
          </form>

          <p className="login-aviso">
            Acesso exclusivo para administradores.
          </p>
        </div>
      </section>
    </main>
  )
}

export default Login

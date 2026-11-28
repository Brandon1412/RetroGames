import { useState } from 'react'
import { Link } from 'react-router-dom'
import { administradorApi } from '../servicos/servicos'

function GerenciarAdministradores() {
  const [formulario, setFormulario] = useState({
    nome: '',
    email: '',
    senha: '',
  })

  const [salvando, setSalvando] = useState(false)
  const [mensagemErro, setMensagemErro] = useState('')
  const [mensagemSucesso, setMensagemSucesso] = useState('')

  const estiloCampo = {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    marginBottom: '14px',
  }

  const estiloInput = {
    width: '100%',
    maxWidth: '500px',
    boxSizing: 'border-box',
    padding: '10px',
  }

  function atualizarCampo(evento) {
    const { name, value } = evento.target

    setFormulario((anterior) => ({
      ...anterior,
      [name]: value,
    }))
  }

  async function cadastrar(evento) {
    evento.preventDefault()

    setMensagemErro('')
    setMensagemSucesso('')

    const nome = formulario.nome.trim()
    const email = formulario.email.trim()
    const senha = formulario.senha

    if (!nome) {
      setMensagemErro('Informe o nome do administrador.')
      return
    }

    if (!email) {
      setMensagemErro('Informe o e-mail do administrador.')
      return
    }

    if (senha.length < 8) {
      setMensagemErro('A senha deve possuir pelo menos 8 caracteres.')
      return
    }

    try {
      setSalvando(true)

      await administradorApi.cadastrar({
        nome,
        email,
        senha,
      })

      setFormulario({
        nome: '',
        email: '',
        senha: '',
      })

      setMensagemSucesso('Administrador cadastrado com sucesso.')
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.mensagem ||
        erro.response?.data?.message ||
        'Não foi possível cadastrar o administrador.'
      )
    } finally {
      setSalvando(false)
    }
  }

  return (
    <main className="container">
      <section className="secao">
        <p>
          <Link to="/admin">Voltar ao painel administrativo</Link>
        </p>

        <h1>Gerenciar administradores</h1>

        <p>
          Cadastre novas contas com acesso administrativo ao RetroGames.
        </p>

        {mensagemErro && (
          <p role="alert" style={{ color: '#dc3545', margin: '12px 0' }}>
            {mensagemErro}
          </p>
        )}

        {mensagemSucesso && (
          <p role="status" style={{ color: '#198754', margin: '12px 0' }}>
            {mensagemSucesso}
          </p>
        )}

        <h2>Cadastrar administrador</h2>

        <form onSubmit={cadastrar}>
          <div style={estiloCampo}>
            <label htmlFor="nome">Nome completo *</label>
            <input
              id="nome"
              name="nome"
              type="text"
              value={formulario.nome}
              onChange={atualizarCampo}
              maxLength={150}
              autoComplete="name"
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="email">E-mail *</label>
            <input
              id="email"
              name="email"
              type="email"
              value={formulario.email}
              onChange={atualizarCampo}
              maxLength={150}
              autoComplete="email"
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="senha">Senha inicial *</label>
            <input
              id="senha"
              name="senha"
              type="password"
              value={formulario.senha}
              onChange={atualizarCampo}
              minLength={8}
              autoComplete="new-password"
              required
              style={estiloInput}
            />
            <small>A senha deve possuir pelo menos 8 caracteres.</small>
          </div>

          <button type="submit" disabled={salvando}>
            {salvando ? 'Cadastrando...' : 'Cadastrar administrador'}
          </button>
        </form>
      </section>
    </main>
  )
}

export default GerenciarAdministradores

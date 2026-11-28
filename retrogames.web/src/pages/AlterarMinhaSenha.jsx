import { useState } from 'react'
import { Link } from 'react-router-dom'
import { autenticacaoApi } from '../servicos/servicos'

function AlterarMinhaSenha() {
  const [formulario, setFormulario] = useState({
    senhaAtual: '',
    novaSenha: '',
    confirmarSenha: '',
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

  async function alterarSenha(evento) {
    evento.preventDefault()

    setMensagemErro('')
    setMensagemSucesso('')

    if (!formulario.senhaAtual) {
      setMensagemErro('Informe sua senha atual.')
      return
    }

    if (formulario.novaSenha.length < 8) {
      setMensagemErro('A nova senha deve possuir pelo menos 8 caracteres.')
      return
    }

    if (formulario.novaSenha !== formulario.confirmarSenha) {
      setMensagemErro('A confirmação da nova senha não corresponde.')
      return
    }

    if (formulario.senhaAtual === formulario.novaSenha) {
      setMensagemErro('A nova senha deve ser diferente da senha atual.')
      return
    }

    try {
      setSalvando(true)

      await autenticacaoApi.alterarMinhaSenha({
        senhaAtual: formulario.senhaAtual,
        novaSenha: formulario.novaSenha,
      })

      setFormulario({
        senhaAtual: '',
        novaSenha: '',
        confirmarSenha: '',
      })

      setMensagemSucesso('Sua senha foi alterada com sucesso.')
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.mensagem ||
        erro.response?.data?.message ||
        'Não foi possível alterar sua senha.'
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

        <h1>Alterar minha senha</h1>

        <p>
          Por segurança, informe sua senha atual para cadastrar uma nova senha.
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

        <form onSubmit={alterarSenha}>
          <div style={estiloCampo}>
            <label htmlFor="senhaAtual">Senha atual *</label>
            <input
              id="senhaAtual"
              name="senhaAtual"
              type="password"
              value={formulario.senhaAtual}
              onChange={atualizarCampo}
              autoComplete="current-password"
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="novaSenha">Nova senha *</label>
            <input
              id="novaSenha"
              name="novaSenha"
              type="password"
              value={formulario.novaSenha}
              onChange={atualizarCampo}
              minLength={8}
              autoComplete="new-password"
              required
              style={estiloInput}
            />
            <small>A nova senha deve possuir pelo menos 8 caracteres.</small>
          </div>

          <div style={estiloCampo}>
            <label htmlFor="confirmarSenha">Confirmar nova senha *</label>
            <input
              id="confirmarSenha"
              name="confirmarSenha"
              type="password"
              value={formulario.confirmarSenha}
              onChange={atualizarCampo}
              minLength={8}
              autoComplete="new-password"
              required
              style={estiloInput}
            />
          </div>

          <button type="submit" disabled={salvando}>
            {salvando ? 'Alterando senha...' : 'Alterar minha senha'}
          </button>
        </form>
      </section>
    </main>
  )
}

export default AlterarMinhaSenha

import { useCallback, useEffect, useState } from 'react'
import AutenticacaoContext from './autenticacaoContext'
import { autenticacaoApi } from '../servicos/servicos'

function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  const consultarUsuario = useCallback(async () => {
    try {
      const usuarioAtual = await autenticacaoApi.usuarioAtual()
      setUsuario(usuarioAtual)
    } catch (erro) {
      if (erro.response?.status === 401) {
        setUsuario(null)
      } else {
        console.error('Erro ao consultar usuário:', erro)
        setUsuario(null)
      }
    } finally {
      setCarregando(false)
    }
  }, [])

  useEffect(() => {
    consultarUsuario()

    function tratarSessaoExpirada() {
      setUsuario(null)
    }

    window.addEventListener('sessao:expirada', tratarSessaoExpirada)

    return () => {
      window.removeEventListener('sessao:expirada', tratarSessaoExpirada)
    }
  }, [consultarUsuario])

  const entrar = async (email, senha) => {
    const usuarioAutenticado = await autenticacaoApi.entrar({
      email,
      senha,
    })

    setUsuario(usuarioAutenticado)

    return usuarioAutenticado
  }

  const sair = async () => {
    try {
      await autenticacaoApi.sair()
    } finally {
      setUsuario(null)
    }
  }

  const estaAutenticado = Boolean(usuario)

  const ehAdministrador =
    usuario?.perfil?.toLowerCase() === 'administrador'

  return (
    <AutenticacaoContext.Provider
      value={{
        usuario,
        carregando,
        estaAutenticado,
        ehAdministrador,
        entrar,
        sair,
      }}
    >
      {children}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider
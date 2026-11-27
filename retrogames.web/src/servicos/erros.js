
export function obterMensagemErro(erro) {
  if (!erro?.response) {
    return 'Não foi possível conectar ao servidor. Verifique se a API está funcionando.'
  }

  return (
    erro.response.data?.message ||
    'Ocorreu um erro inesperado ao consultar os jogos.'
  )
}

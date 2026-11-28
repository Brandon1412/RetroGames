
import api from './api'

function extrairDados(resposta) {
  return resposta.data.data
}

export const jogoApi = {
  listar: (filtros = {}) =>
    api.get('/Jogo', { params: filtros }).then(extrairDados),

  buscarPorId: (id) =>
    api.get(`/Jogo/${id}`).then(extrairDados),
}

export const plataformaApi = {
  listar: () =>
    api.get('/Plataforma').then(extrairDados),

  buscarPorId: (id) =>
    api.get(`/Plataforma/${id}`).then(extrairDados),

  cadastrar: (dados) =>
    api.post('/Plataforma', dados).then(extrairDados),

  atualizar: (id, dados) =>
    api.put(`/Plataforma/${id}`, dados).then(extrairDados),

  excluir: (id) =>
    api.delete(`/Plataforma/${id}`).then(extrairDados),
}

export const autenticacaoApi = {
  entrar: (dados) =>
    api.post('/Autenticacao/login', dados).then((resposta) => resposta.data.usuario),

  sair: () =>
    api.post('/Autenticacao/logout'),

  usuarioAtual: () =>
    api.get('/Autenticacao/eu').then((resposta) => resposta.data.usuario),
}

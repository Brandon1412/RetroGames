
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
}

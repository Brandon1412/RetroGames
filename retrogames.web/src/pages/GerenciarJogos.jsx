import { useEffect, useState } from 'react'
import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi,
} from '../servicos/servicos'

function GerenciarJogos() {
  const formularioInicial = {
    nome: '',
    descricao: '',
    anoLancamento: '',
    preco: '',
    estoque: '0',
    idPlataforma: '',
    idCondicaoJogo: '',
    idsGeneros: [],
    ativo: true,
  }

  const filtrosIniciais = {
    nome: '',
    plataforma: '',
    condicao: '',
    genero: '',
    ordenacao: 'id-asc',
  }

  const [jogos, setJogos] = useState([])
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])
  const [formulario, setFormulario] = useState(formularioInicial)
  const [filtros, setFiltros] = useState(filtrosIniciais)
  const [idEdicao, setIdEdicao] = useState(null)
  const [carregando, setCarregando] = useState(true)
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
    maxWidth: '100%',
    boxSizing: 'border-box',
    padding: '10px',
  }

  const estiloCelula = {
    padding: '12px 10px',
    textAlign: 'left',
    verticalAlign: 'top',
    overflowWrap: 'anywhere',
  }

  const estiloFiltro = {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
    minWidth: '180px',
    flex: '1 1 180px',
  }

  async function carregarDados() {
    try {
      setCarregando(true)
      setMensagemErro('')

      const [
        listaJogos,
        listaPlataformas,
        listaGeneros,
        listaCondicoes,
      ] = await Promise.all([
        jogoApi.listar(),
        plataformaApi.listar(),
        generoApi.listar(),
        condicaoJogoApi.listar(),
      ])

      setJogos(listaJogos || [])
      setPlataformas(listaPlataformas || [])
      setGeneros(listaGeneros || [])
      setCondicoes(listaCondicoes || [])
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível carregar os dados dos jogos.'
      )
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarDados()
  }, [])

  function atualizarCampo(evento) {
    const { name, value, type, checked } = evento.target

    setFormulario((anterior) => ({
      ...anterior,
      [name]: type === 'checkbox' ? checked : value,
    }))
  }

  function atualizarFiltro(evento) {
    const { name, value } = evento.target

    setFiltros((anterior) => ({
      ...anterior,
      [name]: value,
    }))
  }

  function alternarGenero(idGenero) {
    setFormulario((anterior) => {
      const selecionados = anterior.idsGeneros.includes(idGenero)
        ? anterior.idsGeneros.filter((id) => id !== idGenero)
        : [...anterior.idsGeneros, idGenero]

      return {
        ...anterior,
        idsGeneros: selecionados,
      }
    })
  }

  function limparFormulario(limparMensagens = true) {
    setFormulario({ ...formularioInicial, idsGeneros: [] })
    setIdEdicao(null)

    if (limparMensagens) {
      setMensagemErro('')
      setMensagemSucesso('')
    }
  }

  function iniciarEdicao(jogo) {
    setIdEdicao(jogo.id)

    setFormulario({
      nome: jogo.nome || '',
      descricao: jogo.descricao || '',
      anoLancamento: jogo.anoLancamento?.toString() || '',
      preco: jogo.preco?.toString() || '',
      estoque: jogo.estoque?.toString() ?? '0',
      idPlataforma: jogo.idPlataforma?.toString() || '',
      idCondicaoJogo: jogo.idCondicaoJogo?.toString() || '',
      idsGeneros: jogo.idsGeneros || [],
      ativo: jogo.ativo ?? true,
    })

    setMensagemErro('')
    setMensagemSucesso('')
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  async function salvar(evento) {
    evento.preventDefault()
    setMensagemErro('')
    setMensagemSucesso('')

    const estavaEditando = idEdicao !== null
    const anoLancamento = Number(formulario.anoLancamento)
    const preco = Number(formulario.preco)
    const estoque = Number(formulario.estoque)

    if (!formulario.nome.trim()) {
      setMensagemErro('Informe o nome do jogo.')
      return
    }

    if (!formulario.descricao.trim()) {
      setMensagemErro('Informe a descrição do jogo.')
      return
    }

    if (
      !formulario.anoLancamento ||
      !Number.isInteger(anoLancamento) ||
      anoLancamento < 1970 ||
      anoLancamento > 2100
    ) {
      setMensagemErro('Informe um ano de lançamento entre 1970 e 2100.')
      return
    }

    if (!Number.isFinite(preco) || preco <= 0) {
      setMensagemErro('Informe um preço maior que zero.')
      return
    }

    if (!Number.isInteger(estoque) || estoque < 0) {
      setMensagemErro('Informe um estoque igual ou maior que zero.')
      return
    }

    if (!formulario.idPlataforma || !formulario.idCondicaoJogo) {
      setMensagemErro('Selecione a plataforma e a condição do jogo.')
      return
    }

    if (formulario.idsGeneros.length === 0) {
      setMensagemErro('Selecione pelo menos um gênero.')
      return
    }

    const dados = {
      nome: formulario.nome.trim(),
      descricao: formulario.descricao.trim(),
      anoLancamento,
      preco,
      estoque,
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
      ativo: formulario.ativo,
    }

    try {
      setSalvando(true)

      if (estavaEditando) {
        await jogoApi.atualizar(idEdicao, dados)
      } else {
        await jogoApi.cadastrar(dados)
      }

      limparFormulario(false)
      await carregarDados()

      setMensagemSucesso(
        estavaEditando
          ? 'Jogo atualizado com sucesso.'
          : 'Jogo cadastrado com sucesso.'
      )
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível salvar o jogo.'
      )
    } finally {
      setSalvando(false)
    }
  }

  async function excluir(jogo) {
    const confirmar = window.confirm(
      `Deseja realmente excluir o jogo ${jogo.nome}?`
    )

    if (!confirmar) return

    setMensagemErro('')
    setMensagemSucesso('')

    try {
      await jogoApi.excluir(jogo.id)

      if (idEdicao === jogo.id) {
        limparFormulario(false)
      }

      await carregarDados()
      setMensagemSucesso('Jogo excluído com sucesso.')
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível excluir o jogo.'
      )
    }
  }

  function nomePlataforma(id) {
    return plataformas.find((p) => Number(p.id) === Number(id))?.nome || 'Não informado'
  }

  function nomeCondicao(id) {
    return condicoes.find((c) => Number(c.id) === Number(id))?.descricao || 'Não informado'
  }

  function nomesGeneros(ids = []) {
    return ids
      .map((id) => generos.find((g) => Number(g.id) === Number(id))?.nome)
      .filter(Boolean)
      .join(', ') || 'Não informado'
  }

  function formatarPreco(valor) {
    return Number(valor || 0).toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    })
  }

  function limparFiltros() {
    setFiltros({ ...filtrosIniciais })
  }

  const jogosFiltrados = jogos
    .filter((jogo) => {
      const nomeCorresponde = (jogo.nome || '')
        .toLocaleLowerCase('pt-BR')
        .includes(filtros.nome.trim().toLocaleLowerCase('pt-BR'))

      const plataformaCorresponde =
        !filtros.plataforma ||
        Number(jogo.idPlataforma) === Number(filtros.plataforma)

      const condicaoCorresponde =
        !filtros.condicao ||
        Number(jogo.idCondicaoJogo) === Number(filtros.condicao)

      const generoCorresponde =
        !filtros.genero ||
        (jogo.idsGeneros || []).some(
          (id) => Number(id) === Number(filtros.genero)
        )

      return (
        nomeCorresponde &&
        plataformaCorresponde &&
        condicaoCorresponde &&
        generoCorresponde
      )
    })
    .sort((a, b) => {
      switch (filtros.ordenacao) {
        case 'id-desc':
          return Number(b.id) - Number(a.id)

        case 'preco-asc':
          return Number(a.preco) - Number(b.preco)

        case 'preco-desc':
          return Number(b.preco) - Number(a.preco)

        case 'data-asc':
          return new Date(a.dataCadastro || 0).getTime() -
            new Date(b.dataCadastro || 0).getTime()

        case 'data-desc':
          return new Date(b.dataCadastro || 0).getTime() -
            new Date(a.dataCadastro || 0).getTime()

        case 'estoque-asc':
          return Number(a.estoque) - Number(b.estoque)

        case 'estoque-desc':
          return Number(b.estoque) - Number(a.estoque)

        case 'nome-asc':
          return (a.nome || '').localeCompare(b.nome || '', 'pt-BR')

        case 'nome-desc':
          return (b.nome || '').localeCompare(a.nome || '', 'pt-BR')

        case 'id-asc':
        default:
          return Number(a.id) - Number(b.id)
      }
    })

  return (
    <main className="container">
      <section className="secao">
        <h1>Gerenciar jogos</h1>

        <p>Cadastre, consulte, edite e exclua jogos do catálogo.</p>

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

        <h2>{idEdicao !== null ? 'Editar jogo' : 'Cadastrar jogo'}</h2>

        <form onSubmit={salvar}>
          <div style={estiloCampo}>
            <label htmlFor="nome">Nome *</label>
            <input
              id="nome"
              name="nome"
              value={formulario.nome}
              onChange={atualizarCampo}
              maxLength={150}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="descricao">Descrição *</label>
            <textarea
              id="descricao"
              name="descricao"
              value={formulario.descricao}
              onChange={atualizarCampo}
              rows={3}
              maxLength={1000}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="anoLancamento">Ano de lançamento *</label>
            <input
              id="anoLancamento"
              name="anoLancamento"
              type="number"
              min="1970"
              max="2100"
              value={formulario.anoLancamento}
              onChange={atualizarCampo}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="preco">Preço (R$) *</label>
            <input
              id="preco"
              name="preco"
              type="number"
              min="0.01"
              step="0.01"
              value={formulario.preco}
              onChange={atualizarCampo}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="estoque">Estoque *</label>
            <input
              id="estoque"
              name="estoque"
              type="number"
              min="0"
              step="1"
              value={formulario.estoque}
              onChange={atualizarCampo}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="idPlataforma">Plataforma *</label>
            <select
              id="idPlataforma"
              name="idPlataforma"
              value={formulario.idPlataforma}
              onChange={atualizarCampo}
              required
              style={estiloInput}
            >
              <option value="">Selecione uma plataforma</option>
              {plataformas.filter((p) => p.ativo).map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloCampo}>
            <label htmlFor="idCondicaoJogo">Condição *</label>
            <select
              id="idCondicaoJogo"
              name="idCondicaoJogo"
              value={formulario.idCondicaoJogo}
              onChange={atualizarCampo}
              required
              style={estiloInput}
            >
              <option value="">Selecione uma condição</option>
              {condicoes.map((condicao) => (
                <option key={condicao.id} value={condicao.id}>
                  {condicao.descricao}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloCampo}>
            <label>Gêneros * (selecione pelo menos um)</label>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '12px' }}>
              {generos.map((genero) => (
                <label key={genero.id} style={{ display: 'flex', gap: '5px' }}>
                  <input
                    type="checkbox"
                    checked={formulario.idsGeneros.some(
                      (id) => Number(id) === Number(genero.id)
                    )}
                    onChange={() => alternarGenero(genero.id)}
                  />
                  {genero.nome}
                </label>
              ))}
            </div>
          </div>

          <div style={estiloCampo}>
            <label htmlFor="ativo">Situação *</label>
            <select
              id="ativo"
              name="ativo"
              value={formulario.ativo ? 'true' : 'false'}
              onChange={(evento) =>
                setFormulario((anterior) => ({
                  ...anterior,
                  ativo: evento.target.value === 'true',
                }))
              }
              style={estiloInput}
            >
              <option value="true">Ativo</option>
              <option value="false">Inativo</option>
            </select>
          </div>

          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
            <button type="submit" disabled={salvando}>
              {salvando
                ? 'Salvando...'
                : idEdicao !== null
                  ? 'Salvar alterações'
                  : 'Cadastrar jogo'}
            </button>

            {idEdicao !== null && (
              <button
                type="button"
                onClick={() => limparFormulario()}
                disabled={salvando}
              >
                Cancelar edição
              </button>
            )}
          </div>
        </form>

        <hr />

        <h2>Jogos cadastrados</h2>

        <div
          style={{
            display: 'flex',
            flexWrap: 'wrap',
            gap: '14px',
            margin: '18px 0',
            padding: '16px',
            border: '1px solid #ccc',
            borderRadius: '8px',
          }}
        >
          <div style={estiloFiltro}>
            <label htmlFor="filtroNome">Buscar por nome</label>
            <input
              id="filtroNome"
              name="nome"
              type="search"
              placeholder="Digite o nome do jogo..."
              value={filtros.nome}
              onChange={atualizarFiltro}
              style={estiloInput}
            />
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroPlataforma">Filtrar por plataforma</label>
            <select
              id="filtroPlataforma"
              name="plataforma"
              value={filtros.plataforma}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todas as plataformas</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroCondicao">Filtrar por condição</label>
            <select
              id="filtroCondicao"
              name="condicao"
              value={filtros.condicao}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todas as condições</option>
              {condicoes.map((condicao) => (
                <option key={condicao.id} value={condicao.id}>
                  {condicao.descricao}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroGenero">Filtrar por gênero</label>
            <select
              id="filtroGenero"
              name="genero"
              value={filtros.genero}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todos os gêneros</option>
              {generos.map((genero) => (
                <option key={genero.id} value={genero.id}>
                  {genero.nome}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="ordenacao">Ordenar resultados</label>
            <select
              id="ordenacao"
              name="ordenacao"
              value={filtros.ordenacao}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="id-asc">ID: menor para maior</option>
              <option value="id-desc">ID: maior para menor</option>
              <option value="preco-asc">Preço: menor para maior</option>
              <option value="preco-desc">Preço: maior para menor</option>
              <option value="data-desc">Cadastro: mais recente</option>
              <option value="data-asc">Cadastro: mais antigo</option>
              <option value="estoque-desc">Estoque: maior quantidade</option>
              <option value="estoque-asc">Estoque: menor quantidade</option>
              <option value="nome-asc">Nome: A até Z</option>
              <option value="nome-desc">Nome: Z até A</option>
            </select>
          </div>

          <div style={{ display: 'flex', alignItems: 'end' }}>
            <button type="button" onClick={limparFiltros}>
              Limpar filtros
            </button>
          </div>
        </div>

        {!carregando && (
          <p role="status">
            Exibindo {jogosFiltrados.length} de {jogos.length} jogo(s).
          </p>
        )}

        {carregando ? (
          <p>Carregando jogos...</p>
        ) : jogosFiltrados.length === 0 ? (
          <p>
            {jogos.length === 0
              ? 'Nenhum jogo cadastrado.'
              : 'Nenhum jogo corresponde aos filtros selecionados.'}
          </p>
        ) : (
          <div style={{ width: '100%', overflowX: 'auto' }}>
            <table
              style={{
                width: '100%',
                minWidth: '1250px',
                tableLayout: 'fixed',
                borderCollapse: 'collapse',
              }}
            >
              <thead>
                <tr>
                  {['ID', 'Nome', 'Ano', 'Preço', 'Estoque', 'Plataforma', 'Condição', 'Gêneros', 'Status', 'Data de cadastro', 'Ações'].map((titulo) => (
                    <th key={titulo} style={estiloCelula}>{titulo}</th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {jogosFiltrados.map((jogo) => (
                  <tr key={jogo.id}>
                    <td style={estiloCelula}>{jogo.id}</td>
                    <td style={estiloCelula}>{jogo.nome}</td>
                    <td style={estiloCelula}>{jogo.anoLancamento}</td>
                    <td style={estiloCelula}>{formatarPreco(jogo.preco)}</td>
                    <td style={estiloCelula}>{jogo.estoque}</td>
                    <td style={estiloCelula}>{nomePlataforma(jogo.idPlataforma)}</td>
                    <td style={estiloCelula}>{nomeCondicao(jogo.idCondicaoJogo)}</td>
                    <td style={estiloCelula}>{nomesGeneros(jogo.idsGeneros)}</td>
                    <td style={estiloCelula}>{jogo.ativo ? 'Ativo' : 'Inativo'}</td>
                    <td style={estiloCelula}>
                      {jogo.dataCadastro
                        ? new Date(jogo.dataCadastro).toLocaleDateString('pt-BR')
                        : 'Não informado'}
                    </td>
                    <td style={estiloCelula}>
                      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                        <button type="button" onClick={() => iniciarEdicao(jogo)}>
                          Editar
                        </button>
                        <button type="button" onClick={() => excluir(jogo)}>
                          Excluir
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </main>
  )
}

export default GerenciarJogos

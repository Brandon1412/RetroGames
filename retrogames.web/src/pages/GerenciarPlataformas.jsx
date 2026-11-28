import { useEffect, useState } from 'react'
import { plataformaApi } from '../servicos/servicos'

const categorias = [
  'Console de mesa',
  'Console portátil',
  'Arcade',
  'Híbrido',
  'Outro',
]

function GerenciarPlataformas() {
  const anoMinimo = 1972
  const anoAtual = new Date().getFullYear()

  const formularioInicial = {
    nome: '',
    fabricante: '',
    anoLancamento: '',
    categoria: '',
    descricao: '',
    ativo: true,
  }

  const filtrosIniciais = {
    nome: '',
    fabricante: '',
    categoria: '',
    situacao: '',
    ordenacao: 'id-asc',
  }

  const [plataformas, setPlataformas] = useState([])
  const [formulario, setFormulario] = useState(formularioInicial)
  const [filtros, setFiltros] = useState(filtrosIniciais)
  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)
  const [mensagemErro, setMensagemErro] = useState('')
  const [mensagemSucesso, setMensagemSucesso] = useState('')
  const [idEdicao, setIdEdicao] = useState(null)

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

  const estiloFiltro = {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
    minWidth: '180px',
    flex: '1 1 180px',
  }

  const estiloCelula = {
    padding: '12px 10px',
    textAlign: 'left',
    verticalAlign: 'top',
    overflowWrap: 'anywhere',
  }

  async function carregarPlataformas() {
    try {
      setCarregando(true)
      setMensagemErro('')

      const dados = await plataformaApi.listar()
      setPlataformas(dados || [])
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível carregar as plataformas.'
      )
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPlataformas()
  }, [])

  function atualizarCampo(evento) {
    const { name, value, type, checked } = evento.target

    let novoValor = type === 'checkbox' ? checked : value

    if (name === 'anoLancamento') {
      novoValor = value.replace(/\D/g, '').slice(0, 4)
    }

    setFormulario((anterior) => ({
      ...anterior,
      [name]: novoValor,
    }))
  }

  function atualizarFiltro(evento) {
    const { name, value } = evento.target

    setFiltros((anterior) => ({
      ...anterior,
      [name]: value,
    }))
  }

  function limparFiltros() {
    setFiltros({ ...filtrosIniciais })
  }

  function limparFormulario(limparMensagens = true) {
    setFormulario({ ...formularioInicial })
    setIdEdicao(null)

    if (limparMensagens) {
      setMensagemErro('')
      setMensagemSucesso('')
    }
  }

  function iniciarEdicao(plataforma) {
    setIdEdicao(plataforma.id)

    setFormulario({
      nome: plataforma.nome || '',
      fabricante: plataforma.fabricante || '',
      anoLancamento: plataforma.anoLancamento?.toString() || '',
      categoria: plataforma.categoria || '',
      descricao: plataforma.descricao || '',
      ativo: plataforma.ativo ?? true,
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

    if (!formulario.nome.trim()) {
      setMensagemErro('Informe o nome da plataforma.')
      return
    }

    let anoLancamento = null

    if (formulario.anoLancamento.trim() !== '') {
      anoLancamento = Number(formulario.anoLancamento)

      if (
        !/^\d{4}$/.test(formulario.anoLancamento) ||
        !Number.isInteger(anoLancamento) ||
        anoLancamento < anoMinimo ||
        anoLancamento > anoAtual
      ) {
        setMensagemErro(
          `Informe um ano entre ${anoMinimo} e ${anoAtual}.`
        )
        return
      }
    }

    try {
      setSalvando(true)

      const dados = {
        nome: formulario.nome.trim(),
        fabricante: formulario.fabricante.trim() || null,
        anoLancamento,
        categoria: formulario.categoria || null,
        descricao: formulario.descricao.trim() || null,
        ativo: formulario.ativo,
      }

      if (estavaEditando) {
        await plataformaApi.atualizar(idEdicao, dados)
      } else {
        await plataformaApi.cadastrar(dados)
      }

      limparFormulario(false)
      await carregarPlataformas()

      setMensagemSucesso(
        estavaEditando
          ? 'Plataforma atualizada com sucesso.'
          : 'Plataforma cadastrada com sucesso.'
      )
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível salvar a plataforma.'
      )
    } finally {
      setSalvando(false)
    }
  }

  async function excluir(plataforma) {
    const confirmar = window.confirm(
      `Deseja realmente excluir a plataforma ${plataforma.nome}?`
    )

    if (!confirmar) return

    setMensagemErro('')
    setMensagemSucesso('')

    try {
      await plataformaApi.excluir(plataforma.id)

      if (idEdicao === plataforma.id) {
        limparFormulario(false)
      }

      await carregarPlataformas()
      setMensagemSucesso('Plataforma excluída com sucesso.')
    } catch (erro) {
      setMensagemErro(
        erro.response?.data?.message ||
        erro.response?.data?.mensagem ||
        'Não foi possível excluir a plataforma.'
      )
    }
  }

  function exibirValor(valor) {
    return valor !== null && valor !== undefined && valor !== ''
      ? valor
      : 'Não informado'
  }

  const fabricantes = [
    ...new Set(
      plataformas
        .map((plataforma) => plataforma.fabricante)
        .filter((fabricante) => fabricante)
    ),
  ].sort((a, b) => a.localeCompare(b, 'pt-BR'))

  const categoriasDisponiveis = [
    ...new Set([
      ...categorias,
      ...plataformas
        .map((plataforma) => plataforma.categoria)
        .filter((categoria) => categoria),
    ]),
  ].sort((a, b) => a.localeCompare(b, 'pt-BR'))

  const plataformasFiltradas = plataformas
    .filter((plataforma) => {
      const nomeCorresponde = (plataforma.nome || '')
        .toLocaleLowerCase('pt-BR')
        .includes(filtros.nome.trim().toLocaleLowerCase('pt-BR'))

      const fabricanteCorresponde =
        !filtros.fabricante ||
        plataforma.fabricante === filtros.fabricante

      const categoriaCorresponde =
        !filtros.categoria ||
        plataforma.categoria === filtros.categoria

      const situacaoCorresponde =
        filtros.situacao === '' ||
        plataforma.ativo === (filtros.situacao === 'ativa')

      return (
        nomeCorresponde &&
        fabricanteCorresponde &&
        categoriaCorresponde &&
        situacaoCorresponde
      )
    })
    .sort((a, b) => {
      switch (filtros.ordenacao) {
        case 'id-desc':
          return Number(b.id) - Number(a.id)

        case 'nome-asc':
          return (a.nome || '').localeCompare(b.nome || '', 'pt-BR')

        case 'nome-desc':
          return (b.nome || '').localeCompare(a.nome || '', 'pt-BR')

        case 'fabricante-asc':
          return (a.fabricante || '').localeCompare(
            b.fabricante || '',
            'pt-BR'
          )

        case 'fabricante-desc':
          return (b.fabricante || '').localeCompare(
            a.fabricante || '',
            'pt-BR'
          )

        case 'ano-asc':
          return Number(a.anoLancamento || 0) -
            Number(b.anoLancamento || 0)

        case 'ano-desc':
          return Number(b.anoLancamento || 0) -
            Number(a.anoLancamento || 0)

        case 'jogos-asc':
          return Number(a.quantidadeJogos || 0) -
            Number(b.quantidadeJogos || 0)

        case 'jogos-desc':
          return Number(b.quantidadeJogos || 0) -
            Number(a.quantidadeJogos || 0)

        case 'id-asc':
        default:
          return Number(a.id) - Number(b.id)
      }
    })

  return (
    <main className="container">
      <section className="secao">
        <h1>Gerenciar plataformas</h1>

        <p>
          Cadastre, consulte, edite e exclua plataformas de videogame.
        </p>

        {mensagemErro && (
          <p
            role="alert"
            style={{ color: '#dc3545', margin: '12px 0' }}
          >
            {mensagemErro}
          </p>
        )}

        {mensagemSucesso && (
          <p
            role="status"
            style={{ color: '#198754', margin: '12px 0' }}
          >
            {mensagemSucesso}
          </p>
        )}

        <h2>
          {idEdicao !== null
            ? 'Editar plataforma'
            : 'Cadastrar plataforma'}
        </h2>

        <form onSubmit={salvar}>
          <div style={estiloCampo}>
            <label htmlFor="nome">Nome *</label>
            <input
              id="nome"
              name="nome"
              type="text"
              value={formulario.nome}
              onChange={atualizarCampo}
              maxLength={80}
              required
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="fabricante">Fabricante</label>
            <input
              id="fabricante"
              name="fabricante"
              type="text"
              value={formulario.fabricante}
              onChange={atualizarCampo}
              maxLength={80}
              placeholder="Ex.: Nintendo, Sega, Sony..."
              style={estiloInput}
            />
          </div>

          <div style={estiloCampo}>
            <label htmlFor="anoLancamento">Ano de lançamento</label>
            <input
              id="anoLancamento"
              name="anoLancamento"
              type="text"
              inputMode="numeric"
              maxLength={4}
              value={formulario.anoLancamento}
              onChange={atualizarCampo}
              placeholder="Ex.: 1998"
              style={estiloInput}
            />
            <small>
              Informe um ano entre {anoMinimo} e {anoAtual}.
              Deixe vazio se não souber.
            </small>
          </div>

          <div style={estiloCampo}>
            <label htmlFor="categoria">Categoria</label>
            <select
              id="categoria"
              name="categoria"
              value={formulario.categoria}
              onChange={atualizarCampo}
              style={estiloInput}
            >
              <option value="">Selecione uma categoria</option>

              {categoriasDisponiveis.map((categoria) => (
                <option key={categoria} value={categoria}>
                  {categoria}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloCampo}>
            <label htmlFor="descricao">Outras informações</label>
            <textarea
              id="descricao"
              name="descricao"
              value={formulario.descricao}
              onChange={atualizarCampo}
              rows={3}
              maxLength={255}
              placeholder="Informações adicionais sobre a plataforma..."
              style={estiloInput}
            />
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
              <option value="true">Ativa</option>
              <option value="false">Inativa</option>
            </select>
          </div>

          <div
            style={{
              display: 'flex',
              flexWrap: 'wrap',
              gap: '8px',
            }}
          >
            <button type="submit" disabled={salvando}>
              {salvando
                ? 'Salvando...'
                : idEdicao !== null
                  ? 'Salvar alterações'
                  : 'Cadastrar plataforma'}
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

        <h2>Plataformas cadastradas</h2>

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
              placeholder="Digite o nome da plataforma..."
              value={filtros.nome}
              onChange={atualizarFiltro}
              style={estiloInput}
            />
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroFabricante">Filtrar por fabricante</label>
            <select
              id="filtroFabricante"
              name="fabricante"
              value={filtros.fabricante}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todos os fabricantes</option>
              {fabricantes.map((fabricante) => (
                <option key={fabricante} value={fabricante}>
                  {fabricante}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroCategoria">Filtrar por categoria</label>
            <select
              id="filtroCategoria"
              name="categoria"
              value={filtros.categoria}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todas as categorias</option>
              {categoriasDisponiveis.map((categoria) => (
                <option key={categoria} value={categoria}>
                  {categoria}
                </option>
              ))}
            </select>
          </div>

          <div style={estiloFiltro}>
            <label htmlFor="filtroSituacao">Filtrar por situação</label>
            <select
              id="filtroSituacao"
              name="situacao"
              value={filtros.situacao}
              onChange={atualizarFiltro}
              style={estiloInput}
            >
              <option value="">Todas as situações</option>
              <option value="ativa">Ativas</option>
              <option value="inativa">Inativas</option>
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
              <option value="nome-asc">Nome: A até Z</option>
              <option value="nome-desc">Nome: Z até A</option>
              <option value="fabricante-asc">Fabricante: A até Z</option>
              <option value="fabricante-desc">Fabricante: Z até A</option>
              <option value="ano-asc">Ano: mais antigo</option>
              <option value="ano-desc">Ano: mais recente</option>
              <option value="jogos-desc">Jogos: maior quantidade</option>
              <option value="jogos-asc">Jogos: menor quantidade</option>
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
            Exibindo {plataformasFiltradas.length} de {plataformas.length} plataforma(s).
          </p>
        )}

        {carregando ? (
          <p>Carregando plataformas...</p>
        ) : plataformasFiltradas.length === 0 ? (
          <p>
            {plataformas.length === 0
              ? 'Nenhuma plataforma cadastrada.'
              : 'Nenhuma plataforma corresponde aos filtros selecionados.'}
          </p>
        ) : (
          <div style={{ width: '100%', overflowX: 'auto' }}>
            <table
              style={{
                width: '100%',
                minWidth: '1350px',
                tableLayout: 'fixed',
                borderCollapse: 'collapse',
              }}
            >
              <colgroup>
                <col style={{ width: '5%' }} />
                <col style={{ width: '13%' }} />
                <col style={{ width: '10%' }} />
                <col style={{ width: '12%' }} />
                <col style={{ width: '7%' }} />
                <col style={{ width: '12%' }} />
                <col style={{ width: '18%' }} />
                <col style={{ width: '7%' }} />
                <col style={{ width: '16%' }} />
              </colgroup>

              <thead>
                <tr>
                  {[
                    'ID',
                    'Nome',
                    'Jogos cadastrados',
                    'Fabricante',
                    'Ano',
                    'Categoria',
                    'Outras informações',
                    'Situação',
                    'Ações',
                  ].map((titulo) => (
                    <th key={titulo} style={estiloCelula}>
                      {titulo}
                    </th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {plataformasFiltradas.map((plataforma) => (
                  <tr key={plataforma.id}>
                    <td style={estiloCelula}>{plataforma.id}</td>
                    <td style={estiloCelula}>{plataforma.nome}</td>
                    <td style={estiloCelula}>
                      {plataforma.quantidadeJogos ?? 0}
                    </td>
                    <td style={estiloCelula}>
                      {exibirValor(plataforma.fabricante)}
                    </td>
                    <td style={estiloCelula}>
                      {exibirValor(plataforma.anoLancamento)}
                    </td>
                    <td style={estiloCelula}>
                      {exibirValor(plataforma.categoria)}
                    </td>
                    <td style={estiloCelula}>
                      {exibirValor(plataforma.descricao)}
                    </td>
                    <td style={estiloCelula}>
                      {plataforma.ativo ? 'Ativa' : 'Inativa'}
                    </td>
                    <td style={estiloCelula}>
                      <div
                        style={{
                          display: 'flex',
                          flexWrap: 'wrap',
                          gap: '8px',
                        }}
                      >
                        <button
                          type="button"
                          onClick={() => iniciarEdicao(plataforma)}
                        >
                          Editar
                        </button>

                        <button
                          type="button"
                          onClick={() => excluir(plataforma)}
                        >
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

export default GerenciarPlataformas

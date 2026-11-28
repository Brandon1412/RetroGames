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
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)
  const [mensagemErro, setMensagemErro] = useState('')
  const [mensagemSucesso, setMensagemSucesso] = useState('')
  const [idEdicao, setIdEdicao] = useState(null)

  const formularioInicial = {
    nome: '',
    fabricante: '',
    anoLancamento: '',
    categoria: '',
    descricao: '',
    ativo: true,
  }

  const [formulario, setFormulario] = useState(formularioInicial)

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
  }

  async function salvar(evento) {
    evento.preventDefault()

    setMensagemErro('')
    setMensagemSucesso('')

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

      if (idEdicao !== null) {
        await plataformaApi.atualizar(idEdicao, dados)
        setMensagemSucesso('Plataforma atualizada com sucesso.')
      } else {
        await plataformaApi.cadastrar(dados)
        setMensagemSucesso('Plataforma cadastrada com sucesso.')
      }

      limparFormulario(false)
      await carregarPlataformas()
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

    if (!confirmar) {
      return
    }

    setMensagemErro('')
    setMensagemSucesso('')

    try {
      await plataformaApi.excluir(plataforma.id)

      if (idEdicao === plataforma.id) {
        limparFormulario(false)
      }

      setMensagemSucesso('Plataforma excluída com sucesso.')
      await carregarPlataformas()
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

  const estiloTabela = {
    width: '100%',
    minWidth: '1050px',
    tableLayout: 'fixed',
    borderCollapse: 'collapse',
  }

  const estiloCelula = {
    padding: '12px 10px',
    textAlign: 'left',
    verticalAlign: 'top',
    overflowWrap: 'anywhere',
    wordBreak: 'normal',
  }

  const estiloCampo = {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
    marginBottom: '14px',
  }

  const estiloInput = {
    width: '100%',
    maxWidth: '100%',
    boxSizing: 'border-box',
    padding: '10px',
  }

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
              Informe um ano entre {anoMinimo} e {anoAtual}
              (ano atual). Deixe vazio se não souber.
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

              {categorias.map((categoria) => (
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

          {idEdicao !== null && (
            <div style={{ marginBottom: '14px' }}>
              <label>
                <input
                  type="checkbox"
                  name="ativo"
                  checked={formulario.ativo}
                  onChange={atualizarCampo}
                />
                {' '}Plataforma ativa
              </label>
            </div>
          )}

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

        {carregando ? (
          <p>Carregando plataformas...</p>
        ) : plataformas.length === 0 ? (
          <p>Nenhuma plataforma encontrada.</p>
        ) : (
          <div style={{ width: '100%', overflowX: 'auto' }}>
            <table style={estiloTabela}>
              <colgroup>
                <col style={{ width: '5%' }} />
                <col style={{ width: '15%' }} />
                <col style={{ width: '13%' }} />
                <col style={{ width: '9%' }} />
                <col style={{ width: '13%' }} />
                <col style={{ width: '20%' }} />
                <col style={{ width: '8%' }} />
                <col style={{ width: '17%' }} />
              </colgroup>

              <thead>
                <tr>
                  <th style={estiloCelula}>ID</th>
                  <th style={estiloCelula}>Nome</th>
                  <th style={estiloCelula}>Fabricante</th>
                  <th style={estiloCelula}>Ano</th>
                  <th style={estiloCelula}>Categoria</th>
                  <th style={estiloCelula}>Outras informações</th>
                  <th style={estiloCelula}>Status</th>
                  <th style={estiloCelula}>Ações</th>
                </tr>
              </thead>

              <tbody>
                {plataformas.map((plataforma) => (
                  <tr key={plataforma.id}>
                    <td style={estiloCelula}>
                      {plataforma.id}
                    </td>

                    <td style={estiloCelula}>
                      {plataforma.nome}
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
                          gap: '6px',
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


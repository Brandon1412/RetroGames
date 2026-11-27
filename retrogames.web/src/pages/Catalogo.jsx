
import { useEffect, useMemo, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { jogoApi, plataformaApi } from '../servicos/servicos'
import { obterMensagemErro } from '../servicos/erros'

const JOGOS_POR_PAGINA = 6

function formatarPreco(preco) {
  return Number(preco).toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  })
}

function JogoCard({ jogo, nomePlataforma }) {
  return (
    <article className="jogo-card">
      <div className="jogo-capa">
        <span className="capa-console">RETROGAMES</span>
        <span className="capa-titulo">{jogo.nome}</span>
        <span className="capa-ano">{jogo.anoLancamento}</span>
        <span className="capa-decoracao">✦</span>
      </div>

      <div className="jogo-conteudo">
        <div className="jogo-identificacao">
          <span className="jogo-plataforma">
            {nomePlataforma || `Plataforma #${jogo.idPlataforma}`}
          </span>
          <span className="jogo-id">#{jogo.id}</span>
        </div>

        <h3>{jogo.nome}</h3>

        <p className="jogo-descricao">
          {jogo.descricao || 'Clássico dos videogames.'}
        </p>

        <div className="jogo-informacoes">
          <span>Ano: {jogo.anoLancamento}</span>
          <span>
            {jogo.estoque > 0
              ? `${jogo.estoque} em estoque`
              : 'Sem estoque'}
          </span>
        </div>

        <div className="jogo-rodape">
          <strong>{formatarPreco(jogo.preco)}</strong>
          <span className={jogo.estoque > 0 ? 'disponivel' : 'indisponivel'}>
            {jogo.estoque > 0 ? 'Disponível' : 'Indisponível'}
          </span>
        </div>

        <Link to={`/jogos/${jogo.id}`} className="ver-detalhes">
          VER DETALHES
        </Link>
      </div>
    </article>
  )
}

function Catalogo() {
  const [jogos, setJogos] = useState([])
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [mensagemErro, setMensagemErro] = useState('')
  const [parametros, setParametros] = useSearchParams()

  const busca = parametros.get('busca') || ''
  const idPlataforma = parametros.get('idPlataforma') || ''
  const paginaSolicitada = Math.max(
    1,
    Number.parseInt(parametros.get('pagina') || '1', 10) || 1,
  )

  async function carregarJogos() {
    try {
      setCarregando(true)
      setMensagemErro('')

      const dados = await jogoApi.listar()
      setJogos(dados || [])
    } catch (erro) {
      setMensagemErro(obterMensagemErro(erro))
    } finally {
      setCarregando(false)
    }
  }

  async function carregarPlataformas() {
    try {
      const dados = await plataformaApi.listar()
      setPlataformas((dados || []).filter((plataforma) => plataforma.ativo))
    } catch (erro) {
      console.error('Não foi possível carregar as plataformas:', erro)
    }
  }

  useEffect(() => {
    carregarJogos()
    carregarPlataformas()
  }, [])

  function atualizarParametro(nome, valor) {
    const novosParametros = new URLSearchParams(parametros)

    if (valor) {
      novosParametros.set(nome, valor)
    } else {
      novosParametros.delete(nome)
    }

    if (nome !== 'pagina') {
      novosParametros.set('pagina', '1')
    }

    setParametros(novosParametros)
  }

  function limparFiltros() {
    setParametros({})
  }

  const nomesPlataformas = useMemo(() => {
    return Object.fromEntries(
      plataformas.map((plataforma) => [
        String(plataforma.id),
        plataforma.nome,
      ]),
    )
  }, [plataformas])

  const jogosFiltrados = useMemo(() => {
    const termo = busca.trim().toLocaleLowerCase('pt-BR')

    return jogos.filter((jogo) => {
      const correspondeNome =
        !termo ||
        jogo.nome?.toLocaleLowerCase('pt-BR').includes(termo)

      const correspondePlataforma =
        !idPlataforma ||
        String(jogo.idPlataforma) === idPlataforma

      return correspondeNome && correspondePlataforma
    })
  }, [jogos, busca, idPlataforma])

  const totalPaginas = Math.max(
    1,
    Math.ceil(jogosFiltrados.length / JOGOS_POR_PAGINA),
  )

  const paginaAtual = Math.min(paginaSolicitada, totalPaginas)

  useEffect(() => {
    if (paginaSolicitada > totalPaginas) {
      atualizarParametro('pagina', String(totalPaginas))
    }
  }, [paginaSolicitada, totalPaginas])

  const jogosDaPagina = useMemo(() => {
    const inicio = (paginaAtual - 1) * JOGOS_POR_PAGINA
    return jogosFiltrados.slice(inicio, inicio + JOGOS_POR_PAGINA)
  }, [jogosFiltrados, paginaAtual])

  return (
    <main>
      <section className="banner" id="inicio">
        <div className="banner-conteudo">
          <span className="etiqueta">O PASSADO NUNCA SAI DE MODA</span>

          <h1>
            GRANDES JOGOS.
            <br />
            <span>MEMÓRIAS ETERNAS.</span>
          </h1>

          <p>
            Reviva os clássicos que marcaram gerações.
            Encontre seus jogos favoritos no catálogo RetroGames.
          </p>

          <a href="#catalogo" className="botao-banner">
            EXPLORAR CATÁLOGO <span>→</span>
          </a>
        </div>

        <div className="banner-arte" aria-hidden="true">
          <div className="pixel-console">▣</div>
          <div className="pixel-texto">INSERT COIN</div>
          <div className="pixel-estrelas">✦　✧　✦</div>
        </div>
      </section>

      <section className="catalogo" id="catalogo">
        <div className="catalogo-topo">
          <div>
            <span className="secao-etiqueta">NOSSA COLEÇÃO</span>
            <h2>CATÁLOGO DE JOGOS</h2>
            <p>Clássicos que continuam fazendo história.</p>
          </div>

          <div className="contador-jogos">
            <strong>{jogosFiltrados.length}</strong>
            <span>JOGOS ENCONTRADOS</span>
          </div>
        </div>

        <div className="filtros-catalogo">
          <div className="barra-pesquisa">
            <span aria-hidden="true">⌕</span>
            <input
              type="search"
              placeholder="Pesquisar jogo pelo nome..."
              value={busca}
              onChange={(evento) =>
                atualizarParametro('busca', evento.target.value)
              }
              aria-label="Pesquisar jogos pelo nome"
            />

            {busca && (
              <button
                type="button"
                onClick={() => atualizarParametro('busca', '')}
                aria-label="Limpar pesquisa"
              >
                ×
              </button>
            )}
          </div>

          <div className="filtro-plataforma">
            <label htmlFor="filtro-plataforma">PLATAFORMA</label>
            <select
              id="filtro-plataforma"
              value={idPlataforma}
              onChange={(evento) =>
                atualizarParametro('idPlataforma', evento.target.value)
              }
            >
              <option value="">Todas as plataformas</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>
                  {plataforma.nome}
                </option>
              ))}
            </select>
          </div>

          {(busca || idPlataforma) && (
            <button
              type="button"
              className="botao-limpar-filtros"
              onClick={limparFiltros}
            >
              LIMPAR FILTROS
            </button>
          )}
        </div>

        {carregando && (
          <div className="estado" role="status">
            <div className="carregamento"></div>
            <p>CARREGANDO CATÁLOGO...</p>
          </div>
        )}

        {!carregando && mensagemErro && (
          <div className="estado estado-erro" role="alert">
            <span>!</span>
            <h3>Não foi possível carregar os jogos</h3>
            <p>{mensagemErro}</p>
            <button
              type="button"
              className="botao-tentar"
              onClick={carregarJogos}
            >
              TENTAR NOVAMENTE
            </button>
          </div>
        )}

        {!carregando && !mensagemErro && jogosFiltrados.length === 0 && (
          <div className="estado estado-vazio">
            <span className="vazio-icone">⌕</span>
            <h3>Nenhum jogo encontrado</h3>
            <p>
              {busca || idPlataforma
                ? 'Tente alterar os filtros da pesquisa.'
                : 'Ainda não existem jogos disponíveis no catálogo.'}
            </p>
            {(busca || idPlataforma) && (
              <button
                type="button"
                className="botao-tentar"
                onClick={limparFiltros}
              >
                LIMPAR FILTROS
              </button>
            )}
          </div>
        )}

        {!carregando && !mensagemErro && jogosFiltrados.length > 0 && (
          <>
            <div className="grade-jogos">
              {jogosDaPagina.map((jogo) => (
                <JogoCard
                  key={jogo.id}
                  jogo={jogo}
                  nomePlataforma={nomesPlataformas[String(jogo.idPlataforma)]}
                />
              ))}
            </div>

            {totalPaginas > 1 && (
              <nav className="paginacao" aria-label="Paginação do catálogo">
                <button
                  type="button"
                  disabled={paginaAtual === 1}
                  onClick={() =>
                    atualizarParametro('pagina', String(paginaAtual - 1))
                  }
                >
                  ← ANTERIOR
                </button>

                <span>
                  Página <strong>{paginaAtual}</strong> de {totalPaginas}
                </span>

                <button
                  type="button"
                  disabled={paginaAtual === totalPaginas}
                  onClick={() =>
                    atualizarParametro('pagina', String(paginaAtual + 1))
                  }
                >
                  PRÓXIMA →
                </button>
              </nav>
            )}
          </>
        )}
      </section>
    </main>
  )
}

export default Catalogo

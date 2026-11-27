
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi, plataformaApi } from '../servicos/servicos'
import { obterMensagemErro } from '../servicos/erros'

function formatarPreco(preco) {
  return Number(preco).toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  })
}

function DetalheJogo() {
  const { id } = useParams()

  const [jogo, setJogo] = useState(null)
  const [plataforma, setPlataforma] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [mensagemErro, setMensagemErro] = useState('')

  useEffect(() => {
    async function carregarJogo() {
      try {
        setCarregando(true)
        setMensagemErro('')
        setJogo(null)
        setPlataforma(null)

        const dados = await jogoApi.buscarPorId(id)
        setJogo(dados)

        if (dados?.idPlataforma) {
          try {
            const dadosPlataforma = await plataformaApi.buscarPorId(
              dados.idPlataforma,
            )
            setPlataforma(dadosPlataforma)
          } catch {
            // A página continua funcionando mesmo se a plataforma falhar.
          }
        }
      } catch (erro) {
        setMensagemErro(obterMensagemErro(erro))
      } finally {
        setCarregando(false)
      }
    }

    carregarJogo()
  }, [id])

  if (carregando) {
    return (
      <main className="catalogo estado" role="status">
        <div className="carregamento"></div>
        <p>CARREGANDO JOGO...</p>
      </main>
    )
  }

  if (mensagemErro || !jogo) {
    return (
      <main className="catalogo estado estado-erro" role="alert">
        <h2>Não foi possível encontrar o jogo</h2>
        <p>{mensagemErro || 'Jogo não encontrado.'}</p>
        <Link to="/" className="ver-detalhes">
          VOLTAR AO CATÁLOGO
        </Link>
      </main>
    )
  }

  return (
    <main className="detalhe-pagina">
      <div className="detalhe-container">
        <Link to="/" className="detalhe-voltar">
          ← VOLTAR AO CATÁLOGO
        </Link>

        <span className="secao-etiqueta">NOSSA COLEÇÃO</span>
        <h1 className="detalhe-titulo">{jogo.nome}</h1>

        <div className="detalhe-grade">
          <div className="detalhe-capa">
            <span className="capa-console">RETROGAMES</span>
            <span className="detalhe-capa-titulo">{jogo.nome}</span>
            <span className="capa-ano">{jogo.anoLancamento}</span>
            <span className="detalhe-decoracao">✦</span>
          </div>

          <section className="detalhe-informacoes">
            <span className="detalhe-etiqueta">DETALHES DO JOGO</span>

            <h2>{jogo.nome}</h2>

            <p className="detalhe-descricao">
              {jogo.descricao || 'Clássico dos videogames.'}
            </p>

            <div className="detalhe-dados">
              <div>
                <span>PLATAFORMA</span>
                <strong>
                  {plataforma?.nome || `Plataforma #${jogo.idPlataforma}`}
                </strong>
              </div>

              <div>
                <span>ANO DE LANÇAMENTO</span>
                <strong>{jogo.anoLancamento || 'Não informado'}</strong>
              </div>

              <div>
                <span>CÓDIGO DO JOGO</span>
                <strong>#{jogo.id}</strong>
              </div>

              <div>
                <span>CONDIÇÃO</span>
                <strong>
                  {jogo.idCondicaoJogo
                    ? `Condição #${jogo.idCondicaoJogo}`
                    : 'Não informada'}
                </strong>
              </div>
            </div>

            <div className="detalhe-compra">
              <div>
                <span>PREÇO</span>
                <strong>{formatarPreco(jogo.preco)}</strong>
              </div>

              <span
                className={
                  jogo.estoque > 0
                    ? 'disponivel'
                    : 'indisponivel'
                }
              >
                {jogo.estoque > 0
                  ? `${jogo.estoque} unidades em estoque`
                  : 'Sem estoque'}
              </span>
            </div>

            <Link to="/" className="detalhe-botao">
              EXPLORAR MAIS JOGOS →
            </Link>
          </section>
        </div>
      </div>
    </main>
  )
}

export default DetalheJogo

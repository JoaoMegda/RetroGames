import { useEffect, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { obterMensagemErro } from '../../api/erros.js';
import Icone from '../../components/Icone.jsx';
import { EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';
import { useAutenticacao } from '../../hooks/useAutenticacao.js';
import { useCarrinho } from '../../hooks/useCarrinho.js';
import { QUANTIDADE_MAXIMA } from '../../lib/carrinho.js';
import { formatarPreco } from '../../lib/formato.js';

const OPCOES_QUANTIDADE = Array.from({ length: QUANTIDADE_MAXIMA }, (_, indice) => indice + 1);

export default function Carrinho() {
  const { usuario } = useAutenticacao();
  const { itens, total, quantidadeTotal, alterar, remover, finalizar, finalizando } = useCarrinho();
  const [erro, setErro] = useState(null);
  const [pedido, setPedido] = useState(null);
  const tituloConfirmacao = useRef(null);

  useEffect(() => {
    if (pedido) tituloConfirmacao.current?.focus();
  }, [pedido]);

  async function aoFinalizar() {
    setErro(null);
    try {
      const confirmado = await finalizar(usuario.id);
      if (confirmado) setPedido(confirmado);
    } catch (falha) {
      const mensagem = obterMensagemErro(falha);
      const estoqueInsuficiente = falha?.response?.status === 409 && falha.response.data?.responseCode === 111;
      setErro(estoqueInsuficiente ? `${mensagem} Revise as quantidades ou remova itens e tente novamente.` : mensagem);
    }
  }

  return (
    <>
      <h1 className="titulo-pagina">Carrinho</h1>
      {erro && <MensagemErro mensagem={erro} />}

      {pedido ? (
        <section className="card confirmacao-pedido" aria-labelledby="titulo-confirmacao">
          <div className="card-body">
            <span className="icone-confirmacao"><Icone nome="confirmacao" /></span>
            <h2 className="h4" id="titulo-confirmacao" ref={tituloConfirmacao} tabIndex={-1}>Pedido realizado</h2>
            <p className="mb-1">Pedido <strong>#{pedido.id}</strong> · {pedido.situacao}</p>
            <p className="mb-3">Total de <strong>{formatarPreco(pedido.total)}</strong></p>
            <Link className="btn btn-primary" to="/meus-pedidos">Ver meus pedidos <Icone nome="direita" /></Link>
          </div>
        </section>
      ) : itens.length === 0 ? (
        <EstadoVazio titulo="Seu carrinho está vazio">
          <p>Adicione um jogo do catálogo para começar.</p>
          <Link className="btn btn-primary" to="/">Explorar catálogo <Icone nome="direita" /></Link>
        </EstadoVazio>
      ) : (
        <div className="row g-4 align-items-start">
          <div className="col-lg-8">
            <ul className="lista-carrinho list-unstyled mb-0">
              {itens.map((item) => (
                <li className="card item-carrinho" key={item.id}>
                  <div className="card-body">
                    <div className="produto-carrinho">
                      <h2 className="h5 mb-1"><Link to={`/jogos/${item.id}`}>{item.nome}</Link></h2>
                      <p className="text-body-secondary mb-0">{formatarPreco(item.preco)} por unidade</p>
                    </div>
                    <div className="quantidade-carrinho">
                      <label className="form-label mb-1" htmlFor={`quantidade-${item.id}`}>Quantidade</label>
                      <select id={`quantidade-${item.id}`} className="form-select"
                              aria-label={`Quantidade de ${item.nome}`} value={item.quantidade}
                              disabled={finalizando}
                              onChange={(evento) => {
                                setErro(null);
                                alterar(item.id, Number(evento.target.value));
                              }}>
                        {OPCOES_QUANTIDADE.map((quantidade) => <option key={quantidade} value={quantidade}>{quantidade}</option>)}
                      </select>
                    </div>
                    <div className="subtotal-carrinho">
                      <strong>{formatarPreco(item.preco * item.quantidade)}</strong>
                      <button type="button" className="btn btn-outline-danger" disabled={finalizando}
                              aria-label={`Remover ${item.nome} do carrinho`}
                              onClick={() => {
                                setErro(null);
                                remover(item.id);
                              }}>
                        Remover
                      </button>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
            <Link className="link-acao mt-3" to="/"><Icone nome="esquerda" /> Continuar comprando</Link>
          </div>
          <aside className="col-lg-4">
            <div className="card resumo-carrinho">
              <div className="card-body">
                <h2 className="h5">Resumo do pedido</h2>
                <p className="text-body-secondary mb-3">{quantidadeTotal === 1 ? '1 unidade' : `${quantidadeTotal} unidades`}</p>
                <dl className="total-carrinho">
                  <dt>Total estimado</dt>
                  <dd>{formatarPreco(total)}</dd>
                </dl>
                <p className="small text-body-secondary">O total é confirmado ao finalizar a compra.</p>
                <button type="button" className="btn btn-primary w-100" onClick={aoFinalizar} disabled={finalizando}>
                  {finalizando ? 'Finalizando...' : 'Finalizar compra'}
                </button>
                {finalizando && <span className="visually-hidden" role="status">Enviando pedido...</span>}
              </div>
            </div>
          </aside>
        </div>
      )}
    </>
  );
}

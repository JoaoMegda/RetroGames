import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { pedidoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import Icone from '../../components/Icone.jsx';
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';
import { useAutenticacao } from '../../hooks/useAutenticacao.js';
import { formatarDataHora, formatarPreco } from '../../lib/formato.js';

export default function MeusPedidos() {
  const { usuario } = useAutenticacao();
  const idUsuario = usuario.id;
  const [tentativa, setTentativa] = useState(0);
  const [consulta, setConsulta] = useState(null);

  useEffect(() => {
    const requisicao = new AbortController();

    pedidoApi.listarMeus(requisicao.signal)
      .then((pedidos) => {
        if (!requisicao.signal.aborted) setConsulta({ idUsuario, tentativa, pedidos });
      })
      .catch((falha) => {
        if (!requisicao.signal.aborted) {
          setConsulta({ idUsuario, tentativa, erro: obterMensagemErro(falha) });
        }
      });

    return () => requisicao.abort();
  }, [idUsuario, tentativa]);

  const consultaAtual = consulta?.idUsuario === idUsuario && consulta.tentativa === tentativa ? consulta : null;

  return (
    <>
      <h1 className="titulo-pagina">Meus pedidos</h1>
      {!consultaAtual && <Carregando texto="Carregando pedidos..." />}
      {consultaAtual?.erro && (
        <MensagemErro mensagem={consultaAtual.erro} aoTentarNovamente={() => setTentativa((anterior) => anterior + 1)} />
      )}
      {consultaAtual?.pedidos?.length === 0 && (
        <EstadoVazio titulo="Você ainda não fez pedidos">
          <Link className="btn btn-primary mt-2" to="/">Explorar catálogo <Icone nome="direita" /></Link>
        </EstadoVazio>
      )}
      {consultaAtual?.pedidos?.length > 0 && (
        <ul className="lista-pedidos list-unstyled">
          {consultaAtual.pedidos.map((pedido) => (
            <li className="card pedido" key={pedido.id} id={`pedido-${pedido.id}`}>
              <div className="card-header">
                <div>
                  <h2 className="h5 mb-1">Pedido #{pedido.id}</h2>
                  <p className="text-body-secondary mb-0">{formatarDataHora(pedido.data)}</p>
                </div>
                <div className="pedido-total">
                  <span className="etiqueta-situacao">{pedido.situacao}</span>
                  <strong>{formatarPreco(pedido.total)}</strong>
                </div>
              </div>
              <ul className="itens-pedido list-unstyled mb-0">
                {pedido.itens.map((item) => (
                  <li className="item-pedido" key={item.idJogo}>
                    <div>
                      <strong>{item.nomeJogo}</strong>
                      <p className="text-body-secondary small mb-0">{item.quantidade} × {formatarPreco(item.valorUnitario)}</p>
                    </div>
                    <span className="preco-jogo">{formatarPreco(item.quantidade * item.valorUnitario)}</span>
                  </li>
                ))}
              </ul>
            </li>
          ))}
        </ul>
      )}
    </>
  );
}

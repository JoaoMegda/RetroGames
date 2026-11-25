import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import { formatarPreco } from '../../lib/formato.js';
import { QUANTIDADE_MAXIMA } from '../../lib/carrinho.js';
import { useCarrinho } from '../../hooks/useCarrinho.js';
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx';
import Icone from '../../components/Icone.jsx';
import IlustracaoJogo from '../../components/IlustracaoJogo.jsx';

export default function DetalheJogo() {
  const { id } = useParams();
  const { itens, adicionar, finalizando } = useCarrinho();
  const [consulta, setConsulta] = useState(null);
  const [tentativa, setTentativa] = useState(0);

  useEffect(() => {
    let vigente = true;
    jogoApi.buscarPorId(id)
      .then((jogo) => {
        if (vigente) setConsulta({ id, tentativa, jogo });
      })
      .catch((falha) => {
        if (vigente) setConsulta({ id, tentativa, erro: obterMensagemErro(falha) });
      });

    return () => { vigente = false; };
  }, [id, tentativa]);

  if (!consulta || consulta.id !== id || consulta.tentativa !== tentativa) return <Carregando />;

  const { jogo, erro } = consulta;
  if (erro) {
    return (
      <>
        <MensagemErro mensagem={erro} aoTentarNovamente={() => setTentativa((anterior) => anterior + 1)} />
        <Link className="link-acao" to="/"><Icone nome="esquerda" /> Voltar ao catálogo</Link>
      </>
    );
  }

  const esgotado = jogo.estoque === 0;
  const quantidadeNoCarrinho = itens.find((item) => item.id === jogo.id)?.quantidade ?? 0;
  const atingiuLimite = quantidadeNoCarrinho >= QUANTIDADE_MAXIMA;

  return (
    <>
      <Link to="/" className="link-acao mb-3"><Icone nome="esquerda" /> Voltar ao catálogo</Link>
      <div className="card detalhe-jogo">
        <div className="row g-0">
          <div className="col-md-5">
            <IlustracaoJogo id={jogo.id} />
          </div>
          <div className="col-md-7">
            <div className="card-body">
              <span className="etiqueta-plataforma mb-2">{jogo.nomePlataforma}</span>
              {!jogo.ativo && <span className="badge text-bg-warning ms-2">Inativo</span>}
              <h1 className="titulo-pagina">{jogo.nome}</h1>
              <p className="text-body-secondary">{jogo.anoLancamento} · {jogo.descricaoCondicao}</p>
              <div className="d-flex flex-wrap gap-2 mb-3">
                {jogo.generos.map((genero) => <span key={genero.id} className="etiqueta-genero">{genero.nome}</span>)}
              </div>
              {jogo.descricao && <p>{jogo.descricao}</p>}
              <div className="detalhe-compra">
                <p className="preco-detalhe mb-1">{formatarPreco(jogo.preco)}</p>
                {esgotado
                  ? <span className="badge text-bg-danger">Esgotado</span>
                  : <span className="text-body-secondary">{jogo.estoque} em estoque</span>}
                <div className="d-flex flex-wrap gap-2 mt-3">
                  <button type="button" className="btn btn-primary" onClick={() => adicionar(jogo)}
                          disabled={esgotado || !jogo.ativo || atingiuLimite || finalizando}>
                    <Icone nome="carrinho" /> Adicionar ao carrinho
                  </button>
                  {quantidadeNoCarrinho > 0 && <Link className="btn btn-outline-dark" to="/carrinho">Ver carrinho</Link>}
                </div>
                <div role="status">
                  {quantidadeNoCarrinho > 0 && (
                    <p className="text-body-secondary mt-2 mb-0">
                      {quantidadeNoCarrinho === 1 ? '1 unidade no carrinho.' : `${quantidadeNoCarrinho} unidades no carrinho.`}
                      {atingiuLimite && ' Limite por jogo atingido.'}
                    </p>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

import { useCallback, useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../api/servicos.js';
import { obterMensagemErro } from '../api/erros.js';
import { formatarPreco } from '../lib/formato.js';
import { Carregando, MensagemErro } from '../components/MensagensEstado.jsx';

export default function DetalheJogo() {
  const { id } = useParams();
  const [jogo, setJogo] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);

  const carregar = useCallback(() => {
    setCarregando(true);
    setErro(null);
    jogoApi.consultar(id)
      .then(setJogo)
      .catch((e) => setErro(obterMensagemErro(e)))
      .finally(() => setCarregando(false));
  }, [id]);

  useEffect(() => { carregar(); }, [carregar]);

  if (carregando) return <Carregando />;
  if (erro) {
    return (
      <>
        <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />
        <Link to="/">Voltar ao catálogo</Link>
      </>
    );
  }

  const esgotado = jogo.estoque === 0;

  return (
    <>
      <Link to="/" className="d-inline-block mb-3">&larr; Voltar ao catálogo</Link>
      <div className="card shadow-sm">
        <div className="card-body">
          <span className="badge text-bg-dark mb-2">{jogo.nomePlataforma}</span>
          {!jogo.ativo && <span className="badge text-bg-warning ms-2">Inativo</span>}
          <h1 className="h3">{jogo.nome}</h1>
          <p className="text-muted">{jogo.anoLancamento} · Condição: {jogo.descricaoCondicao}</p>
          <p>{jogo.generos.map((g) => <span key={g.id} className="badge text-bg-secondary me-1">{g.nome}</span>)}</p>
          {jogo.descricao && <p>{jogo.descricao}</p>}
          <p className="fs-4 mb-1"><strong>{formatarPreco(jogo.preco)}</strong></p>
          {esgotado
            ? <span className="badge text-bg-danger">Esgotado</span>
            : <span className="text-muted">{jogo.estoque} em estoque</span>}
        </div>
      </div>
    </>
  );
}

import { Link } from 'react-router-dom';
import { formatarPreco } from '../lib/formato.js';
import Icone from './Icone.jsx';
import IlustracaoJogo from './IlustracaoJogo.jsx';

export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0;

  return (
    <article className="card card-jogo h-100">
      <IlustracaoJogo id={jogo.id} />
      <div className="card-body d-flex flex-column">
        <span className="etiqueta-plataforma align-self-start mb-2">{jogo.nomePlataforma}</span>
        <h2 className="h5 card-title">{jogo.nome}</h2>
        <p className="text-body-secondary small mb-2">{jogo.anoLancamento} · {jogo.descricaoCondicao}</p>
        <div className="d-flex flex-wrap gap-1 mb-3">
          {jogo.generos.map((genero) => <span key={genero} className="etiqueta-genero">{genero}</span>)}
        </div>
        <div className="mt-auto d-flex flex-wrap gap-2 align-items-center justify-content-between">
          <strong className="preco-jogo">{formatarPreco(jogo.preco)}</strong>
          {esgotado && <span className="badge text-bg-danger">Esgotado</span>}
        </div>
        <Link to={`/jogos/${jogo.id}`} className="link-acao stretched-link mt-1" aria-label={`Ver detalhes de ${jogo.nome}`}>
          Ver detalhes <Icone nome="direita" />
        </Link>
      </div>
    </article>
  );
}

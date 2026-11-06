import { Link } from 'react-router-dom';
import { formatarPreco } from '../lib/formato.js';

export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0;

  return (
    <div className="card card-jogo h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <span className="badge text-bg-dark align-self-start mb-2">{jogo.nomePlataforma}</span>
        <h2 className="h5 card-title">{jogo.nome}</h2>
        <p className="text-muted small mb-1">
          {jogo.anoLancamento} · {jogo.descricaoCondicao}
        </p>
        <p className="small mb-3">
          {jogo.generos.map((g) => (
            <span key={g.id} className="badge text-bg-secondary me-1">{g.nome}</span>
          ))}
        </p>
        <div className="mt-auto d-flex justify-content-between align-items-center">
          <strong>{formatarPreco(jogo.preco)}</strong>
          {esgotado && <span className="badge text-bg-danger">Esgotado</span>}
        </div>
        <Link to={`/jogos/${jogo.id}`} className="stretched-link" aria-label={`Ver detalhes de ${jogo.nome}`} />
      </div>
    </div>
  );
}

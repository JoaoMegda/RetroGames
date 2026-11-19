const LIMITE_NUMEROS = 5;

export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null;

  const inicio = Math.max(1, Math.min(pagina - 2, totalPaginas - LIMITE_NUMEROS + 1));
  const quantidade = Math.min(LIMITE_NUMEROS, totalPaginas);
  const numeros = Array.from({ length: quantidade }, (_, indice) => inicio + indice);

  return (
    <nav aria-label="Paginação" className="d-flex flex-wrap justify-content-center align-items-center gap-2 my-4">
      <button type="button" className="btn btn-outline-secondary" disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>
        Anterior
      </button>
      {numeros.map((numero) => (
        <button type="button" key={numero} className="btn btn-outline-secondary"
                aria-label={`Página ${numero}`} aria-current={numero === pagina ? 'page' : undefined}
                disabled={numero === pagina} onClick={() => aoMudar(numero)}>
          {numero}
        </button>
      ))}
      <button type="button" className="btn btn-outline-secondary" disabled={pagina >= totalPaginas} onClick={() => aoMudar(pagina + 1)}>
        Próxima
      </button>
    </nav>
  );
}

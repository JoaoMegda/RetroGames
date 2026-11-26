export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="painel-estado" role="status">
      <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />
      {texto}
    </div>
  );
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap gap-3 justify-content-between align-items-center" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  );
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="painel-estado">
      <h2 className="h5">{titulo}</h2>
      {children}
    </div>
  );
}

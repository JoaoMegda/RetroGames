import { useEffect, useState } from 'react';
import { Link, Navigate, NavLink, Outlet, useLocation } from 'react-router-dom';
import { obterMensagemErro } from '../api/erros.js';
import { useAutenticacao } from '../hooks/useAutenticacao.js';
import { MensagemErro } from './MensagensEstado.jsx';

export default function Layout() {
  const { estaAutenticado, ehAdministrador, usuario, sair } = useAutenticacao();
  const { pathname } = useLocation();
  const [saindo, setSaindo] = useState(false);
  const [erroSaida, setErroSaida] = useState(null);
  const voltarAoCatalogo = saindo && !estaAutenticado;

  useEffect(() => {
    if (voltarAoCatalogo && pathname === '/') setSaindo(false);
  }, [pathname, voltarAoCatalogo]);

  async function aoSair() {
    setErroSaida(null);
    setSaindo(true);
    try {
      await sair();
    } catch (falha) {
      setErroSaida(obterMensagemErro(falha));
      setSaindo(false);
    }
  }

  return (
    <div className="d-flex min-vh-100 flex-column">
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand" to="/">RetroGames</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu"
                  aria-controls="menu" aria-expanded="false" aria-label="Alternar navegação">
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse" id="menu">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink className="nav-link" to="/" end>Catálogo</NavLink>
              </li>
              {ehAdministrador && (
                <li className="nav-item dropdown">
                  <button type="button" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                    Administração
                  </button>
                  <ul className="dropdown-menu">
                    <li><Link className="dropdown-item" to="/admin/jogos">Jogos</Link></li>
                    <li><Link className="dropdown-item" to="/admin/plataformas">Plataformas</Link></li>
                  </ul>
                </li>
              )}
            </ul>
            <div className="d-flex align-items-center gap-2">
              {estaAutenticado ? (
                <>
                  <span className="navbar-text">Olá, {usuario.nome}</span>
                  <button type="button" className="btn btn-outline-light btn-sm" onClick={aoSair} disabled={saindo}>
                    {saindo ? 'Saindo...' : 'Sair'}
                  </button>
                </>
              ) : (
                <Link className="btn btn-outline-light btn-sm" to="/login">Entrar</Link>
              )}
            </div>
          </div>
        </div>
      </nav>
      <main className="container flex-grow-1 py-4">
        {estaAutenticado && erroSaida && <MensagemErro mensagem={erroSaida} aoTentarNovamente={aoSair} />}
        {voltarAoCatalogo && pathname !== '/' ? <Navigate to="/" replace /> : <Outlet />}
      </main>
      <footer className="border-top py-3 text-body-secondary">
        <div className="container small">RetroGames</div>
      </footer>
    </div>
  );
}

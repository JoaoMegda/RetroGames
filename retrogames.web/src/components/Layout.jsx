import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao.js';

export default function Layout() {
  const { estaAutenticado, ehAdministrador, usuario, sair } = useAutenticacao();
  const navegar = useNavigate();

  async function aoSair() {
    await sair().catch(() => {});
    navegar('/');
  }

  return (
    <>
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand" to="/">RETROGAMES</Link>
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
                  <a className="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    Administração
                  </a>
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
                  <span className="navbar-text">{usuario.nome}</span>
                  <button type="button" className="btn btn-outline-light btn-sm" onClick={aoSair}>Sair</button>
                </>
              ) : (
                <Link className="btn btn-outline-light btn-sm" to="/login">Entrar</Link>
              )}
            </div>
          </div>
        </div>
      </nav>
      <main className="container py-4">
        <Outlet />
      </main>
    </>
  );
}

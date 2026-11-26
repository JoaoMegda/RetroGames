import { useEffect, useRef, useState } from 'react';
import { Link, Navigate, NavLink, Outlet, useLocation } from 'react-router-dom';
import { obterMensagemErro } from '../api/erros.js';
import { useAutenticacao } from '../hooks/useAutenticacao.js';
import { useCarrinho } from '../hooks/useCarrinho.js';
import Icone from './Icone.jsx';
import { MensagemErro } from './MensagensEstado.jsx';

export default function Layout() {
  const { carregando, estaAutenticado, ehAdministrador, ehCliente, usuario, sair } = useAutenticacao();
  const { quantidadeTotal } = useCarrinho();
  const { key, pathname } = useLocation();
  const [menuAberto, setMenuAberto] = useState(false);
  const [saindo, setSaindo] = useState(false);
  const [erroSaida, setErroSaida] = useState(null);
  const voltarAoCatalogo = saindo && !estaAutenticado;
  const conteudo = useRef(null);
  const focarConteudo = useRef(false);

  function fecharMenuAoNavegar(evento) {
    if (!menuAberto || evento.metaKey || evento.ctrlKey || evento.altKey || evento.shiftKey) return;

    focarConteudo.current = window.matchMedia('(max-width: 991.98px)').matches;
    setMenuAberto(false);
  }

  useEffect(() => {
    if (!focarConteudo.current) return;

    focarConteudo.current = false;
    conteudo.current?.focus({ preventScroll: true });
  }, [key, menuAberto]);

  useEffect(() => {
    if (voltarAoCatalogo && pathname === '/') setSaindo(false);
  }, [pathname, voltarAoCatalogo]);

  async function aoSair() {
    setErroSaida(null);
    setSaindo(true);
    try {
      await sair();
      setMenuAberto(false);
    } catch (falha) {
      setErroSaida(obterMensagemErro(falha));
      setSaindo(false);
    }
  }

  return (
    <div className="d-flex min-vh-100 flex-column">
      <a className="visually-hidden-focusable atalho-conteudo" href="#conteudo">Ir para o conteúdo</a>
      <header className="cabecalho">
        <nav className="navbar navbar-expand-lg" aria-label="Navegação principal" data-bs-theme="dark">
          <div className="container">
            <Link className="navbar-brand marca" to="/" onClick={fecharMenuAoNavegar}>
              <span className="marca-simbolo"><Icone nome="controle" /></span>
              Retro<span>Games</span>
            </Link>
            <button className="navbar-toggler" type="button" onClick={() => setMenuAberto(!menuAberto)}
                    aria-controls="menu" aria-expanded={menuAberto} aria-label="Alternar navegação">
              <Icone nome="menu" />
            </button>
            <div className={`collapse navbar-collapse ${menuAberto ? 'show' : ''}`} id="menu">
              <ul className="navbar-nav me-auto">
                <li className="nav-item">
                  <NavLink className="nav-link" to="/" end onClick={fecharMenuAoNavegar}>Catálogo</NavLink>
                </li>
                {ehCliente && (
                  <>
                    <li className="nav-item">
                      <NavLink className="nav-link" to="/carrinho" onClick={fecharMenuAoNavegar}>
                        <Icone nome="carrinho" /> Carrinho
                        {quantidadeTotal > 0 && <span className="contador-carrinho">{quantidadeTotal}</span>}
                      </NavLink>
                    </li>
                    <li className="nav-item">
                      <NavLink className="nav-link" to="/meus-pedidos" onClick={fecharMenuAoNavegar}>
                        <Icone nome="pedido" /> Meus pedidos
                      </NavLink>
                    </li>
                  </>
                )}
                {ehAdministrador && (
                  <li className="nav-item dropdown">
                    <button type="button" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                      Administração
                    </button>
                    <ul className="dropdown-menu">
                      <li><Link className="dropdown-item" to="/admin/jogos" onClick={fecharMenuAoNavegar}>Jogos</Link></li>
                      <li><Link className="dropdown-item" to="/admin/plataformas" onClick={fecharMenuAoNavegar}>Plataformas</Link></li>
                    </ul>
                  </li>
                )}
              </ul>
              <div className="conta-nav">
                {estaAutenticado ? (
                  <>
                    <span className="navbar-text nome-usuario">Olá, {usuario.nome}</span>
                    <button type="button" className="btn btn-outline-light" onClick={aoSair} disabled={saindo || carregando}>
                      {saindo ? 'Saindo...' : 'Sair'}
                    </button>
                  </>
                ) : (
                  <Link className="btn btn-outline-light" to="/login" onClick={fecharMenuAoNavegar}>Entrar</Link>
                )}
              </div>
            </div>
          </div>
        </nav>
      </header>
      <main ref={conteudo} id="conteudo" className="container flex-grow-1 espaco-pagina" tabIndex={-1}>
        {estaAutenticado && erroSaida && <MensagemErro mensagem={erroSaida} aoTentarNovamente={aoSair} />}
        {voltarAoCatalogo && pathname !== '/' ? <Navigate to="/" replace /> : <Outlet />}
      </main>
      <footer className="rodape">
        <div className="container">RetroGames</div>
      </footer>
    </div>
  );
}

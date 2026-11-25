import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao.js';
import { Carregando } from './MensagensEstado.jsx';

// Só usabilidade: quem decide o acesso é a API (401/403).
export default function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador, usuario } = useAutenticacao();
  const local = useLocation();
  const acessoAtual = estaAutenticado && (!somenteAdministrador || ehAdministrador);

  if (carregando && !acessoAtual) return <Carregando texto="Verificando sessão..." />;

  if (!estaAutenticado) {
    const voltarPara = encodeURIComponent(local.pathname + local.search + local.hash);
    return <Navigate to={`/login?voltarPara=${voltarPara}`} replace />;
  }

  if (!acessoAtual) return <Navigate to="/" replace />;

  return (
    <>
      {carregando && <Carregando texto="Verificando sessão..." />}
      <div key={usuario.id} hidden={carregando}>
        <Outlet />
      </div>
    </>
  );
}

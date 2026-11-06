import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao.js';
import { Carregando } from './MensagensEstado.jsx';

// Só usabilidade: quem decide o acesso é a API (401/403).
export default function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao();
  const local = useLocation();

  if (carregando) return <Carregando />;

  if (!estaAutenticado) {
    const voltarPara = encodeURIComponent(local.pathname + local.search);
    return <Navigate to={`/login?voltarPara=${voltarPara}`} replace />;
  }

  if (somenteAdministrador && !ehAdministrador) return <Navigate to="/" replace />;

  return <Outlet />;
}

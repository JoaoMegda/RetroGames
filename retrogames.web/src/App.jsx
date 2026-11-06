import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout.jsx';
import RotaProtegida from './components/RotaProtegida.jsx';
import Catalogo from './pages/Catalogo.jsx';
import DetalheJogo from './pages/DetalheJogo.jsx';
import Login from './pages/Login.jsx';
import Jogos from './pages/Jogos.jsx';
import FormularioJogo from './pages/FormularioJogo.jsx';
import Plataforma from './pages/Plataforma.jsx';
import NaoEncontrada from './pages/NaoEncontrada.jsx';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* públicas */}
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />

        {/* restritas ao Administrador (a API é quem garante: 401/403) */}
        <Route element={<RotaProtegida somenteAdministrador />}>
          <Route path="admin/jogos" element={<Jogos />} />
          <Route path="admin/jogos/novo" element={<FormularioJogo />} />
          <Route path="admin/jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="admin/plataformas" element={<Plataforma />} />
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  );
}

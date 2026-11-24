import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout.jsx';
import RotaProtegida from './components/RotaProtegida.jsx';
import Catalogo from './pages/Catalogo/Catalogo.jsx';
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx';
import Login from './pages/Login/Login.jsx';
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
        <Route path="admin">
          <Route index element={<NaoEncontrada />} />
          <Route element={<RotaProtegida somenteAdministrador />}>
            <Route path="jogos" element={<Jogos />} />
            <Route path="jogos/novo" element={<FormularioJogo />} />
            <Route path="jogos/:id/editar" element={<FormularioJogo />} />
            <Route path="plataformas" element={<Plataforma />} />
          </Route>
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  );
}

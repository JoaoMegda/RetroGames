import { Navigate, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout.jsx';
import RotaProtegida from './components/RotaProtegida.jsx';
import Catalogo from './pages/Catalogo/Catalogo.jsx';
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx';
import Login from './pages/Login/Login.jsx';
import Cadastro from './pages/Login/Cadastro.jsx';
import Carrinho from './pages/Compras/Carrinho.jsx';
import MeusPedidos from './pages/Compras/MeusPedidos.jsx';
import Jogos from './pages/Administracao/Jogos.jsx';
import FormularioJogo from './pages/Administracao/FormularioJogo.jsx';
import Plataforma from './pages/Auxiliares/Plataforma.jsx';
import NaoEncontrada from './pages/NaoEncontrada.jsx';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route path="cadastro" element={<Cadastro />} />

        <Route element={<RotaProtegida />}>
          <Route path="carrinho" element={<Carrinho />} />
          <Route path="meus-pedidos" element={<MeusPedidos />} />
        </Route>

        <Route path="admin" element={<RotaProtegida somenteAdministrador />}>
          <Route index element={<Navigate to="jogos" replace />} />
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<FormularioJogo />} />
          <Route path="jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="plataformas" element={<Plataforma />} />
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  );
}

import { Link } from 'react-router-dom';
import { EstadoVazio } from '../components/MensagensEstado.jsx';

export default function NaoEncontrada() {
  return (
    <EstadoVazio titulo="Página não encontrada">
      <Link to="/" className="btn btn-dark mt-2">Voltar ao catálogo</Link>
    </EstadoVazio>
  );
}

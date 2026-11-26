import Icone from './Icone.jsx';

export default function IlustracaoJogo({ id }) {
  const tom = id % 4;

  return (
    <div className={`ilustracao-jogo tom-capa-${tom}`} aria-hidden="true">
      <div className="fita-jogo">
        <span className="fita-rotulo"><Icone nome="controle" /></span>
        <span className="fita-contatos" />
      </div>
    </div>
  );
}

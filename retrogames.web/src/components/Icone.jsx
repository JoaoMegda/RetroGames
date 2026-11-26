const desenhos = {
  controle: 'M7 7h10a4 4 0 0 1 3.9 3.2l1 4.7a2.4 2.4 0 0 1-4.1 2.1L15 14H9l-2.8 3a2.4 2.4 0 0 1-4.1-2.1l1-4.7A4 4 0 0 1 7 7Zm0 2v4m-2-2h4m7 0h.01m3 1.5h.01',
  carrinho: 'M2 3h3l3.2 12h10.2l2.6-8H6m3 13h.01m9 0h.01',
  pedido: 'M5 4h14v16l-3-2-4 2-4-2-3 2V4Zm3 5h8m-8 4h5',
  busca: 'M10.5 17a6.5 6.5 0 1 0 0-13 6.5 6.5 0 0 0 0 13Zm4.8-1.7L20 20',
  direita: 'M4 12h15m-6-6 6 6-6 6',
  esquerda: 'M20 12H5m6-6-6 6 6 6',
  confirmacao: 'm5 12 4 4L19 6',
  menu: 'M4 6h16M4 12h16M4 18h16',
};

export default function Icone({ nome }) {
  return (
    <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor"
         strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true" focusable="false">
      <path d={desenhos[nome]} />
    </svg>
  );
}

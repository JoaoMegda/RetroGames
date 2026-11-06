const formatadorPreco = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
const formatadorDataHora = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' });

export function formatarPreco(valor) {
  return formatadorPreco.format(Number(valor));
}

// A API envia datas em UTC; sem sufixo "Z" o JavaScript as trataria como hora local.
export function formatarDataHora(valor) {
  if (!valor) return '';
  const texto = /[zZ]|[+-]\d{2}:\d{2}$/.test(valor) ? valor : `${valor}Z`;
  return formatadorDataHora.format(new Date(texto));
}

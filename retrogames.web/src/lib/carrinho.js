export const QUANTIDADE_MAXIMA = 10;

function quantidadeValida(quantidade) {
  return Number.isInteger(quantidade) && quantidade >= 1 && quantidade <= QUANTIDADE_MAXIMA;
}

function jogoValido(jogo) {
  return jogo && Number.isSafeInteger(jogo.id) && jogo.id > 0
    && typeof jogo.nome === 'string' && jogo.nome.trim().length > 0
    && Number.isFinite(jogo.preco) && jogo.preco >= 0;
}

export function lerCarrinho(texto) {
  let dados;
  try {
    dados = JSON.parse(texto);
  } catch {
    return [];
  }

  if (!Array.isArray(dados)) return [];

  const itens = new Map();
  for (const jogo of dados) {
    if (!jogoValido(jogo) || !quantidadeValida(jogo.quantidade) || itens.has(jogo.id)) continue;

    const { id, nome, preco, quantidade } = jogo;
    itens.set(id, { id, nome, preco, quantidade });
  }
  return [...itens.values()];
}

export function adicionarJogo(itens, jogo) {
  if (!jogoValido(jogo)) return itens;

  const { id, nome, preco } = jogo;
  const existente = itens.find((item) => item.id === id);
  if (!existente) return [...itens, { id, nome, preco, quantidade: 1 }];
  if (existente.quantidade === QUANTIDADE_MAXIMA) return itens;

  return itens.map((item) => item.id === id ? { id, nome, preco, quantidade: item.quantidade + 1 } : item);
}

export function alterarQuantidade(itens, id, quantidade) {
  if (!quantidadeValida(quantidade)) return itens;

  return itens.map((item) => item.id === id ? { ...item, quantidade } : item);
}

export function calcularTotal(itens) {
  const centavos = itens.reduce((total, item) => total + Math.round(item.preco * 100) * item.quantidade, 0);
  return centavos / 100;
}

export function montarItensPedido(itens) {
  return itens.map(({ id, quantidade }) => ({ idJogo: id, quantidade }));
}

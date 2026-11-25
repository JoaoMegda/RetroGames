import { useEffect, useRef, useState } from 'react';
import { pedidoApi } from '../api/servicos.js';
import {
  adicionarJogo, alterarQuantidade, calcularTotal, lerCarrinho, montarItensPedido,
} from '../lib/carrinho.js';
import { CarrinhoContext } from './carrinhoContext.js';

const CHAVE_CARRINHO = 'retrogames.carrinho';

function recuperarItens() {
  try {
    return lerCarrinho(window.localStorage.getItem(CHAVE_CARRINHO));
  } catch {
    return [];
  }
}

export default function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(recuperarItens);
  const [finalizando, setFinalizando] = useState(false);
  const envioPendente = useRef(false);

  useEffect(() => {
    try {
      if (itens.length > 0) {
        window.localStorage.setItem(CHAVE_CARRINHO, JSON.stringify(itens));
      } else {
        window.localStorage.removeItem(CHAVE_CARRINHO);
      }
    } catch {
      // O carrinho continua funcionando durante a sessão se o navegador bloquear o armazenamento.
    }
  }, [itens]);

  async function finalizar(idUsuario) {
    if (envioPendente.current || itens.length === 0) return;

    envioPendente.current = true;
    setFinalizando(true);
    try {
      const pedido = await pedidoApi.criar(montarItensPedido(itens), idUsuario);
      setItens([]);
      return pedido;
    } finally {
      envioPendente.current = false;
      setFinalizando(false);
    }
  }

  function adicionar(jogo) {
    if (!envioPendente.current) setItens((anteriores) => adicionarJogo(anteriores, jogo));
  }

  function alterar(id, quantidade) {
    if (!envioPendente.current) setItens((anteriores) => alterarQuantidade(anteriores, id, quantidade));
  }

  function remover(id) {
    if (!envioPendente.current) setItens((anteriores) => anteriores.filter((item) => item.id !== id));
  }

  const valor = {
    itens,
    total: calcularTotal(itens),
    quantidadeTotal: itens.reduce((total, item) => total + item.quantidade, 0),
    adicionar,
    alterar,
    remover,
    finalizar,
    finalizando,
  };

  return <CarrinhoContext.Provider value={valor}>{children}</CarrinhoContext.Provider>;
}

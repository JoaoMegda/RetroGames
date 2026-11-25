import { useContext } from 'react';
import { CarrinhoContext } from '../contexts/carrinhoContext.js';

export function useCarrinho() {
  return useContext(CarrinhoContext);
}

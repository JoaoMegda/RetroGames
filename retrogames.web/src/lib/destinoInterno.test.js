import test from 'node:test';
import assert from 'node:assert/strict';
import { obterDestinoInterno } from './destinoInterno.js';

const origem = 'https://retrogames.example';

test('preserva caminho, filtros e âncora ao retornar da autenticação', () => {
  const destino = '/meus-pedidos?pagina=2#pedido-5';
  assert.equal(obterDestinoInterno(destino, origem), destino);
});

test('recusa endereços que saem da loja ou retornam à autenticação', () => {
  for (const valor of [
    null,
    'https://outro.example',
    '//outro.example',
    '/\\outro.example',
    '/login?voltarPara=/carrinho',
    '/cadastro/',
    '/pasta/../login',
  ]) {
    assert.equal(obterDestinoInterno(valor, origem), '/');
  }
});

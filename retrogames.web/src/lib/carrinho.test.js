import test from 'node:test';
import assert from 'node:assert/strict';
import { adicionarJogo, alterarQuantidade, calcularTotal, lerCarrinho, montarItensPedido } from './carrinho.js';

const jogo = { id: 5, nome: 'Fase Lunar', preco: 29.95 };

test('recupera itens válidos e ignora dados corrompidos ou duplicados', () => {
  assert.deepEqual(lerCarrinho('{"itens":null}'), []);
  assert.deepEqual(lerCarrinho('[JSON incompleto'), []);
  assert.deepEqual(lerCarrinho(JSON.stringify([
    { ...jogo, quantidade: 2 },
    { ...jogo, quantidade: 3 },
    { ...jogo, id: 6, quantidade: 11 },
    { ...jogo, id: 7, quantidade: 2.5 },
    { ...jogo, id: 8, quantidade: 0 },
    { ...jogo, id: 9, preco: -1, quantidade: 1 },
    { ...jogo, id: '10', quantidade: 1 },
    { ...jogo, id: 11, nome: ' ', quantidade: 1 },
    null,
  ])), [{ ...jogo, quantidade: 2 }]);
});

test('adiciona um jogo e soma unidades até dez', () => {
  assert.deepEqual(adicionarJogo([], jogo), [{ ...jogo, quantidade: 1 }]);
  assert.deepEqual(adicionarJogo([{ ...jogo, quantidade: 9 }], jogo), [{ ...jogo, quantidade: 10 }]);
  const carrinhoCheio = [{ ...jogo, quantidade: 10 }];
  assert.equal(adicionarJogo(carrinhoCheio, jogo), carrinhoCheio);
});

test('aceita somente quantidades inteiras de uma a dez', () => {
  const itens = [{ ...jogo, quantidade: 1 }];

  for (const invalida of [0, 11, 0.5, NaN, '2']) {
    assert.equal(alterarQuantidade(itens, jogo.id, invalida), itens);
  }
  assert.deepEqual(alterarQuantidade(itens, jogo.id, 10), [{ ...jogo, quantidade: 10 }]);
});

test('calcula o total estimado sem erro de ponto flutuante', () => {
  assert.equal(calcularTotal([
    { ...jogo, preco: 0.1, quantidade: 1 },
    { ...jogo, preco: 0.2, quantidade: 1 },
  ]), 0.3);
});

test('envia somente o identificador e a quantidade de cada item', () => {
  assert.deepEqual(montarItensPedido([{ ...jogo, quantidade: 2 }]), [{ idJogo: 5, quantidade: 2 }]);
});

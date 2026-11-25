import test from 'node:test';
import assert from 'node:assert/strict';
import axios from 'axios';
import { SessaoAlteradaErro } from './erros.js';

const chamadas = [];
let idUsuarioAtual;
const adaptadorOriginal = axios.defaults.adapter;

axios.defaults.adapter = async (requisicao) => {
  chamadas.push({ rota: requisicao.url, token: requisicao.headers.get('X-CSRF-TOKEN') });

  if (requisicao.url === '/Autenticacao/csrf') {
    return { status: 200, data: { data: 'token-da-sessao' } };
  }
  if (requisicao.url === '/Autenticacao/eu') {
    return idUsuarioAtual
      ? { status: 200, data: { data: { id: idUsuarioAtual } } }
      : { status: 401, data: { responseCode: 4 } };
  }
  return { status: 201, data: { data: { id: 12 } } };
};

const { default: api } = await import('./api.js');
axios.defaults.adapter = adaptadorOriginal;

test('confere a identidade após buscar o token e envia o pedido com o mesmo token', async () => {
  chamadas.length = 0;
  idUsuarioAtual = 5;

  const resposta = await api.post('/Pedido', { itens: [] }, { idUsuarioEsperado: 5 });

  assert.equal(resposta.data.data.id, 12);
  assert.deepEqual(chamadas, [
    { rota: '/Autenticacao/csrf', token: undefined },
    { rota: '/Autenticacao/eu', token: undefined },
    { rota: '/Pedido', token: 'token-da-sessao' },
  ]);
});

test('impede o envio do pedido quando outra aba alterou ou encerrou a sessão', async () => {
  globalThis.window = new EventTarget();
  let verificacoes = 0;
  window.addEventListener('sessao:verificar', () => { verificacoes += 1; });

  try {
    for (const id of [7, null]) {
      chamadas.length = 0;
      idUsuarioAtual = id;
      await assert.rejects(
        api.post('/Pedido', { itens: [] }, { idUsuarioEsperado: 5 }),
        SessaoAlteradaErro,
      );
      assert.deepEqual(chamadas.map(({ rota }) => rota), ['/Autenticacao/csrf', '/Autenticacao/eu']);
    }
    assert.equal(verificacoes, 2);
  } finally {
    delete globalThis.window;
  }
});

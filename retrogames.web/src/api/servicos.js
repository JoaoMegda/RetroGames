import api from './api.js';

// Cada função devolve o campo "data" do envelope { responseCode, data, errorMessage }.
const dados = (promessa) => promessa.then((resposta) => resposta.data.data);

export const autenticacaoApi = {
  usuarioAtual: (sinal) => dados(api.get('/Autenticacao/eu', { signal: sinal })),
  entrar: (email, senha) => dados(api.post('/Autenticacao/login', { email, senha })),
  sair: () => dados(api.post('/Autenticacao/logout')),
};

export const plataformaApi = {
  listar: () => dados(api.get('/Plataforma')),
  consultar: (id) => dados(api.get(`/Plataforma/${id}`)),
  inserir: (plataforma) => dados(api.post('/Plataforma', plataforma)),
  alterar: (id, plataforma) => dados(api.put(`/Plataforma/${id}`, plataforma)),
  excluir: (id) => dados(api.delete(`/Plataforma/${id}`)),
};

export const generoApi = {
  listar: () => dados(api.get('/Genero')),
};

export const condicaoJogoApi = {
  listar: () => dados(api.get('/CondicaoJogo')),
};

export const jogoApi = {
  listar: (parametros) => dados(api.get('/Jogo', { params: parametros })),
  buscarPorId: (id) => dados(api.get(`/Jogo/${id}`)),
  inserir: (jogo) => dados(api.post('/Jogo', jogo)),
  alterar: (id, jogo) => dados(api.put(`/Jogo/${id}`, jogo)),
  desativar: (id) => dados(api.delete(`/Jogo/${id}`)),
};

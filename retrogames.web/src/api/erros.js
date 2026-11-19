const MENSAGEM_PADRAO = 'Não foi possível concluir a operação. Tente novamente.';
const MENSAGEM_CONEXAO = 'Não foi possível conectar ao servidor.';

export function obterMensagemErro(erro) {
  const resposta = erro?.response;
  // Quando a API está fora do ar, o proxy pode responder 5xx sem envelope.
  const falhaDeConexao = !resposta || (resposta.status >= 500 && resposta.data?.responseCode === undefined);
  return resposta?.data?.errorMessage ?? (falhaDeConexao ? MENSAGEM_CONEXAO : MENSAGEM_PADRAO);
}

/** Erros por campo (responseCode 3): { nome: ['Informe o nome.'], ... }. Vazio nos demais casos. */
export function obterErrosDosCampos(erro) {
  const corpo = erro?.response?.data;
  return corpo?.responseCode === 3 && corpo.data ? corpo.data : {};
}

const MENSAGEM_PADRAO = 'Não foi possível concluir a operação. Tente novamente.';
const MENSAGEM_CONEXAO = 'Não foi possível conectar ao servidor.';

export function obterMensagemErro(erro) {
  const resposta = erro?.response;
  // Quando a API está fora do ar, o proxy pode responder 5xx sem envelope.
  const falhaDeConexao = !resposta || (resposta.status >= 500 && resposta.data?.responseCode === undefined);
  return resposta?.data?.errorMessage ?? (falhaDeConexao ? MENSAGEM_CONEXAO : MENSAGEM_PADRAO);
}

/** Converte as listas de validação da API em um texto por campo do formulário. */
export function obterErrosDosCampos(erro) {
  const corpo = erro?.response?.data;
  if (corpo?.responseCode !== 3 || !corpo.data) return {};

  return Object.fromEntries(Object.entries(corpo.data).map(([campo, mensagens]) => [
    campo,
    Array.isArray(mensagens) ? mensagens.join(' ') : mensagens,
  ]));
}

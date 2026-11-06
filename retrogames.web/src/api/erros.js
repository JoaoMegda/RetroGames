const MENSAGEM_PADRAO = 'Não foi possível concluir a operação. Tente novamente.';

export function obterMensagemErro(erro) {
  return erro?.response?.data?.errorMessage ?? MENSAGEM_PADRAO;
}

/** Erros por campo (responseCode 3): { nome: ['Informe o nome.'], ... }. Vazio nos demais casos. */
export function obterErrosDosCampos(erro) {
  const corpo = erro?.response?.data;
  return corpo?.responseCode === 3 && corpo.data ? corpo.data : {};
}

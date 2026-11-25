import axios from 'axios';
import { SessaoAlteradaErro } from './erros.js';

const configuracao = { baseURL: '/api', withCredentials: true, withXSRFToken: false };
const api = axios.create(configuracao);
const clienteDireto = axios.create(configuracao);
const METODOS_COM_CSRF = ['post', 'put', 'delete'];
const ROTAS_COM_401_ESPERADO = ['/Autenticacao/eu', '/Autenticacao/login', '/Autenticacao/cadastrar'];

api.interceptors.request.use(async (requisicao) => {
  if (METODOS_COM_CSRF.includes(requisicao.method)) {
    // A identidade do cookie pode mudar após login, cadastro ou logout.
    const opcoes = { signal: requisicao.signal };
    const resposta = await clienteDireto.get('/Autenticacao/csrf', opcoes);
    const token = resposta.data?.data;
    if (typeof token !== 'string' || !token) throw new Error('Token CSRF indisponível.');

    if (requisicao.idUsuarioEsperado !== undefined) {
      // O token antecede a conferência. Se o cookie mudar depois, a API invalida o token.
      const sessao = await clienteDireto.get('/Autenticacao/eu', {
        ...opcoes,
        headers: { 'Cache-Control': 'no-cache' },
        validateStatus: (status) => status === 200 || status === 401,
      });
      if (sessao.data?.data?.id !== requisicao.idUsuarioEsperado) {
        window.dispatchEvent(new Event('sessao:verificar'));
        throw new SessaoAlteradaErro();
      }
    }

    requisicao.headers.set('X-CSRF-TOKEN', token);
  }

  return requisicao;
});

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const { response, config } = erro;
    const rota = config?.url?.split('?')[0];

    // Em /eu, 401 identifica um visitante. No login, indica credenciais inválidas.
    if (response?.status === 401 && !ROTAS_COM_401_ESPERADO.includes(rota)) {
      window.dispatchEvent(new Event('sessao:verificar'));
    }

    return Promise.reject(erro);
  },
);

export default api;

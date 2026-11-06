import axios from 'axios';

const METODOS_COM_CSRF = ['post', 'put', 'delete'];

const api = axios.create({ baseURL: '/api', withCredentials: true });

function lerCookie(nome) {
  const par = document.cookie.split('; ').find((c) => c.startsWith(`${nome}=`));
  return par ? decodeURIComponent(par.substring(nome.length + 1)) : null;
}

/** Pede um novo token ao servidor (cookie XSRF-TOKEN). Chamar após login e logout (RS02). */
export async function renovarCsrf() {
  await axios.get('/api/Autenticacao/csrf', { withCredentials: true });
}

const ehMutacao = (config) => METODOS_COM_CSRF.includes((config?.method ?? '').toLowerCase());

api.interceptors.request.use(async (config) => {
  if (ehMutacao(config)) {
    if (!lerCookie('XSRF-TOKEN')) await renovarCsrf();
    config.headers['X-CSRF-TOKEN'] = lerCookie('XSRF-TOKEN');
  }
  return config;
});

api.interceptors.response.use(
  (resposta) => resposta,
  async (erro) => {
    const { response, config } = erro;

    // Token recusado: o ASP.NET responde 400 sem o envelope (sem responseCode). Renova e repete uma única vez.
    if (response?.status === 400 && response.data?.responseCode === undefined && ehMutacao(config) && !config._repetida) {
      config._repetida = true;
      await renovarCsrf();
      config.headers['X-CSRF-TOKEN'] = lerCookie('XSRF-TOKEN');
      return api(config);
    }

    // Sessão expirada. No login, 401 significa credenciais inválidas e é tratado pela própria tela.
    if (response?.status === 401 && !config?.url?.endsWith('/Autenticacao/login')) {
      window.dispatchEvent(new Event('sessao:expirada'));
    }

    return Promise.reject(erro);
  },
);

export default api;

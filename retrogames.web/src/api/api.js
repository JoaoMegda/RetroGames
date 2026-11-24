import axios from 'axios';

const api = axios.create({ baseURL: '/api', withCredentials: true });
const ROTAS_COM_401_ESPERADO = ['/Autenticacao/eu', '/Autenticacao/login'];

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const { response, config } = erro;
    const rota = config?.url?.split('?')[0];

    // Em /eu, 401 identifica um visitante. No login, indica credenciais inválidas.
    if (response?.status === 401 && !ROTAS_COM_401_ESPERADO.includes(rota)) {
      window.dispatchEvent(new Event('sessao:expirada'));
    }

    return Promise.reject(erro);
  },
);

export default api;

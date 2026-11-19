import axios from 'axios';

const api = axios.create({ baseURL: '/api', withCredentials: true });

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const { response, config } = erro;

    // Sessão expirada. No login, 401 significa credenciais inválidas e é tratado pela própria tela.
    if (response?.status === 401 && !config?.url?.endsWith('/Autenticacao/login')) {
      window.dispatchEvent(new Event('sessao:expirada'));
    }

    return Promise.reject(erro);
  },
);

export default api;

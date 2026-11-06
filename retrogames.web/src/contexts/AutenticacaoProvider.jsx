import { useCallback, useEffect, useMemo, useState } from 'react';
import { AutenticacaoContext } from './autenticacaoContext.js';
import { autenticacaoApi } from '../api/servicos.js';
import { renovarCsrf } from '../api/api.js';

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null);
  const [carregando, setCarregando] = useState(true);

  // Ao iniciar, pergunta à API quem está logado (GET /api/Autenticacao/eu).
  useEffect(() => {
    let ativo = true;
    autenticacaoApi
      .eu()
      .then((dados) => ativo && setUsuario(dados))
      .catch(() => ativo && setUsuario(null))
      .finally(() => ativo && setCarregando(false));
    return () => { ativo = false; };
  }, []);

  useEffect(() => {
    const limpar = () => setUsuario(null);
    window.addEventListener('sessao:expirada', limpar);
    return () => window.removeEventListener('sessao:expirada', limpar);
  }, []);

  const entrar = useCallback(async (email, senha) => {
    const dados = await autenticacaoApi.entrar(email, senha);
    await renovarCsrf(); // o token antiforgery é atrelado à identidade, que mudou
    setUsuario(dados);
    return dados;
  }, []);

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair();
    } finally {
      setUsuario(null);
      await renovarCsrf();
    }
  }, []);

  const valor = useMemo(
    () => ({
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.descricaoPerfil === 'Administrador',
      entrar,
      sair,
    }),
    [usuario, carregando, entrar, sair],
  );

  return <AutenticacaoContext.Provider value={valor}>{children}</AutenticacaoContext.Provider>;
}

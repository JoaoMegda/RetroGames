import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { AutenticacaoContext } from './autenticacaoContext.js';
import { autenticacaoApi } from '../api/servicos.js';

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const consultaInicial = useRef(null);

  const atualizarUsuario = useCallback((dados) => {
    // Uma mudança de sessão invalida a consulta inicial que ainda estiver em andamento.
    consultaInicial.current?.abort();
    consultaInicial.current = null;
    setUsuario(dados);
    setCarregando(false);
  }, []);

  useEffect(() => {
    const consulta = new AbortController();
    consultaInicial.current = consulta;
    const limpar = () => atualizarUsuario(null);
    window.addEventListener('sessao:expirada', limpar);

    autenticacaoApi.usuarioAtual(consulta.signal)
      .catch(() => null)
      .then((dados) => {
        if (!consulta.signal.aborted) atualizarUsuario(dados);
      });

    return () => {
      consulta.abort();
      window.removeEventListener('sessao:expirada', limpar);
    };
  }, [atualizarUsuario]);

  const entrar = useCallback(async (email, senha) => {
    const dados = await autenticacaoApi.entrar(email, senha);
    atualizarUsuario(dados);
    return dados;
  }, [atualizarUsuario]);

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair();
    } finally {
      atualizarUsuario(null);
    }
  }, [atualizarUsuario]);

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

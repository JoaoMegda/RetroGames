import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { AutenticacaoContext } from './autenticacaoContext.js';
import { autenticacaoApi } from '../api/servicos.js';

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const consultaAtiva = useRef(null);
  const canalSessao = useRef(null);

  const atualizarUsuario = useCallback((dados) => {
    consultaAtiva.current?.abort();
    consultaAtiva.current = null;
    setUsuario(dados);
    setCarregando(false);
  }, []);

  useEffect(() => {
    function verificarSessao(reiniciar = false) {
      if (consultaAtiva.current && !reiniciar) return;

      consultaAtiva.current?.abort();
      const consulta = new AbortController();
      consultaAtiva.current = consulta;
      setCarregando(true);

      autenticacaoApi.usuarioAtual(consulta.signal)
        .catch(() => null)
        .then((dados) => {
          if (!consulta.signal.aborted) atualizarUsuario(dados);
        });
    }

    const aoReceberMudanca = () => verificarSessao(true);
    const aoReceberFoco = () => verificarSessao();
    const aoMudarVisibilidade = () => {
      if (document.visibilityState === 'visible') verificarSessao();
    };
    const canal = typeof BroadcastChannel === 'function' ? new BroadcastChannel('retrogames.sessao') : null;
    canalSessao.current = canal;
    canal?.addEventListener('message', aoReceberMudanca);
    window.addEventListener('sessao:verificar', aoReceberMudanca);
    window.addEventListener('focus', aoReceberFoco);
    document.addEventListener('visibilitychange', aoMudarVisibilidade);
    verificarSessao();

    return () => {
      consultaAtiva.current?.abort();
      consultaAtiva.current = null;
      canal?.close();
      canalSessao.current = null;
      window.removeEventListener('sessao:verificar', aoReceberMudanca);
      window.removeEventListener('focus', aoReceberFoco);
      document.removeEventListener('visibilitychange', aoMudarVisibilidade);
    };
  }, [atualizarUsuario]);

  const registrarMudanca = useCallback((dados) => {
    atualizarUsuario(dados);
    canalSessao.current?.postMessage('mudanca');
  }, [atualizarUsuario]);

  const entrar = useCallback(async (email, senha) => {
    const dados = await autenticacaoApi.entrar(email, senha);
    registrarMudanca(dados);
    return dados;
  }, [registrarMudanca]);

  const cadastrar = useCallback(async (cadastro) => {
    const dados = await autenticacaoApi.cadastrar(cadastro);
    registrarMudanca(dados);
  }, [registrarMudanca]);

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair();
    } catch (falha) {
      if (falha?.response?.status !== 401) throw falha;
    }
    registrarMudanca(null);
  }, [registrarMudanca]);

  const valor = useMemo(
    () => ({
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.descricaoPerfil === 'Administrador',
      ehCliente: usuario?.idPerfil === 2,
      entrar,
      cadastrar,
      sair,
    }),
    [usuario, carregando, entrar, cadastrar, sair],
  );

  return <AutenticacaoContext.Provider value={valor}>{children}</AutenticacaoContext.Provider>;
}

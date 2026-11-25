import { useState } from 'react';
import { Link, Navigate, useSearchParams } from 'react-router-dom';
import { useAutenticacao } from '../../hooks/useAutenticacao.js';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx';
import { obterDestinoInterno } from '../../lib/destinoInterno.js';

export default function Login() {
  const { entrar, estaAutenticado, carregando } = useAutenticacao();
  const [params] = useSearchParams();
  const destino = obterDestinoInterno(params.get('voltarPara'), window.location.origin);
  const linkCadastro = destino === '/' ? '/cadastro' : `/cadastro?voltarPara=${encodeURIComponent(destino)}`;

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState(null);
  const [errosCampos, setErrosCampos] = useState({});
  const [enviando, setEnviando] = useState(false);

  if (carregando) return <Carregando texto="Verificando sessão..." />;
  if (estaAutenticado) return <Navigate to={destino} replace />;

  async function aoEnviar(evento) {
    evento.preventDefault();
    setErro(null);
    setErrosCampos({});
    setEnviando(true);
    try {
      await entrar(email, senha);
    } catch (falha) {
      setErrosCampos(obterErrosDosCampos(falha));
      setErro(obterMensagemErro(falha));
    } finally {
      setEnviando(false);
    }
  }

  return (
    <section className="painel-autenticacao card mx-auto">
      <div className="card-body">
        <h1 className="titulo-pagina">Entrar</h1>
        {erro && <MensagemErro mensagem={erro} />}
        <form onSubmit={aoEnviar} noValidate>
          <fieldset disabled={enviando}>
            <div className="mb-3">
              <label className="form-label" htmlFor="email">E-mail</label>
              <input id="email" type="email" autoComplete="username" required
                     className={`form-control ${errosCampos.email ? 'is-invalid' : ''}`}
                     aria-invalid={Boolean(errosCampos.email)}
                     aria-describedby={errosCampos.email ? 'email-erro' : undefined}
                     value={email} onChange={(evento) => setEmail(evento.target.value)} />
              {errosCampos.email && <div className="invalid-feedback" id="email-erro">{errosCampos.email}</div>}
            </div>
            <div className="mb-4">
              <label className="form-label" htmlFor="senha">Senha</label>
              <input id="senha" type="password" autoComplete="current-password" required
                     className={`form-control ${errosCampos.senha ? 'is-invalid' : ''}`}
                     aria-invalid={Boolean(errosCampos.senha)}
                     aria-describedby={errosCampos.senha ? 'senha-erro' : undefined}
                     value={senha} onChange={(evento) => setSenha(evento.target.value)} />
              {errosCampos.senha && <div className="invalid-feedback" id="senha-erro">{errosCampos.senha}</div>}
            </div>
            <button className="btn btn-primary w-100" type="submit">
              {enviando ? 'Entrando...' : 'Entrar'}
            </button>
          </fieldset>
        </form>
        <p className="mb-0 mt-4">Primeira vez por aqui? <Link to={linkCadastro}>Criar conta</Link></p>
      </div>
    </section>
  );
}

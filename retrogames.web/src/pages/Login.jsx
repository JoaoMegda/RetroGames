import { useState } from 'react';
import { Navigate, useNavigate, useSearchParams } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao.js';
import { obterErrosDosCampos, obterMensagemErro } from '../api/erros.js';

export default function Login() {
  const { entrar, estaAutenticado, carregando } = useAutenticacao();
  const navegar = useNavigate();
  const [params] = useSearchParams();
  const voltarPara = params.get('voltarPara');
  // Só aceita caminhos internos, para não redirecionar o usuário a outro site.
  const destino = voltarPara && voltarPara.startsWith('/') && !voltarPara.startsWith('//') ? voltarPara : '/';

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState(null);
  const [errosCampos, setErrosCampos] = useState({});
  const [enviando, setEnviando] = useState(false);

  if (!carregando && estaAutenticado) return <Navigate to={destino} replace />;

  async function aoEnviar(evento) {
    evento.preventDefault();
    setErro(null);
    setErrosCampos({});
    setEnviando(true);
    try {
      await entrar(email, senha);
      navegar(destino, { replace: true });
    } catch (e) {
      setErrosCampos(obterErrosDosCampos(e));
      setErro(obterMensagemErro(e));
    } finally {
      setEnviando(false);
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-md-5 col-lg-4">
        <h1 className="h3 mb-3">Entrar</h1>
        {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
        <form onSubmit={aoEnviar} noValidate>
          <div className="mb-3">
            <label className="form-label" htmlFor="email">E-mail</label>
            <input id="email" type="email" autoComplete="username"
                   className={`form-control ${errosCampos.email ? 'is-invalid' : ''}`}
                   value={email} onChange={(e) => setEmail(e.target.value)} />
            {errosCampos.email && <div className="invalid-feedback">{errosCampos.email.join(' ')}</div>}
          </div>
          <div className="mb-3">
            <label className="form-label" htmlFor="senha">Senha</label>
            <input id="senha" type="password" autoComplete="current-password"
                   className={`form-control ${errosCampos.senha ? 'is-invalid' : ''}`}
                   value={senha} onChange={(e) => setSenha(e.target.value)} />
            {errosCampos.senha && <div className="invalid-feedback">{errosCampos.senha.join(' ')}</div>}
          </div>
          <button className="btn btn-dark w-100" type="submit" disabled={enviando}>
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  );
}

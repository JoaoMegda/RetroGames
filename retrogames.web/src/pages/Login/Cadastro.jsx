import { useState } from 'react';
import { Link, Navigate, useSearchParams } from 'react-router-dom';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx';
import { useAutenticacao } from '../../hooks/useAutenticacao.js';
import { obterDestinoInterno } from '../../lib/destinoInterno.js';

export default function Cadastro() {
  const { cadastrar, estaAutenticado, carregando } = useAutenticacao();
  const [params] = useSearchParams();
  const destino = obterDestinoInterno(params.get('voltarPara'), window.location.origin);
  const linkLogin = destino === '/' ? '/login' : `/login?voltarPara=${encodeURIComponent(destino)}`;

  const [cadastro, setCadastro] = useState({ nome: '', email: '', senha: '' });
  const [erro, setErro] = useState(null);
  const [errosCampos, setErrosCampos] = useState({});
  const [enviando, setEnviando] = useState(false);

  if (carregando) return <Carregando texto="Verificando sessão..." />;
  if (estaAutenticado) return <Navigate to={destino} replace />;

  function atualizarCampo(evento) {
    const { name, value } = evento.target;
    setCadastro((anterior) => ({ ...anterior, [name]: value }));
  }

  async function aoEnviar(evento) {
    evento.preventDefault();
    setErro(null);
    setErrosCampos({});
    setEnviando(true);
    try {
      await cadastrar({ ...cadastro, nome: cadastro.nome.trim(), email: cadastro.email.trim() });
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
        <h1 className="titulo-pagina">Criar conta</h1>
        {erro && <MensagemErro mensagem={erro} />}
        <form onSubmit={aoEnviar}>
          <fieldset disabled={enviando}>
            <div className="mb-3">
              <label className="form-label" htmlFor="nome">Nome</label>
              <input id="nome" name="nome" type="text" autoComplete="name" maxLength={100} required
                     className={`form-control ${errosCampos.nome ? 'is-invalid' : ''}`}
                     aria-invalid={Boolean(errosCampos.nome)}
                     aria-describedby={errosCampos.nome ? 'nome-erro' : undefined}
                     value={cadastro.nome} onChange={atualizarCampo} />
              {errosCampos.nome && <div className="invalid-feedback" id="nome-erro">{errosCampos.nome}</div>}
            </div>
            <div className="mb-3">
              <label className="form-label" htmlFor="email">E-mail</label>
              <input id="email" name="email" type="email" autoComplete="email" maxLength={150} required
                     className={`form-control ${errosCampos.email ? 'is-invalid' : ''}`}
                     aria-invalid={Boolean(errosCampos.email)}
                     aria-describedby={errosCampos.email ? 'email-erro' : undefined}
                     value={cadastro.email} onChange={atualizarCampo} />
              {errosCampos.email && <div className="invalid-feedback" id="email-erro">{errosCampos.email}</div>}
            </div>
            <div className="mb-4">
              <label className="form-label" htmlFor="senha">Senha</label>
              <input id="senha" name="senha" type="password" autoComplete="new-password" minLength={8} maxLength={128} required
                     className={`form-control ${errosCampos.senha ? 'is-invalid' : ''}`}
                     aria-invalid={Boolean(errosCampos.senha)}
                     aria-describedby={errosCampos.senha ? 'senha-ajuda senha-erro' : 'senha-ajuda'}
                     value={cadastro.senha} onChange={atualizarCampo} />
              <div className="form-text" id="senha-ajuda">Use de 8 a 128 caracteres.</div>
              {errosCampos.senha && <div className="invalid-feedback" id="senha-erro">{errosCampos.senha}</div>}
            </div>
            <button className="btn btn-primary w-100" type="submit">
              {enviando ? 'Criando conta...' : 'Criar conta'}
            </button>
          </fieldset>
        </form>
        <p className="mb-0 mt-4">Já tem uma conta? <Link to={linkLogin}>Entrar</Link></p>
      </div>
    </section>
  );
}

import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { generoApi, jogoApi, plataformaApi, tabelaTipoApi } from '../api/servicos.js';
import { obterErrosDosCampos, obterMensagemErro } from '../api/erros.js';
import { Carregando, MensagemErro } from '../components/MensagensEstado.jsx';

const VAZIO = {
  nome: '', descricao: '', anoLancamento: '', preco: '', estoque: '',
  idPlataforma: '', idCondicaoJogo: '', idsGenero: [],
};

export default function FormularioJogo() {
  const { id } = useParams();
  const edicao = Boolean(id);
  const navegar = useNavigate();

  const [form, setForm] = useState(VAZIO);
  const [plataformas, setPlataformas] = useState([]);
  const [condicoes, setCondicoes] = useState([]);
  const [generos, setGeneros] = useState([]);
  const [carregando, setCarregando] = useState(true);
  const [erroCarga, setErroCarga] = useState(null);
  const [erro, setErro] = useState(null);
  const [errosCampos, setErrosCampos] = useState({});
  const [enviando, setEnviando] = useState(false);

  useEffect(() => {
    let ativo = true;
    setCarregando(true);
    setErroCarga(null);
    Promise.all([
      plataformaApi.listar(), tabelaTipoApi.listarCondicoesJogo(), generoApi.listar(),
      edicao ? jogoApi.consultar(id) : Promise.resolve(null),
    ])
      .then(([p, c, g, jogo]) => {
        if (!ativo) return;
        setPlataformas(p); setCondicoes(c); setGeneros(g);
        if (jogo) {
          setForm({
            nome: jogo.nome, descricao: jogo.descricao ?? '', anoLancamento: String(jogo.anoLancamento),
            preco: String(jogo.preco), estoque: String(jogo.estoque),
            idPlataforma: String(jogo.idPlataforma), idCondicaoJogo: String(jogo.idCondicaoJogo),
            idsGenero: jogo.generos.map((x) => x.id),
          });
        }
      })
      .catch((e) => ativo && setErroCarga(obterMensagemErro(e)))
      .finally(() => ativo && setCarregando(false));
    return () => { ativo = false; };
  }, [id, edicao]);

  const alterar = (campo) => (e) => setForm((f) => ({ ...f, [campo]: e.target.value }));

  function alternarGenero(idGenero) {
    setForm((f) => ({
      ...f,
      idsGenero: f.idsGenero.includes(idGenero) ? f.idsGenero.filter((x) => x !== idGenero) : [...f.idsGenero, idGenero],
    }));
  }

  async function aoEnviar(evento) {
    evento.preventDefault();
    setErro(null);
    setErrosCampos({});
    setEnviando(true);

    const corpo = {
      nome: form.nome,
      descricao: form.descricao || null,
      anoLancamento: Number(form.anoLancamento),
      preco: Number(String(form.preco).replace(',', '.')),
      estoque: Number(form.estoque),
      idPlataforma: Number(form.idPlataforma),
      idCondicaoJogo: Number(form.idCondicaoJogo),
      idsGenero: form.idsGenero,
    };

    try {
      if (edicao) await jogoApi.alterar(id, corpo); else await jogoApi.inserir(corpo);
      navegar('/admin/jogos');
    } catch (e) {
      setErrosCampos(obterErrosDosCampos(e));
      setErro(obterMensagemErro(e));
    } finally {
      setEnviando(false);
    }
  }

  const feedback = (campo) => errosCampos[campo] && <div className="invalid-feedback d-block">{errosCampos[campo].join(' ')}</div>;
  const classe = (campo) => `form-control ${errosCampos[campo] ? 'is-invalid' : ''}`;
  const classeSelect = (campo) => `form-select ${errosCampos[campo] ? 'is-invalid' : ''}`;

  if (carregando) return <Carregando />;
  if (erroCarga) return <MensagemErro mensagem={erroCarga} />;

  return (
    <>
      <h1 className="h3 mb-3">{edicao ? 'Editar jogo' : 'Novo jogo'}</h1>
      {erro && <div className="alert alert-danger" role="alert">{erro}</div>}

      <form onSubmit={aoEnviar} noValidate className="bg-white p-3 rounded shadow-sm">
        <div className="row g-3">
          <div className="col-md-8">
            <label className="form-label" htmlFor="nome">Nome</label>
            <input id="nome" className={classe('nome')} value={form.nome} onChange={alterar('nome')} />
            {feedback('nome')}
          </div>
          <div className="col-md-4">
            <label className="form-label" htmlFor="ano">Ano de lançamento</label>
            <input id="ano" type="number" className={classe('anoLancamento')} value={form.anoLancamento} onChange={alterar('anoLancamento')} />
            {feedback('anoLancamento')}
          </div>
          <div className="col-12">
            <label className="form-label" htmlFor="descricao">Descrição (opcional)</label>
            <textarea id="descricao" rows="3" className={classe('descricao')} value={form.descricao} onChange={alterar('descricao')} />
            {feedback('descricao')}
          </div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="plataforma">Plataforma</label>
            <select id="plataforma" className={classeSelect('idPlataforma')} value={form.idPlataforma} onChange={alterar('idPlataforma')}>
              <option value="">Selecione</option>
              {plataformas.map((p) => <option key={p.id} value={p.id}>{p.nome}</option>)}
            </select>
            {feedback('idPlataforma')}
          </div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="condicao">Condição</label>
            <select id="condicao" className={classeSelect('idCondicaoJogo')} value={form.idCondicaoJogo} onChange={alterar('idCondicaoJogo')}>
              <option value="">Selecione</option>
              {condicoes.map((c) => <option key={c.id} value={c.id}>{c.descricao}</option>)}
            </select>
            {feedback('idCondicaoJogo')}
          </div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="preco">Preço (R$)</label>
            <input id="preco" inputMode="decimal" className={classe('preco')} value={form.preco} onChange={alterar('preco')} />
            {feedback('preco')}
          </div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="estoque">Estoque</label>
            <input id="estoque" type="number" className={classe('estoque')} value={form.estoque} onChange={alterar('estoque')} />
            {feedback('estoque')}
          </div>
          <div className="col-12">
            <span className="form-label d-block">Gêneros</span>
            {generos.map((g) => (
              <div className="form-check form-check-inline" key={g.id}>
                <input className="form-check-input" type="checkbox" id={`genero-${g.id}`}
                       checked={form.idsGenero.includes(g.id)} onChange={() => alternarGenero(g.id)} />
                <label className="form-check-label" htmlFor={`genero-${g.id}`}>{g.nome}</label>
              </div>
            ))}
            {feedback('idsGenero')}
          </div>
        </div>

        <div className="mt-4 d-flex gap-2">
          <button className="btn btn-dark" type="submit" disabled={enviando}>{enviando ? 'Salvando...' : 'Salvar'}</button>
          <Link to="/admin/jogos" className="btn btn-outline-secondary">Cancelar</Link>
        </div>
      </form>
    </>
  );
}

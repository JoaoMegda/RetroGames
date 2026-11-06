import { useCallback, useEffect, useState } from 'react';
import { plataformaApi } from '../api/servicos.js';
import { obterErrosDosCampos, obterMensagemErro } from '../api/erros.js';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado.jsx';

const VAZIO = { nome: '', fabricante: '', anoLancamento: '' };

export default function Plataforma() {
  const [plataformas, setPlataformas] = useState([]);
  const [carregando, setCarregando] = useState(true);
  const [erroCarga, setErroCarga] = useState(null);

  const [form, setForm] = useState(VAZIO);
  const [idEdicao, setIdEdicao] = useState(null);
  const [erro, setErro] = useState(null);
  const [errosCampos, setErrosCampos] = useState({});
  const [enviando, setEnviando] = useState(false);

  const carregar = useCallback(() => {
    setCarregando(true);
    setErroCarga(null);
    plataformaApi.listar()
      .then(setPlataformas)
      .catch((e) => setErroCarga(obterMensagemErro(e)))
      .finally(() => setCarregando(false));
  }, []);

  useEffect(() => { carregar(); }, [carregar]);

  const alterar = (campo) => (e) => setForm((f) => ({ ...f, [campo]: e.target.value }));

  function limpar() {
    setForm(VAZIO);
    setIdEdicao(null);
    setErro(null);
    setErrosCampos({});
  }

  function editar(p) {
    setIdEdicao(p.id);
    setForm({ nome: p.nome, fabricante: p.fabricante, anoLancamento: String(p.anoLancamento) });
    setErro(null);
    setErrosCampos({});
  }

  async function aoEnviar(evento) {
    evento.preventDefault();
    setErro(null);
    setErrosCampos({});
    setEnviando(true);
    const corpo = { nome: form.nome, fabricante: form.fabricante, anoLancamento: Number(form.anoLancamento) };
    try {
      if (idEdicao) await plataformaApi.alterar(idEdicao, corpo); else await plataformaApi.inserir(corpo);
      limpar();
      carregar();
    } catch (e) {
      setErrosCampos(obterErrosDosCampos(e));
      setErro(obterMensagemErro(e));
    } finally {
      setEnviando(false);
    }
  }

  async function excluir(p) {
    if (!window.confirm(`Excluir a plataforma "${p.nome}"?`)) return;
    setErro(null);
    try {
      await plataformaApi.excluir(p.id);
      if (idEdicao === p.id) limpar();
      carregar();
    } catch (e) {
      setErro(obterMensagemErro(e));
    }
  }

  const classe = (campo) => `form-control ${errosCampos[campo] ? 'is-invalid' : ''}`;
  const feedback = (campo) => errosCampos[campo] && <div className="invalid-feedback">{errosCampos[campo].join(' ')}</div>;

  return (
    <>
      <h1 className="h3 mb-3">Plataformas</h1>
      {erro && <div className="alert alert-danger" role="alert">{erro}</div>}

      <form onSubmit={aoEnviar} noValidate className="bg-white p-3 rounded shadow-sm mb-4">
        <h2 className="h5">{idEdicao ? 'Alterar plataforma' : 'Nova plataforma'}</h2>
        <div className="row g-3">
          <div className="col-md-4">
            <label className="form-label" htmlFor="nome">Nome</label>
            <input id="nome" className={classe('nome')} value={form.nome} onChange={alterar('nome')} />
            {feedback('nome')}
          </div>
          <div className="col-md-4">
            <label className="form-label" htmlFor="fabricante">Fabricante</label>
            <input id="fabricante" className={classe('fabricante')} value={form.fabricante} onChange={alterar('fabricante')} />
            {feedback('fabricante')}
          </div>
          <div className="col-md-4">
            <label className="form-label" htmlFor="ano">Ano de lançamento</label>
            <input id="ano" type="number" className={classe('anoLancamento')} value={form.anoLancamento} onChange={alterar('anoLancamento')} />
            {feedback('anoLancamento')}
          </div>
        </div>
        <div className="mt-3 d-flex gap-2">
          <button className="btn btn-dark" type="submit" disabled={enviando}>{enviando ? 'Salvando...' : 'Salvar'}</button>
          {idEdicao && <button type="button" className="btn btn-outline-secondary" onClick={limpar}>Cancelar</button>}
        </div>
      </form>

      {carregando && <Carregando />}
      {!carregando && erroCarga && <MensagemErro mensagem={erroCarga} aoTentarNovamente={carregar} />}
      {!carregando && !erroCarga && (
        plataformas.length === 0 ? (
          <EstadoVazio titulo="Nenhuma plataforma cadastrada" />
        ) : (
          <div className="table-responsive">
            <table className="table table-hover align-middle bg-white">
              <thead>
                <tr><th>Nome</th><th>Fabricante</th><th>Ano</th><th className="text-end">Ações</th></tr>
              </thead>
              <tbody>
                {plataformas.map((p) => (
                  <tr key={p.id}>
                    <td>{p.nome}</td>
                    <td>{p.fabricante}</td>
                    <td>{p.anoLancamento}</td>
                    <td className="text-end text-nowrap">
                      <button type="button" className="btn btn-sm btn-outline-dark me-1" onClick={() => editar(p)}>Editar</button>
                      <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => excluir(p)}>Excluir</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )
      )}
    </>
  );
}

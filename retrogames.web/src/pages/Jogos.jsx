import { useCallback, useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { jogoApi } from '../api/servicos.js';
import { obterMensagemErro } from '../api/erros.js';
import { formatarPreco } from '../lib/formato.js';
import Paginacao from '../components/Paginacao.jsx';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado.jsx';

const TAMANHO_PAGINA = 10;

export default function Jogos() {
  const [params, setParams] = useSearchParams();
  const busca = params.get('busca') ?? '';
  const pagina = Number(params.get('pagina') ?? 1) || 1;

  const [textoBusca, setTextoBusca] = useState(busca);
  const [resultado, setResultado] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);
  const [erroAcao, setErroAcao] = useState(null);

  const carregar = useCallback(() => {
    setCarregando(true);
    setErro(null);
    jogoApi
      .listar({ busca: busca || undefined, pagina, tamanhoPagina: TAMANHO_PAGINA, incluirInativos: true })
      .then(setResultado)
      .catch((e) => setErro(obterMensagemErro(e)))
      .finally(() => setCarregando(false));
  }, [busca, pagina]);

  useEffect(() => { carregar(); }, [carregar]);

  function atualizar(mudancas) {
    const novo = new URLSearchParams(params);
    Object.entries(mudancas).forEach(([chave, valor]) => {
      if (valor) novo.set(chave, valor); else novo.delete(chave);
    });
    setParams(novo);
  }

  async function desativar(jogo) {
    if (!window.confirm(`Desativar "${jogo.nome}"? O jogo deixa de aparecer no catálogo público.`)) return;
    setErroAcao(null);
    try {
      await jogoApi.desativar(jogo.id);
      carregar();
    } catch (e) {
      setErroAcao(obterMensagemErro(e));
    }
  }

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="h3 mb-0">Jogos</h1>
        <Link to="/admin/jogos/novo" className="btn btn-dark">Novo jogo</Link>
      </div>

      <form className="d-flex gap-2 mb-3" onSubmit={(e) => { e.preventDefault(); atualizar({ busca: textoBusca.trim(), pagina: '' }); }}>
        <input className="form-control" type="search" placeholder="Buscar jogo" aria-label="Buscar jogo"
               value={textoBusca} onChange={(e) => setTextoBusca(e.target.value)} />
        <button className="btn btn-outline-dark" type="submit">Buscar</button>
      </form>

      {erroAcao && <div className="alert alert-danger" role="alert">{erroAcao}</div>}
      {carregando && <Carregando />}
      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />}
      {!carregando && !erro && resultado && (
        resultado.itens.length === 0 ? (
          <EstadoVazio titulo="Nenhum jogo encontrado" />
        ) : (
          <>
            <div className="table-responsive">
              <table className="table table-hover align-middle bg-white">
                <thead>
                  <tr>
                    <th>Nome</th><th>Plataforma</th><th>Condição</th>
                    <th className="text-end">Preço</th><th className="text-end">Estoque</th>
                    <th>Situação</th><th className="text-end">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {resultado.itens.map((jogo) => (
                    <tr key={jogo.id}>
                      <td>{jogo.nome}</td>
                      <td>{jogo.nomePlataforma}</td>
                      <td>{jogo.descricaoCondicao}</td>
                      <td className="text-end">{formatarPreco(jogo.preco)}</td>
                      <td className="text-end">{jogo.estoque}</td>
                      <td>{jogo.ativo
                        ? <span className="badge text-bg-success">Ativo</span>
                        : <span className="badge text-bg-secondary">Inativo</span>}</td>
                      <td className="text-end text-nowrap">
                        <Link to={`/admin/jogos/${jogo.id}/editar`} className="btn btn-sm btn-outline-dark me-1">Editar</Link>
                        <button type="button" className="btn btn-sm btn-outline-danger" disabled={!jogo.ativo}
                                onClick={() => desativar(jogo)}>Desativar</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas}
                       aoMudar={(p) => atualizar({ pagina: p === 1 ? '' : String(p) })} />
          </>
        )
      )}
    </>
  );
}

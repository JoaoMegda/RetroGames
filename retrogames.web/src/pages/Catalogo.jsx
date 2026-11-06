import { useCallback, useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { generoApi, jogoApi, plataformaApi } from '../api/servicos.js';
import { obterMensagemErro } from '../api/erros.js';
import CardJogo from '../components/CardJogo.jsx';
import Paginacao from '../components/Paginacao.jsx';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado.jsx';

const TAMANHO_PAGINA = 12;

export default function Catalogo() {
  const [params, setParams] = useSearchParams();
  const busca = params.get('busca') ?? '';
  const idPlataforma = params.get('idPlataforma') ?? '';
  const idGenero = params.get('idGenero') ?? '';
  const pagina = Number(params.get('pagina') ?? 1) || 1;

  const [textoBusca, setTextoBusca] = useState(busca);
  const [plataformas, setPlataformas] = useState([]);
  const [generos, setGeneros] = useState([]);
  const [resultado, setResultado] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);

  useEffect(() => {
    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([p, g]) => { setPlataformas(p); setGeneros(g); })
      .catch(() => { /* os filtros ficam vazios; a lista de jogos mostra o erro, se houver */ });
  }, []);

  const carregar = useCallback(() => {
    setCarregando(true);
    setErro(null);
    jogoApi
      .listar({
        busca: busca || undefined,
        idPlataforma: idPlataforma || undefined,
        idGenero: idGenero || undefined,
        pagina,
        tamanhoPagina: TAMANHO_PAGINA,
      })
      .then(setResultado)
      .catch((e) => setErro(obterMensagemErro(e)))
      .finally(() => setCarregando(false));
  }, [busca, idPlataforma, idGenero, pagina]);

  useEffect(() => { carregar(); }, [carregar]);

  // Filtros e página ficam na query string.
  function atualizar(mudancas) {
    const novo = new URLSearchParams(params);
    Object.entries(mudancas).forEach(([chave, valor]) => {
      if (valor) novo.set(chave, valor); else novo.delete(chave);
    });
    setParams(novo);
  }

  function aoBuscar(evento) {
    evento.preventDefault();
    atualizar({ busca: textoBusca.trim(), pagina: '' });
  }

  return (
    <>
      <h1 className="h3 mb-3">Catálogo</h1>

      <form className="row g-2 mb-4" onSubmit={aoBuscar}>
        <div className="col-md-5">
          <input className="form-control" type="search" placeholder="Buscar jogo" aria-label="Buscar jogo"
                 value={textoBusca} onChange={(e) => setTextoBusca(e.target.value)} />
        </div>
        <div className="col-md-3">
          <select className="form-select" aria-label="Plataforma" value={idPlataforma}
                  onChange={(e) => atualizar({ idPlataforma: e.target.value, pagina: '' })}>
            <option value="">Todas as plataformas</option>
            {plataformas.map((p) => <option key={p.id} value={p.id}>{p.nome}</option>)}
          </select>
        </div>
        <div className="col-md-3">
          <select className="form-select" aria-label="Gênero" value={idGenero}
                  onChange={(e) => atualizar({ idGenero: e.target.value, pagina: '' })}>
            <option value="">Todos os gêneros</option>
            {generos.map((g) => <option key={g.id} value={g.id}>{g.nome}</option>)}
          </select>
        </div>
        <div className="col-md-1 d-grid">
          <button className="btn btn-dark" type="submit">Buscar</button>
        </div>
      </form>

      {carregando && <Carregando />}
      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />}
      {!carregando && !erro && resultado && (
        resultado.itens.length === 0 ? (
          <EstadoVazio titulo="Nenhum jogo encontrado">Ajuste a busca ou os filtros.</EstadoVazio>
        ) : (
          <>
            <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3">
              {resultado.itens.map((jogo) => (
                <div className="col" key={jogo.id}><CardJogo jogo={jogo} /></div>
              ))}
            </div>
            <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas}
                       aoMudar={(p) => atualizar({ pagina: p === 1 ? '' : String(p) })} />
          </>
        )
      )}
    </>
  );
}

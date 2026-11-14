# Aula 6 — Autenticação e administração — Plano de implementação

> Plano da execução inline autorizada pelo estudante; cada entrega será verificada antes do commit.

**Objetivo:** completar a autenticação por cookie, a proteção de mutações da API e a autorização administrativa, preservando os envelopes e as camadas do RetroGames.

**Arquitetura:** o `AutenticacaoService` valida usuários e hashes; a API cria claims e mantém sessão em cookie. Um serviço de usuário atual em `RetroGames.Services` informa ao `JogoService` se a pessoa autenticada pode consultar jogos inativos. Controllers protegem mutações com a policy `SomenteAdministrador`, e o antiforgery exige o cabeçalho `X-CSRF-TOKEN`.

**Stack:** .NET 10, ASP.NET Core controllers, cookie authentication, antiforgery, ADO.NET/MySqlConnector, FluentValidation, xUnit e Moq. Sem novas dependências.

**Especificação:** slides enviados de “RetroGames - Aula 6: Autenticação e administração.pdf”; regras globais em `AGENTS.md`.

## Restrições globais

- Manter o envelope `{ responseCode, data, errorMessage }` para respostas da API; falha CSRF permanece HTTP 400 sem envelope, como especificado.
- Cookie de sessão HttpOnly, SameSite Strict, expiração deslizante de duas horas e Secure em produção.
- Nunca incluir senha/hash nas claims, logs, respostas ou arquivos versionados.
- Somente Administrador pode gravar plataformas e jogos; leituras do catálogo seguem públicas, exceto `/api/Autenticacao/eu`, que exige sessão.
- O Service conhece o usuário atual por interface, sem receber `HttpContext`, HTTP ou SQL.
- `.env` continua local e ignorado pelo Git; `.env.example` só mantém placeholders.

## Foco de revisão

- Credencial errada ou usuário inativo → HTTP 401 e `responseCode` 6.
- Sem sessão em mutação → HTTP 401 e código 4; sessão sem perfil exigido → HTTP 403 e código 5.
- Token CSRF ausente/inválido em POST/PUT/DELETE → HTTP 400 sem envelope; GET não exige token.
- Usuário anônimo não inclui jogos inativos mesmo com `incluirInativos=true`; Administrador pode incluí-los.
- Cookie e claims nunca expõem senha/hash; Secure é ativado fora do ambiente de desenvolvimento.

---

### Tarefa 1: identidade atual e regra de jogos inativos

**Arquivos:** criar `RetroGames.Services/IUsuarioAtualService.cs` e a implementação na API; modificar `JogoService.cs`, `IJogoService.cs`, `JogoController.cs`, `JogoApiModels.cs`, `JogoMapper.cs` e configuração de DI.

- [x] Implementar interface de contexto de usuário desacoplada de ASP.NET e ler claims na API via `IHttpContextAccessor`.
- [x] Remover parâmetros `ehAdministrador` passados pelo Controller; adicionar `incluirInativos` à query e deixar o Service aplicar RN04.
- [x] Confirmar compilação da solução (build concluído sem erros).

### Tarefa 2: sessão, autenticação e autorização

**Arquivos:** modificar `AutenticacaoController.cs`, `Program.cs`, `PlataformaController.cs` e `JogoController.cs`.

- [x] Permitir `/eu` e logout para qualquer usuário autenticado; manter login e obtenção de CSRF públicos.
- [x] Aplicar `SomenteAdministrador` a POST/PUT/DELETE de plataformas e jogos, sem restringir GETs do catálogo.
- [x] Configurar cookie de sessão com HttpOnly, SameSite Strict, duas horas deslizantes e Secure em produção; manter claims mínimas sem senha.
- [x] Confirmar compilação e revisar os handlers 401/403 e a ordem dos middlewares no código.

### Tarefa 3: CSRF e verificação final

**Arquivos:** ajustar `Program.cs`/`AutenticacaoController.cs` se a inspeção mostrar lacunas; revisar README e configuração de desenvolvimento apenas se necessário.

- [x] Revisar que `GET /api/Autenticacao/csrf` emite cookie legível `XSRF-TOKEN` e cookie antiforgery HttpOnly.
- [x] Revisar a validação global das mutações pelo header `X-CSRF-TOKEN`; a resposta de falha CSRF será confirmada no Swagger pelo estudante.
- [x] Executar `dotnet build` da solução (nenhuma alteração foi feita no front-end).
- [ ] Fazer os testes funcionais no Swagger quando o roteiro completo estiver disponível.
- [ ] Revisar `git diff`, garantir que `.env` não está no diff e criar commit da Aula 6; enviar ao GitHub se o remoto estiver disponível.

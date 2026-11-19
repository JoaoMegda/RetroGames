# RetroGames — back-end (base)

Banco MySQL, API em camadas, testes e front-end React (`retrogames.web`).

## Montar a solução (na raiz do repositório)

```
dotnet new sln -n RetroGames --format sln
dotnet sln add RetroGames.Model RetroGames.Repository RetroGames.Services RetroGames.WebApi.ApiModels RetroGames.WebApi RetroGames.WebApi.UnitTests
dotnet restore
dotnet build
```

(No .NET 10, `dotnet new sln` cria `.slnx` por padrão; `--format sln` gera o `RetroGames.sln` da especificação.)
Se algum pacote não restaurar na versão indicada nos `.csproj`, ajuste para a versão disponível.

## Configurar e executar

1. Execute `scripts/001_criar_estrutura.sql` no MySQL 8.
2. Copie `.env.example` para `.env` (raiz) e troque `SUA_SENHA` pelas suas senhas. O `.env` está no `.gitignore`.
3. `dotnet run --project RetroGames.WebApi` → API em http://localhost:5080, Swagger em /swagger.
4. `dotnet test` na raiz.

## Conferência da Aula 6

Abra o Swagger em `http://localhost:5080/swagger` em uma janela anônima. Use uma
sessão sem login para as linhas que pedem acesso anônimo e um banco de
desenvolvimento para a gravação de teste.

| Requisição | HTTP | `responseCode` |
| --- | ---: | --- |
| `POST /api/Plataforma` sem login | 401 | 4 |
| `POST /api/Autenticacao/login` com senha errada | 401 | 6 |
| `POST /api/Autenticacao/login` com e-mail malformado | 400 | 3, com erros em `data` |
| `POST /api/Autenticacao/login` com as credenciais do administrador | 200 | 0 |
| `POST /api/Plataforma` após login como administrador | 201 | 0 |
| `GET /api/Jogo?incluirInativos=true` sem login | 200 | 0, apenas jogos ativos |
| `POST /api/Autenticacao/logout` e depois `GET /api/Autenticacao/eu` | 200 e 401 | 0 e 4 |

Confira o cookie `retrogames.sessao` com `HttpOnly` e `SameSite=Strict` após
entrar. Com um jogo inativo no banco, compare a listagem anônima com a do
administrador usando `incluirInativos=true`. O detalhe do inativo responde 404 ao
visitante e 200 ao administrador. Uma conta de teste com perfil Cliente recebe
403, código 5, ao tentar gravar no catálogo.

## Front-end (`retrogames.web`)

```
cd retrogames.web
npm install
npm run dev      # http://localhost:5173 (o Vite encaminha /api para a API em :5080)
```

Suba a API antes (veja acima). Ela usa `AdminInicial__Email` e `AdminInicial__Senha` do `.env` para criar a primeira conta administrativa. Se o banco já tem um administrador, use as credenciais dessa conta.

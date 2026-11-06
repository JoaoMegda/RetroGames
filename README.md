# RetroGames — back-end (base)

Parte 1: banco, API em camadas e um teste de exemplo. O front-end (`retrogames.web`) ainda não foi gerado.

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

## Como verificar

- `GET /api/Plataforma` → `{ "responseCode": 0, "data": [], "errorMessage": null }`
- `POST /api/Plataforma` sem login → 401 (responseCode 4)
- `GET /api/Autenticacao/csrf` → cookie `XSRF-TOKEN`; depois `POST /api/Autenticacao/login` com o header `X-CSRF-TOKEN`.

## Front-end (`retrogames.web`)

```
cd retrogames.web
npm install
npm run dev      # http://localhost:5173 (o Vite encaminha /api para a API em :5080)
```

Suba a API antes (veja acima). Login do administrador: o e-mail e a senha definidos em `AdminInicial__*` no `.env`.

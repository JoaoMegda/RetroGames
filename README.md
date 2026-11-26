# RetroGames

Loja de jogos retrô com catálogo público, administração e a versão Básica do Módulo de Compras. O back-end usa ASP.NET Core e MySQL 8. A interface usa React, Vite, Bootstrap e Axios.

## Configurar o banco

1. Em um banco novo, execute [scripts/001_criar_estrutura.sql](scripts/001_criar_estrutura.sql) para criar catálogo e usuários. Se o banco de catálogo já existe e funciona, siga para o próximo passo.
2. Execute [scripts/002_criar_tabelas_compras.sql](scripts/002_criar_tabelas_compras.sql) uma vez, com o banco RetroGames selecionado. Esse script acrescenta situações, pedidos e itens de pedido.
3. Copie o arquivo .env.example para .env na raiz e preencha os valores do seu ambiente. O .env é ignorado pelo Git. Se o banco já tem uma conta administrativa, use essa conta.

## Executar

Na raiz:

~~~powershell
dotnet run --project RetroGames.WebApi
~~~

Em outro terminal:

~~~powershell
cd retrogames.web
npm ci
npm run dev
~~~

Abra [a loja](http://localhost:5173). A [documentação da API](http://localhost:5080/swagger) está disponível com a configuração de desenvolvimento. O Vite encaminha /api para a porta 5080 e mantém o front-end e a API na mesma origem para os cookies.

## Verificar a entrega

~~~powershell
dotnet test RetroGames.sln
cd retrogames.web
npm run build
~~~

Em um banco de desenvolvimento, use um jogo público com estoque e faça o fluxo de cadastro, carrinho, finalização e Meus pedidos. Confira a baixa de estoque. Repita com quantidade maior do que o estoque disponível e verifique que a tentativa não deixou pedido, itens ou baixa parcial.

Para testar gravações diretamente na API, obtenha o token por GET /api/Autenticacao/csrf na mesma sessão e envie-o como X-CSRF-TOKEN em POST, PUT e DELETE. Conserve os cookies nas chamadas e obtenha outro token depois de entrar, cadastrar-se ou sair. A interface faz isso automaticamente.

Veja a [especificação do módulo e o guia de apresentação](docs/MODULO_COMPRAS.md) e o [quadro público da sprint](https://github.com/users/JoaoMegda/projects/2).

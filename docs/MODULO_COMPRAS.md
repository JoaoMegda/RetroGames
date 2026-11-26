# Módulo de Compras · versão Básica

O cliente cria uma conta, monta o carrinho, finaliza uma compra e acompanha os pedidos. A implementação cobre C01 a C05.

## 3 · Arquitetura

O Controller recebe o ApiModel, chama o Service e devolve o status HTTP e o envelope. O FluentValidation verifica formato, limites e campos obrigatórios antes da operação. O Service aplica as regras de negócio, usa a identidade autenticada, combina dados de várias tabelas com LINQ e abre a transação de compra. Cada Repository acessa uma tabela com SQL parametrizado e converte as colunas em propriedades do Model.

O Axios mantém as requisições na mesma origem. O React reutiliza a sessão existente para identificar o perfil, montar o menu e redirecionar o visitante para o Login com retorno à rota original. O carrinho é estado da interface e persiste no navegador. A autenticação é mantida em cookie de sessão HttpOnly e SameSite=Strict.

## 4 · Banco de dados

O script incremental está em [scripts/002_criar_tabelas_compras.sql](../scripts/002_criar_tabelas_compras.sql). Ele reutiliza tb_usuario, tb_perfil e tb_jogo do catálogo e acrescenta as tabelas abaixo.

| Tabela | Colunas e relacionamentos |
| --- | --- |
| tb_situacao_pedido | co_situacao_pedido é a PK. ds_situacao_pedido descreve a situação. Dados fixos 1 Pendente, 2 Pago, 3 Enviado e 4 Cancelado. |
| tb_pedido | co_seq_pedido é a PK. dt_pedido guarda a data em UTC e vl_total guarda o total. co_usuario e co_situacao_pedido são FKs. |
| tb_item_pedido | co_seq_item_pedido é a PK. qt_item guarda a quantidade e vl_unitario o preço na compra. co_pedido e co_jogo são FKs. UNIQUE (co_pedido, co_jogo) impede repetir um jogo no mesmo pedido. |

Cada alteração na estrutura fica em um novo script. As FKs mantêm o comportamento padrão. As consultas usadas na aplicação ficam nos Repositories versionados.

## 5 · Contrato HTTP

Respostas da API usam o envelope com responseCode, data e errorMessage.

| Método e caminho | Entrada e retorno | Acesso |
| --- | --- | --- |
| POST /api/Autenticacao/cadastrar | Nome, e-mail e senha. HTTP 201 e código 0 com os dados da sessão iniciada. | Público |
| POST /api/Pedido | Lista itens com idJogo e quantidade. HTTP 201 e código 0 com o pedido registrado. | Sessão autenticada |
| GET /api/Pedido/meus | HTTP 200 e código 0 com os pedidos do usuário autenticado. | Sessão autenticada |

O pedido retorna id, data em UTC, situacao, total e itens. Cada item contém idJogo, nomeJogo, quantidade e valorUnitario. A resposta de sucesso ao usuário sem pedidos inclui uma lista vazia.

A rota de infraestrutura GET /api/Autenticacao/csrf pertence ao fluxo de autenticação da especificação. Ela fornece o token antes de requisições de escrita. POST, PUT e DELETE enviam o cabeçalho X-CSRF-TOKEN. A proteção é validada pela API junto das permissões já existentes.

| HTTP | Código | Condição |
| --- | --- | --- |
| 400 | 2 | Corpo obrigatório ausente ou nulo, conforme o envelope geral. |
| 400 | 3 | Campos inválidos, pedido sem itens ou quantidade fora de 1 a 10. |
| 400 | 110 | Um ou mais jogos inexistentes ou inativos. |
| 401 | 4 | Acesso protegido sem sessão. |
| 409 | 106 | E-mail já cadastrado. |
| 409 | 111 | Estoque insuficiente. |

## 7 · Regras de negócio

- **C01** · Nome e e-mail são aparados. O e-mail precisa ser válido e comportar até 150 caracteres. O nome pode ter até 100 caracteres e a senha deve ter de 8 a 128 caracteres. O usuário recebe perfil Cliente (2), começa ativo e tem apenas o hash da senha armazenado. A restrição única de e-mail também protege contra cadastros concorrentes.
- **C02** · O menu do cliente mostra Carrinho e Meus pedidos. A área administrativa continua reservada ao administrador. Carrinho e Meus pedidos redirecionam o visitante ao Login com retorno ao destino após autenticação.
- **C03** · O detalhe permite adicionar jogo disponível. O carrinho limita a quantidade de cada jogo de 1 a 10, permite remoção e exibe o total da seleção. O navegador armazena apenas os dados do carrinho no localStorage.
- **C04** · A compra usa apenas jogos ativos e o preço atual do banco. O pedido começa Pendente com data UTC. A transação ReadCommitted mantém pedido, itens e baixa de estoque juntos. O estoque disponível é protegido também na gravação, para que compras concorrentes não vendam a mesma unidade. A compra aceita cada jogo uma vez. Alterar outros dados no formulário de administração preserva o estoque atual quando a quantidade não foi editada.
- **C05** · A consulta usa o identificador da sessão, ordena do pedido mais recente para o mais antigo e exibe situação, total e itens com preço histórico.

### 7.1 · Rastreabilidade

| História | Endpoint ou tela | Onde a regra é aplicada | Verificação |
| --- | --- | --- | --- |
| [C01](https://github.com/JoaoMegda/RetroGames/issues/1) | Cadastro e POST /api/Autenticacao/cadastrar | AutenticacaoController, Validator, AutenticacaoService e UsuarioRepository | Testes de autenticação e validação, cadastro com sessão e e-mail repetido na API |
| [C02](https://github.com/JoaoMegda/RetroGames/issues/2) | Menu e rotas protegidas | Layout, contexto de autenticação e RotaProtegida | Visitante, cliente e administrador, com retorno após Login |
| [C03](https://github.com/JoaoMegda/RetroGames/issues/3) | Detalhe do jogo e /carrinho | DetalheJogo e estado do carrinho no React | Limite e persistência, bloqueio de esgotado e atualização do total |
| [C04](https://github.com/JoaoMegda/RetroGames/issues/4) | POST /api/Pedido e finalização do carrinho | PedidoController, Validator, PedidoService e Repositories | PedidoServiceTests com Moq, compra real, disputa pelo estoque e tentativa rejeitada |
| [C05](https://github.com/JoaoMegda/RetroGames/issues/5) | /meus-pedidos e GET /api/Pedido/meus | PedidoService, Repositories e tela MeusPedidos | PedidoServiceTests, pedidos da sessão e preço histórico |

## Guia de apresentação e prova

1. Abra o quadro e explique como uma história virou tarefas. Mostre um commit com o código da história e o teste correspondente.
2. Com um jogo em estoque, mostre cadastro, carrinho, compra e Meus pedidos. Refaça a demonstração no modo responsivo a 360 px e 1366 px.
3. Mostre exemplos de cinco heurísticas de Nielsen. O botão que informa a finalização é **visibilidade do estado**. Valores em reais são **correspondência com o mundo real**. Remover um jogo é **controle do usuário**. O limite de quantidade e o bloqueio de esgotado aplicam **prevenção de erros**. As mensagens junto aos campos ajudam a **reconhecer e corrigir erros**.
4. Para explicar uma regra, use a compra sem estoque. O Validator avalia a quantidade digitada, enquanto o Service consulta o estoque real e decide se a compra pode acontecer. O Repository só executa a consulta e a atualização parametrizadas. Se uma gravação falhar, a transação é descartada e a aplicação não deixa a compra pela metade. No teste unitário, o Moq simula as respostas do Repository para verificar a decisão do Service sem MySQL.
5. Explique por que o estoque e o preço precisam ser conferidos no servidor. A tela pode estar desatualizada ou ser manipulada. O histórico registra o preço pago, portanto uma alteração no catálogo não muda um pedido anterior.

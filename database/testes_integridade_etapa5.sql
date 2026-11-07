-- RetroGames: testes manuais de integridade da etapa 5 (MySQL 8).
-- Execute cada INSERT/DELETE de teste separadamente (Ctrl+Enter no Workbench).
-- São esperados erros nos quatro primeiros casos; não execute o arquivo inteiro de uma vez.
-- Execute a limpeza final depois dos cinco casos.

USE retrogames;

-- Teste 1: jogo com plataforma inexistente.
-- Resultado esperado: ERRO 1452 (violação da chave estrangeira).
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('TESTE_ETAPA5_PLATAFORMA_INEXISTENTE', 'Registro descartado pelo FK.',
     2026, 10.00, 1, TRUE, '2026-11-06 10:00:00', 999999, 1);

-- Teste 2: plataforma com nome já cadastrado.
-- Resultado esperado: ERRO 1062 (violação de UNIQUE em no_plataforma).
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Atari 2600', 'Atari', 1977);

-- Teste 3: excluir plataforma referenciada por jogos.
-- Resultado esperado: ERRO 1451 (violação da chave estrangeira).
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = 1;

-- Teste 4: inserir jogo com nome nulo.
-- Resultado esperado: ERRO 1048 (no_jogo não aceita NULL).
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (NULL, 'Registro descartado por NOT NULL.',
     2026, 10.00, 1, TRUE, '2026-11-06 10:00:00', 1, 1);

-- Teste 5: preço zero aceito diretamente pelo banco; RN06 pertence ao Service.
-- Resultado esperado: inserção aceita (1 row affected); sem CHECK no banco.
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('TESTE_ETAPA5_PRECO_ZERO', 'Registro temporário do teste RN06.',
     2026, 0.00, 1, TRUE, '2026-11-06 10:00:00', 1, 1);

-- Limpeza final: remova somente o jogo que o teste de preço zero inseriu.
DELETE FROM tb_jogo
WHERE no_jogo = 'TESTE_ETAPA5_PRECO_ZERO';

-- Resultado esperado após a limpeza: 0.
SELECT COUNT(*) AS jogos_de_teste_restantes
FROM tb_jogo
WHERE no_jogo = 'TESTE_ETAPA5_PRECO_ZERO';

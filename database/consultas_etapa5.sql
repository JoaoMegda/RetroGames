-- RetroGames: consultas da etapa 5 (MySQL 8).
-- Execute em retrogames depois das etapas 3 e 4.

USE retrogames;

-- Plataforma temporária para confirmar que a contagem inclui plataformas sem jogos.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('ZZ - Plataforma sem jogos (teste etapa 5)', 'Teste', 2026);

-- 1. Catálogo: jogos ativos com plataforma e descrição da condição, ordenados pelo nome.
SELECT j.no_jogo AS nome_jogo,
       p.no_plataforma,
       c.ds_condicao_jogo AS descricao_condicao
FROM tb_jogo AS j
INNER JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo AS c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. Gêneros do jogo informado (Super Mario World, id 9).
SET @id_jogo_informado = 9;
SELECT j.no_jogo, g.no_genero
FROM tb_jogo AS j
INNER JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
INNER JOIN tb_genero AS g ON g.co_seq_genero = jg.co_genero
WHERE j.co_seq_jogo = @id_jogo_informado
ORDER BY g.no_genero;

-- 3. Jogos de um gênero informado (RPG, id 4).
SET @id_genero_informado = 4;
SELECT g.no_genero, j.no_jogo
FROM tb_genero AS g
INNER JOIN tb_jogo_genero AS jg ON jg.co_genero = g.co_seq_genero
INNER JOIN tb_jogo AS j ON j.co_seq_jogo = jg.co_jogo
WHERE g.co_seq_genero = @id_genero_informado
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma. A plataforma temporária deve aparecer com 0.
SELECT p.no_plataforma,
       COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca pela parte 'e' do nome, página 1 (5 itens por página).
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE
  AND no_jogo LIKE '%e%'
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

-- Busca da mesma parte, página 2 (deve trazer outros resultados).
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE
  AND no_jogo LIKE '%e%'
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;

-- Limpeza da plataforma temporária criada no início deste arquivo.
DELETE FROM tb_plataforma
WHERE no_plataforma = 'ZZ - Plataforma sem jogos (teste etapa 5)';

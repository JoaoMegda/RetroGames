-- RetroGames: dados de exemplo da etapa 4 (MySQL 8).
-- Execute uma única vez, depois de database/estrutura.sql, em um schema vazio.
-- Não insere usuários: o administrador será criado pela API.

USE retrogames;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    (1, 'Atari 2600', 'Atari', 1977),
    (2, 'Nintendo Entertainment System', 'Nintendo', 1983),
    (3, 'Super Nintendo Entertainment System', 'Nintendo', 1990),
    (4, 'Mega Drive', 'Sega', 1988),
    (5, 'Game Boy', 'Nintendo', 1989),
    (6, 'PlayStation', 'Sony', 1994);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
    (1, 'Ação'),
    (2, 'Aventura'),
    (3, 'Plataforma'),
    (4, 'RPG'),
    (5, 'Esporte'),
    (6, 'Corrida'),
    (7, 'Luta'),
    (8, 'Quebra-cabeça'),
    (9, 'Tiro'),
    (10, 'Estratégia'),
    (11, 'Simulação');

INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES
    (1, 'Pitfall!', 'Aventura de plataforma clássica da Atari.', 1982, 79.90, 4, TRUE, '2025-01-10 10:00:00', 1, 2),
    (2, 'Pac-Man', 'Conversão para Atari 2600 do clássico de labirinto.', 1982, 59.90, 7, TRUE, '2025-01-10 10:00:00', 1, 2),
    (3, 'River Raid', 'Jogo de ação com combate aéreo sobre um rio.', 1982, 69.90, 3, TRUE, '2025-01-10 10:00:00', 1, 2),
    (4, 'Space Invaders', 'Clássico de tiro com invasores espaciais.', 1980, 54.90, 5, TRUE, '2025-01-10 10:00:00', 1, 3),
    (5, 'Super Mario Bros.', 'Aventura de plataforma do encanador Mario.', 1985, 99.90, 6, TRUE, '2025-01-10 10:00:00', 2, 2),
    (6, 'The Legend of Zelda', 'Aventura de exploração no reino de Hyrule.', 1986, 149.90, 2, TRUE, '2025-01-10 10:00:00', 2, 2),
    (7, 'Metroid', 'Ação e exploração em um planeta alienígena.', 1986, 129.90, 3, TRUE, '2025-01-10 10:00:00', 2, 3),
    (8, 'Mega Man 2', 'Ação e plataforma contra os robôs do Dr. Wily.', 1988, 119.90, 4, TRUE, '2025-01-10 10:00:00', 2, 2),
    (9, 'Super Mario World', 'Aventura de plataforma lançada com o Super Nintendo.', 1990, 159.90, 5, TRUE, '2025-01-10 10:00:00', 3, 2),
    (10, 'The Legend of Zelda: A Link to the Past', 'Aventura de ação em Hyrule.', 1991, 189.90, 2, TRUE, '2025-01-10 10:00:00', 3, 2),
    (11, 'Donkey Kong Country', 'Aventura de plataforma com Donkey Kong e Diddy Kong.', 1994, 179.90, 3, TRUE, '2025-01-10 10:00:00', 3, 2),
    (12, 'Chrono Trigger', 'RPG com viagens no tempo.', 1995, 349.90, 0, TRUE, '2025-01-10 10:00:00', 3, 3),
    (13, 'Super Metroid', 'Aventura de ação e exploração espacial.', 1994, 219.90, 2, TRUE, '2025-01-10 10:00:00', 3, 2),
    (14, 'Sonic the Hedgehog', 'Aventura de plataforma do mascote da Sega.', 1991, 119.90, 6, TRUE, '2025-01-10 10:00:00', 4, 2),
    (15, 'Streets of Rage 2', 'Jogo de ação e luta cooperativo.', 1992, 169.90, 3, TRUE, '2025-01-10 10:00:00', 4, 2),
    (16, 'Golden Axe', 'Aventura de ação e luta em fantasia medieval.', 1989, 139.90, 2, TRUE, '2025-01-10 10:00:00', 4, 3),
    (17, 'Phantasy Star IV', 'RPG da série Phantasy Star.', 1993, 299.90, 1, TRUE, '2025-01-10 10:00:00', 4, 2),
    (18, 'Tetris', 'Jogo de quebra-cabeça com peças que se encaixam.', 1989, 89.90, 8, TRUE, '2025-01-10 10:00:00', 5, 2),
    (19, 'Pokémon Red', 'RPG de captura e treinamento de Pokémon.', 1996, 249.90, 2, TRUE, '2025-01-10 10:00:00', 5, 2),
    (20, 'The Legend of Zelda: Link’s Awakening', 'Aventura de exploração para Game Boy.', 1993, 229.90, 1, TRUE, '2025-01-10 10:00:00', 5, 3),
    (21, 'Kirby’s Dream Land', 'Aventura de plataforma estrelada por Kirby.', 1992, 119.90, 3, TRUE, '2025-01-10 10:00:00', 5, 2),
    (22, 'Castlevania: Symphony of the Night', 'Aventura de ação e plataforma em um castelo.', 1997, 279.90, 2, TRUE, '2025-01-10 10:00:00', 6, 2),
    (23, 'Final Fantasy VII', 'RPG com Cloud e seus companheiros.', 1997, 249.90, 4, TRUE, '2025-01-10 10:00:00', 6, 2),
    (24, 'Resident Evil 2', 'Aventura de sobrevivência contra zumbis.', 1998, 199.90, 2, TRUE, '2025-01-10 10:00:00', 6, 2),
    (25, 'Crash Bandicoot', 'Aventura de plataforma com Crash.', 1996, 179.90, 3, TRUE, '2025-01-10 10:00:00', 6, 2),
    (26, 'Metal Gear Solid', 'Jogo de ação furtiva com Solid Snake.', 1998, 229.90, 1, FALSE, '2025-01-10 10:00:00', 6, 3);

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (1, 2), (1, 3),
    (2, 8),
    (3, 1), (3, 9),
    (4, 9),
    (5, 3),
    (6, 2), (6, 4),
    (7, 1), (7, 2),
    (8, 1), (8, 3),
    (9, 3),
    (10, 2), (10, 4),
    (11, 3),
    (12, 4),
    (13, 1), (13, 2),
    (14, 3),
    (15, 1), (15, 7),
    (16, 1), (16, 7),
    (17, 4),
    (18, 8),
    (19, 2), (19, 4),
    (20, 2), (20, 4),
    (21, 3),
    (22, 1), (22, 2), (22, 3),
    (23, 4),
    (24, 1), (24, 2), (24, 9),
    (25, 3),
    (26, 1), (26, 2), (26, 10);

-- Consultas para conferir os requisitos da etapa.
SELECT 'plataformas' AS tabela, COUNT(*) AS quantidade FROM tb_plataforma
UNION ALL
SELECT 'generos', COUNT(*) FROM tb_genero
UNION ALL
SELECT 'jogos', COUNT(*) FROM tb_jogo;

-- Deve retornar zero linhas: nenhum jogo pode ficar sem gênero.
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo AS j
LEFT JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_jogo IS NULL;

-- Deve retornar o Metal Gear Solid (inativo) e o Chrono Trigger (estoque zero).
SELECT co_seq_jogo, no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0;

-- Deve retornar zero: usuários são criados pela API na etapa de autenticação.
SELECT COUNT(*) AS quantidade_usuarios FROM tb_usuario;

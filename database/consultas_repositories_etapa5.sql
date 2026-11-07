-- Consultas de leitura dos repositórios de domínio (etapa 5).
USE retrogames;

SELECT co_seq_genero, no_genero
FROM tb_genero
ORDER BY no_genero;

SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo
ORDER BY co_condicao_jogo;

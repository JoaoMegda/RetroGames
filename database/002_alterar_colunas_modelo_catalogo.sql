-- Alinha o schema já criado ao modelo relacional de Catálogo e Administração.
-- Execute uma vez após database/estrutura.sql no schema retrogames.
-- As tabelas de Compras ficam para a etapa específica dessa funcionalidade.
USE retrogames;

ALTER TABLE tb_plataforma
    MODIFY COLUMN no_plataforma VARCHAR(80) NOT NULL,
    MODIFY COLUMN no_fabricante VARCHAR(80) NOT NULL;

ALTER TABLE tb_jogo
    MODIFY COLUMN ds_jogo VARCHAR(1000) NULL;

ALTER TABLE tb_usuario
    MODIFY COLUMN ds_email VARCHAR(256) NOT NULL,
    MODIFY COLUMN ds_senha_hash VARCHAR(500) NOT NULL;

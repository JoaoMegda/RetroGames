-- 001_criar_estrutura.sql
-- Estrutura do Módulo de Catálogo e Administração.
-- Regra: só estrutura (PK, FK padrão, tipos, NOT NULL, UNIQUE). Sem CHECK, trigger, function ou ON DELETE.
-- Um script já executado não deve ser alterado: mudanças entram em um novo script (002_...).

CREATE DATABASE IF NOT EXISTS retrogames CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE retrogames;

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo  INT          NOT NULL,
    ds_condicao_jogo  VARCHAR(50)  NOT NULL,
    PRIMARY KEY (co_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil  INT          NOT NULL,
    ds_perfil  VARCHAR(50)  NOT NULL,
    PRIMARY KEY (co_perfil)
);

CREATE TABLE tb_plataforma (
    co_seq_plataforma  INT          NOT NULL AUTO_INCREMENT,
    no_plataforma      VARCHAR(100) NOT NULL,
    no_fabricante      VARCHAR(100) NOT NULL,
    nu_ano_lancamento  INT          NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero  INT          NOT NULL AUTO_INCREMENT,
    no_genero      VARCHAR(50)  NOT NULL,
    PRIMARY KEY (co_seq_genero),
    UNIQUE (no_genero)
);

CREATE TABLE tb_jogo (
    co_seq_jogo        INT           NOT NULL AUTO_INCREMENT,
    no_jogo            VARCHAR(150)  NOT NULL,
    ds_jogo            TEXT          NULL,
    nu_ano_lancamento  INT           NOT NULL,
    vl_preco           DECIMAL(10,2) NOT NULL,
    qt_estoque         INT           NOT NULL,
    st_ativo           TINYINT(1)    NOT NULL,
    dt_cadastro        DATETIME      NOT NULL,
    co_plataforma      INT           NOT NULL,
    co_condicao_jogo   INT           NOT NULL,
    PRIMARY KEY (co_seq_jogo),
    FOREIGN KEY (co_plataforma)    REFERENCES tb_plataforma (co_seq_plataforma),
    FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

CREATE TABLE tb_jogo_genero (
    co_jogo    INT NOT NULL,
    co_genero  INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),
    FOREIGN KEY (co_jogo)   REFERENCES tb_jogo (co_seq_jogo),
    FOREIGN KEY (co_genero) REFERENCES tb_genero (co_seq_genero)
);

CREATE TABLE tb_usuario (
    co_seq_usuario  INT          NOT NULL AUTO_INCREMENT,
    no_usuario      VARCHAR(100) NOT NULL,
    ds_email        VARCHAR(150) NOT NULL,
    ds_senha_hash   VARCHAR(255) NOT NULL,
    st_ativo        TINYINT(1)   NOT NULL,
    dt_cadastro     DATETIME     NOT NULL,
    co_perfil       INT          NOT NULL,
    PRIMARY KEY (co_seq_usuario),
    UNIQUE (ds_email),
    FOREIGN KEY (co_perfil) REFERENCES tb_perfil (co_perfil)
);

-- Dados das tabelas de domínio. Os valores de condição e gênero são SUGESTÃO: ajuste conforme o roteiro.
INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES (1, 'Administrador'), (2, 'Cliente');

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Lacrado'), (2, 'Excelente'), (3, 'Bom'), (4, 'Regular'), (5, 'Ruim');

INSERT INTO tb_genero (no_genero) VALUES
    ('Ação'), ('Aventura'), ('Plataforma'), ('RPG'), ('Esporte'), ('Corrida'),
    ('Luta'), ('Quebra-cabeça'), ('Tiro'), ('Estratégia'), ('Simulação');

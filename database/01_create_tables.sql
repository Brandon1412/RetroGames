
-- ============================================================================
-- 01_create_tables.sql
-- Criação inicial da estrutura do banco RetroGames
-- ============================================================================

CREATE DATABASE IF NOT EXISTS retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE retrogames;

-- 1. Condições de jogo
CREATE TABLE tb_condicao_jogo (
    co_seq_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_seq_condicao_jogo),
    UNIQUE (ds_condicao_jogo)
) ENGINE=InnoDB;

-- 2. Perfis de acesso
CREATE TABLE tb_perfil (
    co_seq_perfil INT NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_seq_perfil),
    UNIQUE (ds_perfil)
) ENGINE=InnoDB;

-- 3. Plataformas
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(80) NOT NULL,
    ds_plataforma VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE (no_plataforma)
) ENGINE=InnoDB;

-- 4. Gêneros
CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(50) NOT NULL,
    ds_genero VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (co_seq_genero),
    UNIQUE (no_genero)
) ENGINE=InnoDB;

-- 5. Jogos
CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo VARCHAR(500) NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_jogo),

    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma(co_seq_plataforma),

    CONSTRAINT fk_jogo_condicao
        FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo(co_seq_condicao_jogo)
) ENGINE=InnoDB;

-- 6. Usuários
CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(120) NOT NULL,
    ds_email VARCHAR(150) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    co_perfil INT NOT NULL,
    PRIMARY KEY (co_seq_usuario),
    UNIQUE (ds_email),

    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil(co_seq_perfil)
) ENGINE=InnoDB;

-- 7. Relacionamento entre jogos e gêneros
CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),

    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo(co_seq_jogo),

    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero)
        REFERENCES tb_genero(co_seq_genero)
) ENGINE=InnoDB;

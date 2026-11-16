-- ============================================================================
-- 02_corrigir_estrutura.sql
-- Alinha as tabelas criadas pelo 01_create_tables.sql ao dicionário de dados
-- da especificação (seções 4.1 a 4.3) e ao diagrama do modelo relacional.
--
-- O 01 já foi executado, então ele não é alterado (AGENTS.md: "Um script já
-- executado não é alterado"). Esta correção entra como um novo script.
--
-- Ordem de execução: 01 -> 02 -> 03 (dados) -> 04 (consultas) -> 05 (testes).
-- Execute no MySQL Workbench, com o banco retrogames, de uma vez só.
-- ============================================================================

USE retrogames;

-- ----------------------------------------------------------------------------
-- 1. Tabelas de domínio: chave fixa usa o prefixo co_ (e não co_seq_)
--    As FKs que apontam para elas são recriadas.
-- ----------------------------------------------------------------------------
ALTER TABLE tb_jogo    DROP FOREIGN KEY fk_jogo_condicao;
ALTER TABLE tb_usuario DROP FOREIGN KEY fk_usuario_perfil;

ALTER TABLE tb_condicao_jogo RENAME COLUMN co_seq_condicao_jogo TO co_condicao_jogo;
ALTER TABLE tb_perfil        RENAME COLUMN co_seq_perfil        TO co_perfil;

ALTER TABLE tb_jogo
    ADD CONSTRAINT fk_jogo_condicao
    FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo(co_condicao_jogo);

ALTER TABLE tb_usuario
    ADD CONSTRAINT fk_usuario_perfil
    FOREIGN KEY (co_perfil) REFERENCES tb_perfil(co_perfil);

-- ----------------------------------------------------------------------------
-- 2. tb_plataforma: co_seq_plataforma, no_plataforma, no_fabricante,
--    nu_ano_lancamento (sem ds_plataforma e sem st_ativo)
-- ----------------------------------------------------------------------------
ALTER TABLE tb_plataforma
    DROP COLUMN ds_plataforma,
    DROP COLUMN st_ativo,
    MODIFY no_plataforma VARCHAR(80) NOT NULL,
    ADD COLUMN no_fabricante VARCHAR(80) NOT NULL AFTER no_plataforma,
    ADD COLUMN nu_ano_lancamento INT NOT NULL AFTER no_fabricante;

-- ----------------------------------------------------------------------------
-- 3. tb_genero: co_seq_genero, no_genero (sem ds_genero e sem st_ativo)
-- ----------------------------------------------------------------------------
ALTER TABLE tb_genero
    DROP COLUMN ds_genero,
    DROP COLUMN st_ativo,
    MODIFY no_genero VARCHAR(50) NOT NULL;

-- ----------------------------------------------------------------------------
-- 4. tb_jogo: ds_jogo VARCHAR(1000) opcional e nu_ano_lancamento obrigatório
-- ----------------------------------------------------------------------------
ALTER TABLE tb_jogo
    MODIFY ds_jogo VARCHAR(1000) NULL,
    ADD COLUMN nu_ano_lancamento INT NOT NULL AFTER ds_jogo;

-- ----------------------------------------------------------------------------
-- 5. tb_usuario: tamanhos do dicionário de dados
-- ----------------------------------------------------------------------------
ALTER TABLE tb_usuario
    MODIFY ds_email      VARCHAR(256) NOT NULL,
    MODIFY ds_senha_hash VARCHAR(500) NOT NULL;

-- ----------------------------------------------------------------------------
-- Conferência: compare cada tabela com a seção 4.2 da especificação
-- ----------------------------------------------------------------------------
SHOW TABLES;
DESCRIBE tb_condicao_jogo;
DESCRIBE tb_perfil;
DESCRIBE tb_plataforma;
DESCRIBE tb_genero;
DESCRIBE tb_jogo;
DESCRIBE tb_jogo_genero;
DESCRIBE tb_usuario;

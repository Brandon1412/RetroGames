-- ============================================================================
-- 05_testes_integridade.sql
-- Testes de integridade do banco (Aula 1, etapa 5).
--
-- ATENÇÃO: execute UM TESTE POR VEZ (selecione o comando e use Ctrl+Enter).
-- Se rodar o arquivo inteiro, o Workbench para no primeiro erro.
--
-- Depois de cada teste, confira o erro obtido e anote o resultado real no
-- comentário "Resultado obtido". Ao final, rode a limpeza.
-- ============================================================================

USE retrogames;

-- ----------------------------------------------------------------------------
-- Teste 1: inserir jogo com co_plataforma inexistente
-- Esperado: erro 1452 (violação de chave estrangeira)
-- ----------------------------------------------------------------------------
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, co_plataforma, co_condicao_jogo)
VALUES ('Jogo de Teste FK', 1990, 10.00, 1, 99999, 1);
-- Resultado obtido: (anote aqui)

-- ----------------------------------------------------------------------------
-- Teste 2: inserir plataforma com nome já cadastrado
-- Esperado: erro 1062 (violação de UNIQUE)
-- ----------------------------------------------------------------------------
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);
-- Resultado obtido: (anote aqui)

-- ----------------------------------------------------------------------------
-- Teste 3: excluir plataforma que possui jogos
-- Esperado: erro 1451 (violação de chave estrangeira: há jogos vinculados)
-- ----------------------------------------------------------------------------
DELETE FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo';
-- Resultado obtido: (anote aqui)

-- ----------------------------------------------------------------------------
-- Teste 4: inserir jogo com no_jogo nulo
-- Esperado: erro 1048 (coluna não aceita NULL)
-- ----------------------------------------------------------------------------
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, co_plataforma, co_condicao_jogo)
VALUES (NULL, 1990, 10.00, 1,
        (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 1);
-- Resultado obtido: (anote aqui)

-- ----------------------------------------------------------------------------
-- Teste 5: inserir jogo com vl_preco igual a 0
-- Esperado: ACEITO. O banco não tem CHECK; a regra "preço maior que zero"
-- (RN06) fica no Service.
-- ----------------------------------------------------------------------------
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, co_plataforma, co_condicao_jogo)
VALUES ('Jogo de Teste Preco Zero', 1990, 0, 1,
        (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 1);
-- Resultado obtido: (anote aqui)

-- ----------------------------------------------------------------------------
-- Limpeza: remove os dados inseridos pelos testes (só o teste 5 gravou algo).
-- ----------------------------------------------------------------------------
DELETE FROM tb_jogo WHERE no_jogo IN ('Jogo de Teste FK', 'Jogo de Teste Preco Zero');

-- Conferência: não deve sobrar nenhum jogo de teste (0 linhas)
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE no_jogo LIKE 'Jogo de Teste%';

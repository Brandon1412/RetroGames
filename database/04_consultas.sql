-- ============================================================================
-- 04_consultas.sql
-- Consultas do catÃ¡logo (Aula 1, etapa 5). Rode uma de cada vez no Workbench
-- (selecione o trecho e use Ctrl+Enter) e confira com os seus dados.
-- ============================================================================

USE retrogames;

-- 1. CatÃ¡logo: jogos ativos com o nome da plataforma e a descriÃ§Ã£o da condiÃ§Ã£o,
--    ordenados por nome.
SELECT j.co_seq_jogo,
       j.no_jogo,
       p.no_plataforma,
       c.ds_condicao_jogo,
       j.nu_ano_lancamento,
       j.vl_preco,
       j.qt_estoque
FROM tb_jogo j
JOIN tb_plataforma   p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. GÃªneros de um jogo informado (troque o valor de @id_jogo).
SET @id_jogo = 1;

SELECT g.co_seq_genero,
       g.no_genero
FROM tb_jogo_genero jg
JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @id_jogo
ORDER BY g.no_genero;

-- 3. Jogos de um gÃªnero informado (troque o valor de @id_genero).
SET @id_genero = 1;

SELECT j.co_seq_jogo,
       j.no_jogo,
       j.vl_preco
FROM tb_jogo_genero jg
JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = @id_genero
  AND j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo as plataformas sem jogos
--    (LEFT JOIN: a plataforma sem jogos aparece com 0).
SELECT p.co_seq_plataforma,
       p.no_plataforma,
       COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome do jogo, com 5 itens por pÃ¡gina.
--    PÃ¡gina 1: OFFSET 0. PÃ¡gina 2: OFFSET 5. PÃ¡gina N: OFFSET (N - 1) * 5.
SET @busca = '%zelda%';

SELECT j.co_seq_jogo,
       j.no_jogo,
       j.vl_preco
FROM tb_jogo j
WHERE j.st_ativo = TRUE
  AND j.no_jogo COLLATE utf8mb4_unicode_ci
    LIKE @busca COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo
LIMIT 5 OFFSET 0;

-- Total de resultados da mesma busca (para calcular o nÃºmero de pÃ¡ginas):
SELECT COUNT(*) AS total
FROM tb_jogo j
WHERE j.st_ativo = TRUE
  AND j.no_jogo COLLATE utf8mb4_unicode_ci
    LIKE @busca COLLATE utf8mb4_unicode_ci;

-- Segunda pÃ¡gina. Para ver resultados nela, use uma busca mais ampla,
-- por exemplo @busca = '%a%', jÃ¡ que 'zelda' tem poucos resultados.
SET @busca = '%a%';

SELECT j.co_seq_jogo,
       j.no_jogo,
       j.vl_preco
FROM tb_jogo j
WHERE j.st_ativo = TRUE
  AND j.no_jogo COLLATE utf8mb4_unicode_ci
    LIKE @busca COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo
LIMIT 5 OFFSET 5;

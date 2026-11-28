START TRANSACTION;

UPDATE tb_plataforma
SET
    no_fabricante = CASE co_seq_plataforma
        WHEN 8 THEN 'Atari'
        WHEN 17 THEN 'Atari'
        WHEN 10 THEN 'Sega'
        WHEN 4 THEN 'Nintendo'
        WHEN 7 THEN 'Sega'
        WHEN 3 THEN 'Sega'
        WHEN 6 THEN 'Nintendo'
        WHEN 1 THEN 'Nintendo'
        WHEN 16 THEN 'Nintendo'
        WHEN 5 THEN 'Sony'
        WHEN 2 THEN 'Nintendo'
        WHEN 12 THEN 'Microsoft'
    END,
    nu_ano_lancamento = CASE co_seq_plataforma
        WHEN 8 THEN 1977
        WHEN 17 THEN 1993
        WHEN 10 THEN 1998
        WHEN 4 THEN 1989
        WHEN 7 THEN 1985
        WHEN 3 THEN 1988
        WHEN 6 THEN 1996
        WHEN 1 THEN 1983
        WHEN 16 THEN 2017
        WHEN 5 THEN 1994
        WHEN 2 THEN 1990
        WHEN 12 THEN 2001
    END,
    ds_categoria = CASE co_seq_plataforma
        WHEN 8 THEN 'Console de mesa'
        WHEN 17 THEN 'Console de mesa'
        WHEN 10 THEN 'Console de mesa'
        WHEN 4 THEN 'Console portÃ¡til'
        WHEN 7 THEN 'Console de mesa'
        WHEN 3 THEN 'Console de mesa'
        WHEN 6 THEN 'Console de mesa'
        WHEN 1 THEN 'Console de mesa'
        WHEN 16 THEN 'HÃ­brido'
        WHEN 5 THEN 'Console de mesa'
        WHEN 2 THEN 'Console de mesa'
        WHEN 12 THEN 'Console de mesa'
        ELSE ds_categoria
    END
WHERE co_seq_plataforma IN (1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 17);

COMMIT;

SELECT
    co_seq_plataforma AS ID,
    no_plataforma AS Nome,
    no_fabricante AS Fabricante,
    nu_ano_lancamento AS Ano,
    ds_categoria AS Categoria,
    ds_plataforma AS OutrasInformacoes,
    st_ativo AS Status
FROM tb_plataforma
WHERE co_seq_plataforma IN (1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 17)
ORDER BY no_plataforma;

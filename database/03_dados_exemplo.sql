-- ============================================================================
-- 03_dados_exemplo.sql
-- Dados de exemplo do RetroGames. Execute depois do 01 e do 02, num banco recém-criado.
-- Os usuários NÃO são inseridos aqui: o administrador será criado pela API (aula 6).
-- ============================================================================

USE retrogames;

-- Condições de jogo (chaves fixas)
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

-- Perfis de acesso (chaves fixas)
INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- Plataformas (Atari 2600 fica sem jogos de propósito, para o teste da contagem com 0)
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    ('Nintendo Entertainment System', 'Nintendo', 1983),
    ('Super Nintendo', 'Nintendo', 1990),
    ('Mega Drive', 'Sega', 1988),
    ('Game Boy', 'Nintendo', 1989),
    ('PlayStation', 'Sony', 1994),
    ('Nintendo 64', 'Nintendo', 1996),
    ('Master System', 'Sega', 1985),
    ('Atari 2600', 'Atari', 1977);

-- Gêneros
INSERT INTO tb_genero (no_genero) VALUES
    ('Ação'),
    ('Aventura'),
    ('Plataforma'),
    ('RPG'),
    ('Luta'),
    ('Corrida'),
    ('Puzzle'),
    ('Tiro'),
    ('Esporte');

-- Jogos (a plataforma é resolvida pelo nome; a data de cadastro usa o padrão da coluna)
-- Inclui 1 jogo com estoque zero (Street Fighter II Turbo) e 1 jogo inativo (Alex Kidd in Miracle World).
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo)
VALUES
    ('Super Mario Bros.', 'O clássico de plataforma que definiu uma geração.', 1985, 159.90, 5, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System'), 2),
    ('The Legend of Zelda', 'Explore Hyrule e reúna os fragmentos da Triforce.', 1986, 199.90, 3, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System'), 2),
    ('Contra', 'Ação run and gun para um ou dois jogadores.', 1987, 129.90, 4, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System'), 2),
    ('Mega Man 2', 'Derrote os oito Robot Masters do Dr. Wily.', 1988, 179.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System'), 3),
    ('Super Mario World', 'Mario e Yoshi em Dinosaur Land.', 1990, 219.90, 6, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 2),
    ('Super Metroid', 'Samus explora o planeta Zebes.', 1994, 349.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 2),
    ('Chrono Trigger', 'RPG com viagem no tempo e múltiplos finais.', 1995, 499.90, 1, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 3),
    ('Street Fighter II Turbo', 'Luta com os lutadores do mundo todo.', 1993, 189.90, 0, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 2),
    ('F-Zero', 'Corridas futuristas em alta velocidade.', 1990, 139.90, 3, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 2),
    ('Donkey Kong Country', 'Gráficos pré-renderizados e muita aventura.', 1994, 169.90, 4, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo'), 2),
    ('Sonic the Hedgehog', 'O mascote da Sega em alta velocidade.', 1991, 149.90, 5, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Mega Drive'), 2),
    ('Streets of Rage 2', 'Beat ''em up com trilha sonora marcante.', 1992, 229.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Mega Drive'), 2),
    ('Gunstar Heroes', 'Ação frenética da Treasure.', 1993, 259.90, 1, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Mega Drive'), 3),
    ('Phantasy Star IV', 'Encerra a saga de Phantasy Star.', 1993, 399.90, 1, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Mega Drive'), 2),
    ('Tetris', 'O quebra-cabeça mais famoso do mundo.', 1989, 99.90, 8, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Game Boy'), 2),
    ('Pokémon Red', 'Capture, treine e evolua seus monstros.', 1996, 299.90, 3, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Game Boy'), 2),
    ('The Legend of Zelda: Link''s Awakening', 'Link naufragado na ilha Koholint.', 1993, 249.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Game Boy'), 2),
    ('Final Fantasy VII', 'RPG que popularizou o gênero no Ocidente.', 1997, 279.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'PlayStation'), 2),
    ('Metal Gear Solid', 'Espionagem tática com Solid Snake.', 1998, 239.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'PlayStation'), 2),
    ('Crash Bandicoot', 'Plataforma 3D com muito desafio.', 1996, 129.90, 4, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'PlayStation'), 3),
    ('Super Mario 64', 'O salto do encanador para o 3D.', 1996, 299.90, 3, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 2),
    ('The Legend of Zelda: Ocarina of Time', 'Considerado um dos melhores jogos de todos os tempos.', 1998, 329.90, 2, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 2),
    ('GoldenEye 007', 'Tiro em primeira pessoa com multiplayer histórico.', 1997, 219.90, 3, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 2),
    ('Mario Kart 64', 'Corrida com itens para quatro jogadores.', 1996, 199.90, 5, TRUE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 3),
    ('Alex Kidd in Miracle World', 'Aventura de plataforma do Master System.', 1986, 189.90, 0, FALSE,
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Master System'), 1);

-- Vínculos jogo-gênero (cada jogo tem de 1 a 3 gêneros)
INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM (
    SELECT 'Super Mario Bros.' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Super Mario Bros.' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Contra' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Contra' AS no_jogo, 'Tiro' AS no_genero
    UNION ALL
    SELECT 'Mega Man 2' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Mega Man 2' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Super Mario World' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Super Mario World' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Super Metroid' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Super Metroid' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Chrono Trigger' AS no_jogo, 'RPG' AS no_genero
    UNION ALL
    SELECT 'Chrono Trigger' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Street Fighter II Turbo' AS no_jogo, 'Luta' AS no_genero
    UNION ALL
    SELECT 'F-Zero' AS no_jogo, 'Corrida' AS no_genero
    UNION ALL
    SELECT 'Donkey Kong Country' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Donkey Kong Country' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Sonic the Hedgehog' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Sonic the Hedgehog' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Streets of Rage 2' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Streets of Rage 2' AS no_jogo, 'Luta' AS no_genero
    UNION ALL
    SELECT 'Gunstar Heroes' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Gunstar Heroes' AS no_jogo, 'Tiro' AS no_genero
    UNION ALL
    SELECT 'Gunstar Heroes' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Phantasy Star IV' AS no_jogo, 'RPG' AS no_genero
    UNION ALL
    SELECT 'Tetris' AS no_jogo, 'Puzzle' AS no_genero
    UNION ALL
    SELECT 'Pokémon Red' AS no_jogo, 'RPG' AS no_genero
    UNION ALL
    SELECT 'Pokémon Red' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda: Link''s Awakening' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda: Link''s Awakening' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Final Fantasy VII' AS no_jogo, 'RPG' AS no_genero
    UNION ALL
    SELECT 'Final Fantasy VII' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Metal Gear Solid' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Metal Gear Solid' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'Crash Bandicoot' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Super Mario 64' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Super Mario 64' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda: Ocarina of Time' AS no_jogo, 'Aventura' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda: Ocarina of Time' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'The Legend of Zelda: Ocarina of Time' AS no_jogo, 'RPG' AS no_genero
    UNION ALL
    SELECT 'GoldenEye 007' AS no_jogo, 'Tiro' AS no_genero
    UNION ALL
    SELECT 'GoldenEye 007' AS no_jogo, 'Ação' AS no_genero
    UNION ALL
    SELECT 'Mario Kart 64' AS no_jogo, 'Corrida' AS no_genero
    UNION ALL
    SELECT 'Alex Kidd in Miracle World' AS no_jogo, 'Plataforma' AS no_genero
    UNION ALL
    SELECT 'Alex Kidd in Miracle World' AS no_jogo, 'Aventura' AS no_genero
) AS v
JOIN tb_jogo   j ON j.no_jogo   = v.no_jogo
JOIN tb_genero g ON g.no_genero = v.no_genero;

-- Conferência rápida
SELECT 'condicoes'   AS tabela, COUNT(*) AS qtd FROM tb_condicao_jogo
UNION ALL SELECT 'perfis',      COUNT(*) FROM tb_perfil
UNION ALL SELECT 'plataformas', COUNT(*) FROM tb_plataforma
UNION ALL SELECT 'generos',     COUNT(*) FROM tb_genero
UNION ALL SELECT 'jogos',       COUNT(*) FROM tb_jogo
UNION ALL SELECT 'vinculos',    COUNT(*) FROM tb_jogo_genero
UNION ALL SELECT 'usuarios',    COUNT(*) FROM tb_usuario;

-- Nenhum jogo pode ficar sem gênero (deve devolver 0 linhas)
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo j
WHERE NOT EXISTS (SELECT 1 FROM tb_jogo_genero jg WHERE jg.co_jogo = j.co_seq_jogo);

-- Deve existir 1 jogo inativo e 1 com estoque zero
SELECT no_jogo, st_ativo, qt_estoque FROM tb_jogo WHERE st_ativo = FALSE OR qt_estoque = 0;

-- Modelo usado por Scripts/02-Criar-Banco.ps1.
-- Nao execute com o marcador: substitua {{SENHA_ESCAPADA}} por uma senha entre aspas SQL.
-- A aplicacao usa somente esta conta; root fica restrito a administracao e backups.
-- CREATE USER falha se a conta ja existir: nao redefine silenciosamente a senha.
CREATE USER 'vandercake_app'@'127.0.0.1' IDENTIFIED BY {{SENHA_ESCAPADA}};
GRANT SELECT, INSERT, UPDATE, DELETE ON vandercake.* TO 'vandercake_app'@'127.0.0.1';
-- Nao e necessario FLUSH PRIVILEGES apos CREATE USER/GRANT.

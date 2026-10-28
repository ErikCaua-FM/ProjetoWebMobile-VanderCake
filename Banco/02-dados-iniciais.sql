USE vandercake;
SET NAMES utf8mb4;
START TRANSACTION;

INSERT IGNORE INTO Estados (Nome, Uf) VALUES
('Acre','AC'), ('Alagoas','AL'), ('Amapá','AP'), ('Amazonas','AM'), ('Bahia','BA'),
('Ceará','CE'), ('Distrito Federal','DF'), ('Espírito Santo','ES'), ('Goiás','GO'),
('Maranhão','MA'), ('Mato Grosso','MT'), ('Mato Grosso do Sul','MS'), ('Minas Gerais','MG'),
('Pará','PA'), ('Paraíba','PB'), ('Paraná','PR'), ('Pernambuco','PE'), ('Piauí','PI'),
('Rio de Janeiro','RJ'), ('Rio Grande do Norte','RN'), ('Rio Grande do Sul','RS'),
('Rondônia','RO'), ('Roraima','RR'), ('Santa Catarina','SC'), ('São Paulo','SP'),
('Sergipe','SE'), ('Tocantins','TO');
INSERT IGNORE INTO Cidades (EstadoId, Nome)
SELECT Id, 'Foz do Iguaçu' FROM Estados WHERE Uf = 'PR';
INSERT IGNORE INTO Cidades (EstadoId, Nome)
SELECT Id, 'Cascavel' FROM Estados WHERE Uf = 'PR';
INSERT IGNORE INTO Cidades (EstadoId, Nome)
SELECT Id, 'Curitiba' FROM Estados WHERE Uf = 'PR';

INSERT IGNORE INTO CondicoesPagamento (Descricao) VALUES ('À vista'), ('7/14 dias'), ('7/14/21 dias');
INSERT IGNORE INTO ParcelasCondicaoPagamento (CondicaoPagamentoId, NumeroParcela, DiasAposConfirmacao)
SELECT Id, 1, 0 FROM CondicoesPagamento WHERE Descricao = 'À vista';
INSERT IGNORE INTO ParcelasCondicaoPagamento (CondicaoPagamentoId, NumeroParcela, DiasAposConfirmacao)
SELECT Id, 1, 7 FROM CondicoesPagamento WHERE Descricao IN ('7/14 dias','7/14/21 dias');
INSERT IGNORE INTO ParcelasCondicaoPagamento (CondicaoPagamentoId, NumeroParcela, DiasAposConfirmacao)
SELECT Id, 2, 14 FROM CondicoesPagamento WHERE Descricao IN ('7/14 dias','7/14/21 dias');
INSERT IGNORE INTO ParcelasCondicaoPagamento (CondicaoPagamentoId, NumeroParcela, DiasAposConfirmacao)
SELECT Id, 3, 21 FROM CondicoesPagamento WHERE Descricao = '7/14/21 dias';

INSERT IGNORE INTO FormasPagamento (Descricao) VALUES
('Pix'), ('Boleto bancário'), ('Dinheiro em espécie'), ('Cartão de crédito'), ('Cartão de débito');
INSERT IGNORE INTO Categorias (Nome) VALUES
('Farinhas'), ('Chocolates'), ('Laticínios'), ('Fermentos'), ('Confeitaria');

COMMIT;
-- Não cria usuários com senha padrão. Use a API com --criar-admin.

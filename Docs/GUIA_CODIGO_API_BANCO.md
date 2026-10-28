# VanderCake — código da API, banco, scripts e integração

Este guia reproduz o código integral, separado por pasta e arquivo, com indentação preservada. O ZIP já traz todos os arquivos nos caminhos corretos. Comece pelo TUTORIAL_WINDOWS.md para instalar e pelo Docs/ARQUITETURA_E_BANCO.md para entender as classes e chaves estrangeiras.

Leia os Controllers para acompanhar as rotas; Services contém validações transacionais, pagamentos e tempo real; Models representa os dados e Contracts representa as entradas HTTP. Os testes usam dados fictícios em um schema exclusivo. Não copie as credenciais de teste para a aplicação da loja.

## Arquivos

- `.gitignore`
- `Banco/01-estrutura.sql`
- `Banco/02-dados-iniciais.sql`
- `Banco/03-usuario-api.sql`
- `Scripts/01-Instalar-MySQL.ps1`
- `Scripts/02-Criar-Banco.ps1`
- `Scripts/03-Configurar-Conexao.ps1`
- `Scripts/04-Iniciar-API.ps1`
- `Scripts/05-Testar.ps1`
- `TestesIntegracao/Program.cs`
- `TestesIntegracao/TestesIntegracao.csproj`
- `TestesIntegracao/web-http.mjs`
- `VanderCake.Api/Contracts/AtividadeRequest.cs`
- `VanderCake.Api/Contracts/CancelarRequest.cs`
- `VanderCake.Api/Contracts/CategoriaRequest.cs`
- `VanderCake.Api/Contracts/CidadeRequest.cs`
- `VanderCake.Api/Contracts/ConfirmarRequest.cs`
- `VanderCake.Api/Contracts/EstadoRequest.cs`
- `VanderCake.Api/Contracts/ItemPedidoEnvio.cs`
- `VanderCake.Api/Contracts/LoginRequisicao.cs`
- `VanderCake.Api/Contracts/NovoClienteRequisicao.cs`
- `VanderCake.Api/Contracts/PagamentoCadastroRequest.cs`
- `VanderCake.Api/Contracts/ParcelaEnvio.cs`
- `VanderCake.Api/Contracts/PedidoEnvio.cs`
- `VanderCake.Api/Contracts/PedidoGeralEnvio.cs`
- `VanderCake.Api/Contracts/PessoaRequest.cs`
- `VanderCake.Api/Contracts/ProdutoRequest.cs`
- `VanderCake.Api/Contracts/RenovarRequest.cs`
- `VanderCake.Api/Contracts/VersaoRequest.cs`
- `VanderCake.Api/Controllers/AdminController.cs`
- `VanderCake.Api/Controllers/AuthController.cs`
- `VanderCake.Api/Controllers/MobileController.cs`
- `VanderCake.Api/Hubs/AtualizacoesHub.cs`
- `VanderCake.Api/Infrastructure/ApiException.cs`
- `VanderCake.Api/Infrastructure/Banco.cs`
- `VanderCake.Api/Infrastructure/Inicializacao.cs`
- `VanderCake.Api/Infrastructure/Regras.cs`
- `VanderCake.Api/Models/Administrador.cs`
- `VanderCake.Api/Models/Categoria.cs`
- `VanderCake.Api/Models/Cidade.cs`
- `VanderCake.Api/Models/Cliente.cs`
- `VanderCake.Api/Models/CondicaoPagamento.cs`
- `VanderCake.Api/Models/ContaReceber.cs`
- `VanderCake.Api/Models/ControleEnvio.cs`
- `VanderCake.Api/Models/DadosLoja.cs`
- `VanderCake.Api/Models/Estado.cs`
- `VanderCake.Api/Models/EventoAlteracao.cs`
- `VanderCake.Api/Models/FormaPagamento.cs`
- `VanderCake.Api/Models/ItemPedido.cs`
- `VanderCake.Api/Models/ItemVenda.cs`
- `VanderCake.Api/Models/Operacao.cs`
- `VanderCake.Api/Models/ParcelaPedido.cs`
- `VanderCake.Api/Models/Pedido.cs`
- `VanderCake.Api/Models/PedidoGeral.cs`
- `VanderCake.Api/Models/Pessoa.cs`
- `VanderCake.Api/Models/Previa.cs`
- `VanderCake.Api/Models/Produto.cs`
- `VanderCake.Api/Models/RegraParcela.cs`
- `VanderCake.Api/Models/Venda.cs`
- `VanderCake.Api/Models/Vendedor.cs`
- `VanderCake.Api/Program.cs`
- `VanderCake.Api/Properties/launchSettings.json`
- `VanderCake.Api/Services/AutenticacaoService.cs`
- `VanderCake.Api/Services/CadastroService.cs`
- `VanderCake.Api/Services/EventosService.cs`
- `VanderCake.Api/Services/LeituraService.cs`
- `VanderCake.Api/Services/PagamentosService.cs`
- `VanderCake.Api/Services/PedidosService.cs`
- `VanderCake.Api/Services/ProjecaoWeb.cs`
- `VanderCake.Api/Services/RateioService.cs`
- `VanderCake.Api/VanderCake.Api.csproj`
- `VanderCake.Api/appsettings.json`

## .gitignore

````text
**/bin/
**/obj/
**/node_modules/
.vs/
**/*.user
**/*.db3
**/*.sqlite
**/*.sqlite-*
**/ApiKeys/
**/appsettings.Local.json
````

## Banco/01-estrutura.sql

````sql
-- VanderCake: MySQL 8.4, InnoDB, UTF-8. Execute com o cliente mysql.
-- Primeira instalação: não apaga dados. Alterações futuras exigem migração versionada.
CREATE DATABASE IF NOT EXISTS vandercake CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE vandercake;
SET NAMES utf8mb4;
SET SESSION time_zone = '+00:00';

CREATE TABLE IF NOT EXISTS Estados (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    Uf CHAR(2) NOT NULL,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Estados_Uf (Uf),
    CONSTRAINT CK_Estados_Uf CHECK (REGEXP_LIKE(Uf, '^[A-Z]{2}$', 'c'))
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Cidades (
    Id INT NOT NULL AUTO_INCREMENT,
    EstadoId INT NOT NULL,
    Nome VARCHAR(100) NOT NULL,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Cidades_EstadoNome (EstadoId, Nome),
    CONSTRAINT FK_Cidades_Estados FOREIGN KEY (EstadoId) REFERENCES Estados(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Administradores (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    Documento VARCHAR(20) NOT NULL,
    Rua VARCHAR(100) NOT NULL,
    Bairro VARCHAR(60) NOT NULL,
    Cep VARCHAR(9) NOT NULL,
    Numero VARCHAR(10) NOT NULL,
    CidadeId INT NOT NULL,
    Telefone VARCHAR(20) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    SenhaHash VARCHAR(255) NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    VersaoSeguranca INT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Administradores_Documento (Documento),
    UNIQUE KEY UQ_Administradores_Email (Email),
    CONSTRAINT FK_Administradores_Cidades FOREIGN KEY (CidadeId) REFERENCES Cidades(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Vendedores (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    Documento VARCHAR(20) NOT NULL,
    Rua VARCHAR(100) NOT NULL,
    Bairro VARCHAR(60) NOT NULL,
    Cep VARCHAR(9) NOT NULL,
    Numero VARCHAR(10) NOT NULL,
    CidadeId INT NOT NULL,
    Telefone VARCHAR(20) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    SenhaHash VARCHAR(255) NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    VersaoSeguranca INT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Vendedores_Documento (Documento),
    UNIQUE KEY UQ_Vendedores_Email (Email),
    CONSTRAINT FK_Vendedores_Cidades FOREIGN KEY (CidadeId) REFERENCES Cidades(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Clientes (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    Documento VARCHAR(20) NOT NULL,
    Rua VARCHAR(100) NOT NULL,
    Bairro VARCHAR(60) NOT NULL,
    Cep VARCHAR(9) NOT NULL,
    Numero VARCHAR(10) NOT NULL,
    CidadeId INT NOT NULL,
    Telefone VARCHAR(20) NOT NULL,
    Email VARCHAR(150) NULL,
    VendedorId INT NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Clientes_Documento (Documento),
    KEY IX_Clientes_Carteira (VendedorId, Ativo, Nome),
    CONSTRAINT FK_Clientes_Cidades FOREIGN KEY (CidadeId) REFERENCES Cidades(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Clientes_Vendedores FOREIGN KEY (VendedorId) REFERENCES Vendedores(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Categorias (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(60) NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Categorias_Nome (Nome)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Produtos (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(100) NOT NULL,
    CategoriaId INT NOT NULL,
    Descricao TEXT NULL,
    Preco DECIMAL(12,2) NOT NULL,
    Estoque INT NOT NULL DEFAULT 0,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    KEY IX_Produtos_Catalogo (Ativo, CategoriaId, Nome),
    CONSTRAINT FK_Produtos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_Produtos_Preco CHECK (Preco > 0),
    CONSTRAINT CK_Produtos_Estoque CHECK (Estoque >= 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS CondicoesPagamento (
    Id INT NOT NULL AUTO_INCREMENT,
    Descricao VARCHAR(60) NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Condicoes_Descricao (Descricao)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ParcelasCondicaoPagamento (
    Id INT NOT NULL AUTO_INCREMENT,
    CondicaoPagamentoId INT NOT NULL,
    NumeroParcela INT NOT NULL,
    DiasAposConfirmacao INT NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_ParcelasCondicao_Numero (CondicaoPagamentoId, NumeroParcela),
    CONSTRAINT FK_ParcelasCondicao_Condicoes FOREIGN KEY (CondicaoPagamentoId) REFERENCES CondicoesPagamento(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_ParcelasCondicao_Numero CHECK (NumeroParcela BETWEEN 1 AND 3),
    CONSTRAINT CK_ParcelasCondicao_Dias CHECK (DiasAposConfirmacao >= 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS FormasPagamento (
    Id INT NOT NULL AUTO_INCREMENT,
    Descricao VARCHAR(50) NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Formas_Descricao (Descricao)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS PedidoGeral (
    Id INT NOT NULL AUTO_INCREMENT,
    IdentificadorEnvio CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    VendedorId INT NOT NULL,
    DataHoraCriacao DATETIME(6) NOT NULL,
    DataHoraRecebimento DATETIME(6) NOT NULL,
    DataHoraProcessamento DATETIME(6) NULL,
    AdministradorId INT NULL,
    Status VARCHAR(30) NOT NULL DEFAULT 'AguardandoConfirmacao',
    MotivoCancelamento VARCHAR(255) NULL,
    Versao BIGINT NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_PedidoGeral_Envio (IdentificadorEnvio),
    KEY IX_PedidoGeral_Pendentes (Status, DataHoraRecebimento),
    KEY IX_PedidoGeral_Vendedor (VendedorId, Id),
    CONSTRAINT FK_PedidoGeral_Vendedor FOREIGN KEY (VendedorId) REFERENCES Vendedores(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_PedidoGeral_Administrador FOREIGN KEY (AdministradorId) REFERENCES Administradores(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_PedidoGeral_Status CHECK (Status IN ('AguardandoConfirmacao','Processado','Cancelado')),
    CONSTRAINT CK_PedidoGeral_Processamento CHECK (
        (Status = 'AguardandoConfirmacao' AND DataHoraProcessamento IS NULL AND AdministradorId IS NULL AND MotivoCancelamento IS NULL)
        OR (Status = 'Processado' AND DataHoraProcessamento IS NOT NULL AND AdministradorId IS NOT NULL AND MotivoCancelamento IS NULL)
        OR (Status = 'Cancelado' AND DataHoraProcessamento IS NOT NULL AND AdministradorId IS NOT NULL AND MotivoCancelamento IS NOT NULL AND LENGTH(TRIM(MotivoCancelamento)) > 0)
    )
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Pedidos (
    Id INT NOT NULL AUTO_INCREMENT,
    PedidoGeralId INT NOT NULL,
    ClienteId INT NOT NULL,
    CondicaoPagamentoId INT NOT NULL,
    DataHoraCriacao DATETIME(6) NOT NULL,
    ValorTotalSolicitado DECIMAL(12,2) NOT NULL,
    ValorTotalAtendido DECIMAL(12,2) NULL,
    Status VARCHAR(30) NOT NULL DEFAULT 'Pendente',
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Pedidos_ClienteLote (PedidoGeralId, ClienteId),
    KEY IX_Pedidos_Cliente (ClienteId, DataHoraCriacao),
    CONSTRAINT FK_Pedidos_Lote FOREIGN KEY (PedidoGeralId) REFERENCES PedidoGeral(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Pedidos_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Pedidos_Condicao FOREIGN KEY (CondicaoPagamentoId) REFERENCES CondicoesPagamento(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_Pedidos_Total CHECK (ValorTotalSolicitado > 0 AND (ValorTotalAtendido IS NULL OR ValorTotalAtendido BETWEEN 0 AND ValorTotalSolicitado)),
    CONSTRAINT CK_Pedidos_Status CHECK (
        (Status IN ('Pendente','Cancelado') AND ValorTotalAtendido IS NULL)
        OR (Status = 'NaoAtendido' AND ValorTotalAtendido = 0)
        OR (Status IN ('Confirmado','ConfirmadoComCorte') AND ValorTotalAtendido > 0)
    )
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ItemPedido (
    Id INT NOT NULL AUTO_INCREMENT,
    PedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    NomeProduto VARCHAR(100) NOT NULL,
    QuantidadeSolicitada INT NOT NULL,
    QuantidadeAtendida INT NULL,
    PrecoUnitario DECIMAL(12,2) NOT NULL,
    SubtotalSolicitado DECIMAL(12,2) GENERATED ALWAYS AS (QuantidadeSolicitada * PrecoUnitario) STORED,
    SubtotalAtendido DECIMAL(12,2) GENERATED ALWAYS AS (QuantidadeAtendida * PrecoUnitario) STORED,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_ItemPedido_Produto (PedidoId, ProdutoId),
    UNIQUE KEY UQ_ItemPedido_Origem (Id, PedidoId, ProdutoId),
    CONSTRAINT FK_ItemPedido_Pedido FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_ItemPedido_Produto FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_ItemPedido_Quantidade CHECK (QuantidadeSolicitada > 0 AND (QuantidadeAtendida IS NULL OR QuantidadeAtendida BETWEEN 0 AND QuantidadeSolicitada)),
    CONSTRAINT CK_ItemPedido_Preco CHECK (PrecoUnitario > 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ParcelasPedido (
    Id INT NOT NULL AUTO_INCREMENT,
    PedidoId INT NOT NULL,
    NumeroParcela INT NOT NULL,
    DiasAposConfirmacao INT NOT NULL,
    FormaPagamentoId INT NOT NULL,
    ValorPrevisto DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_ParcelasPedido_Numero (PedidoId, NumeroParcela),
    UNIQUE KEY UQ_ParcelasPedido_Origem (Id, PedidoId),
    CONSTRAINT FK_ParcelasPedido_Pedido FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_ParcelasPedido_Forma FOREIGN KEY (FormaPagamentoId) REFERENCES FormasPagamento(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_ParcelasPedido_Valores CHECK (NumeroParcela BETWEEN 1 AND 3 AND DiasAposConfirmacao >= 0 AND ValorPrevisto >= 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Vendas (
    Id INT NOT NULL AUTO_INCREMENT,
    PedidoId INT NOT NULL,
    DataHoraConfirmacao DATETIME(6) NOT NULL,
    ValorTotal DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_Vendas_Pedido (PedidoId),
    UNIQUE KEY UQ_Vendas_Origem (Id, PedidoId),
    CONSTRAINT FK_Vendas_Pedido FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_Vendas_Total CHECK (ValorTotal > 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ItemVenda (
    Id INT NOT NULL AUTO_INCREMENT,
    VendaId INT NOT NULL,
    PedidoId INT NOT NULL,
    ItemPedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    NomeProduto VARCHAR(100) NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(12,2) NOT NULL,
    Subtotal DECIMAL(12,2) GENERATED ALWAYS AS (Quantidade * PrecoUnitario) STORED,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_ItemVenda_ItemPedido (ItemPedidoId),
    UNIQUE KEY UQ_ItemVenda_Produto (VendaId, ProdutoId),
    CONSTRAINT FK_ItemVenda_Venda FOREIGN KEY (VendaId, PedidoId) REFERENCES Vendas(Id, PedidoId) ON DELETE RESTRICT,
    CONSTRAINT FK_ItemVenda_Origem FOREIGN KEY (ItemPedidoId, PedidoId, ProdutoId) REFERENCES ItemPedido(Id, PedidoId, ProdutoId) ON DELETE RESTRICT,
    CONSTRAINT CK_ItemVenda_Valores CHECK (Quantidade > 0 AND PrecoUnitario > 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ContasReceber (
    Id INT NOT NULL AUTO_INCREMENT,
    VendaId INT NOT NULL,
    PedidoId INT NOT NULL,
    ParcelaPedidoId INT NOT NULL,
    DataVencimento DATE NOT NULL,
    Valor DECIMAL(12,2) NOT NULL,
    Status VARCHAR(15) NOT NULL DEFAULT 'Pendente',
    DataPagamento DATE NULL,
    AdministradorBaixaId INT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UQ_ContasReceber_Parcela (ParcelaPedidoId),
    KEY IX_ContasReceber_Vencimento (Status, DataVencimento),
    CONSTRAINT FK_ContasReceber_Venda FOREIGN KEY (VendaId, PedidoId) REFERENCES Vendas(Id, PedidoId) ON DELETE RESTRICT,
    CONSTRAINT FK_ContasReceber_Parcela FOREIGN KEY (ParcelaPedidoId, PedidoId) REFERENCES ParcelasPedido(Id, PedidoId) ON DELETE RESTRICT,
    CONSTRAINT FK_ContasReceber_Administrador FOREIGN KEY (AdministradorBaixaId) REFERENCES Administradores(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_ContasReceber_Valor CHECK (Valor > 0),
    CONSTRAINT CK_ContasReceber_Status CHECK (
        (Status = 'Pendente' AND DataPagamento IS NULL AND AdministradorBaixaId IS NULL)
        OR (Status = 'Paga' AND DataPagamento IS NOT NULL AND AdministradorBaixaId IS NOT NULL)
    )
) ENGINE=InnoDB;

-- Uma rejeição definitiva fica registrada para o mesmo UUID nunca ser aceito depois.
CREATE TABLE IF NOT EXISTS ControleEnvios (
    IdentificadorEnvio CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    VendedorId INT NOT NULL,
    ConteudoHash CHAR(64) CHARACTER SET ascii NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'EmProcessamento',
    PedidoGeralId INT NULL,
    CodigoRejeicao INT NULL,
    MotivoRejeicao VARCHAR(1000) NULL,
    CriadoEm DATETIME(6) NOT NULL,
    PRIMARY KEY (IdentificadorEnvio),
    UNIQUE KEY UQ_ControleEnvios_Lote (PedidoGeralId),
    CONSTRAINT FK_ControleEnvios_Vendedor FOREIGN KEY (VendedorId) REFERENCES Vendedores(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_ControleEnvios_Lote FOREIGN KEY (PedidoGeralId) REFERENCES PedidoGeral(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_ControleEnvios_Status CHECK (Status IN ('EmProcessamento','Aceito','Rejeitado'))
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Operacoes (
    Identificador CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    PedidoGeralId INT NOT NULL,
    AdministradorId INT NOT NULL,
    Acao VARCHAR(20) NOT NULL,
    ConteudoHash CHAR(64) CHARACTER SET ascii NOT NULL,
    CriadoEm DATETIME(6) NOT NULL,
    PRIMARY KEY (Identificador),
    CONSTRAINT FK_Operacoes_Lote FOREIGN KEY (PedidoGeralId) REFERENCES PedidoGeral(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Operacoes_Administrador FOREIGN KEY (AdministradorId) REFERENCES Administradores(Id) ON DELETE RESTRICT,
    CONSTRAINT CK_Operacoes_Acao CHECK (Acao IN ('Confirmar','Cancelar'))
) ENGINE=InnoDB;

-- Outbox: a alteração e a notificação são gravadas na mesma transação.
CREATE TABLE IF NOT EXISTS EventosAlteracao (
    Id BIGINT NOT NULL AUTO_INCREMENT,
    Tipo VARCHAR(60) NOT NULL,
    VendedorId INT NULL,
    CriadoEm DATETIME(6) NOT NULL,
    PublicadoEm DATETIME(6) NULL,
    PRIMARY KEY (Id),
    KEY IX_EventosAlteracao_Pendentes (PublicadoEm, Id),
    CONSTRAINT FK_EventosAlteracao_Vendedor FOREIGN KEY (VendedorId) REFERENCES Vendedores(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS VersoesBanco (
    Versao INT NOT NULL,
    AplicadaEm DATETIME(6) NOT NULL,
    PRIMARY KEY (Versao)
) ENGINE=InnoDB;
INSERT IGNORE INTO VersoesBanco (Versao, AplicadaEm) VALUES (1, UTC_TIMESTAMP(6));
````

## Banco/02-dados-iniciais.sql

````sql
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
````

## Banco/03-usuario-api.sql

````sql
-- Modelo usado por Scripts/02-Criar-Banco.ps1.
-- Nao execute com o marcador: substitua {{SENHA_ESCAPADA}} por uma senha entre aspas SQL.
-- A aplicacao usa somente esta conta; root fica restrito a administracao e backups.
-- CREATE USER falha se a conta ja existir: nao redefine silenciosamente a senha.
CREATE USER 'vandercake_app'@'127.0.0.1' IDENTIFIED BY {{SENHA_ESCAPADA}};
GRANT SELECT, INSERT, UPDATE, DELETE ON vandercake.* TO 'vandercake_app'@'127.0.0.1';
-- Nao e necessario FLUSH PRIVILEGES apos CREATE USER/GRANT.
````

## Scripts/01-Instalar-MySQL.ps1

````powershell
# Execute em PowerShell como Administrador. Somente para instalacao NOVA.
[CmdletBinding()]
param([string]$Pasta = 'C:\VanderCake', [string]$Versao = '8.4.13')
$ErrorActionPreference = 'Stop'
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Abra o PowerShell como Administrador para instalar o servico.'
}
$mysql = Join-Path $Pasta 'mysql'
$dados = Join-Path $Pasta 'mysql-data'
$ini = Join-Path $Pasta 'my.ini'
if ((Get-Service -Name 'VanderCakeMySQL' -ErrorAction SilentlyContinue) -or
    (Test-Path $dados) -or (Test-Path $mysql) -or (Test-Path $ini)) {
    throw 'Ja existem arquivos ou servico VanderCakeMySQL. Nao inicialize novamente; consulte o tutorial.'
}
if (Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorAction SilentlyContinue) {
    throw 'A porta 3306 ja esta ocupada. Identifique a instalacao existente antes de continuar.'
}
New-Item -ItemType Directory -Path $Pasta -Force | Out-Null
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$redistribuivel = Join-Path $Pasta 'vc_redist.x64.exe'
Invoke-WebRequest 'https://aka.ms/vc14/vc_redist.x64.exe' -OutFile $redistribuivel -UseBasicParsing
$resultado = Start-Process $redistribuivel -ArgumentList '/install','/quiet','/norestart' -Wait -PassThru
if ($resultado.ExitCode -notin @(0, 3010, 1638)) { throw "Falha no Visual C++ Redistributable: $($resultado.ExitCode)" }
$zip = Join-Path $Pasta "mysql-$Versao-winx64.zip"
Invoke-WebRequest "https://cdn.mysql.com/Downloads/MySQL-8.4/mysql-$Versao-winx64.zip" -OutFile $zip -UseBasicParsing
if ($Versao -eq '8.4.13' -and (Get-FileHash $zip -Algorithm MD5).Hash -ne '7B94489D5524DEA9B27097068BEA25A8') {
    throw 'O arquivo baixado nao corresponde ao checksum publicado pela Oracle. Nao foi instalado.'
}
$extracao = Join-Path $Pasta ('extracao-' + [Guid]::NewGuid().ToString('N'))
Expand-Archive -Path $zip -DestinationPath $extracao
Move-Item (Join-Path $extracao "mysql-$Versao-winx64") $mysql
Remove-Item $extracao
$baseSql = $mysql.Replace('\','/')
$dadosSql = $dados.Replace('\','/')
@"
[mysqld]
basedir=$baseSql
datadir=$dadosSql
port=3306
bind-address=127.0.0.1
mysqlx=0
character-set-server=utf8mb4
collation-server=utf8mb4_0900_ai_ci
default-time-zone=+00:00

[client]
port=3306
default-character-set=utf8mb4
"@ | Set-Content $ini -Encoding ASCII
$mysqld = Join-Path $mysql 'bin\mysqld.exe'
Write-Host 'Anote a senha TEMPORARIA de root que aparecera abaixo. A inicializacao e executada uma unica vez.'
& $mysqld "--defaults-file=$ini" --initialize --console
if ($LASTEXITCODE -ne 0) { throw 'Falha na inicializacao. Preserve a pasta de dados e consulte o log.' }
# Na instalacao do servico, --install deve vir antes de --defaults-file.
& $mysqld --install VanderCakeMySQL "--defaults-file=$ini"
if ($LASTEXITCODE -ne 0) { throw 'Nao foi possivel registrar o servico.' }
Start-Service VanderCakeMySQL
Set-Service VanderCakeMySQL -StartupType Automatic
Get-Service VanderCakeMySQL
Write-Host 'MySQL instalado. Proximo passo: trocar a senha temporaria de root, conforme o tutorial.'
````

## Scripts/02-Criar-Banco.ps1

````powershell
# Execute depois de definir a senha definitiva de root. Nao altera contas existentes.
[CmdletBinding()]
param([string]$MySqlExe = 'C:\VanderCake\mysql\bin\mysql.exe', [int]$Porta = 3306)
$ErrorActionPreference = 'Stop'
$projeto = Split-Path $PSScriptRoot -Parent
$api = Join-Path $projeto 'VanderCake.Api\VanderCake.Api.csproj'
if (-not (Test-Path $MySqlExe)) { throw 'mysql.exe nao encontrado. Confira -MySqlExe.' }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Instale o SDK .NET 10 e reabra o terminal.' }
Write-Host 'Escolha a senha do usuario TECNICO vandercake_app. Nao e a senha de login da WEB.'
$segredo = Read-Host 'Senha do usuario da API (minimo 12 caracteres)' -AsSecureString
$ponteiro = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segredo)
try { $senha = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiro) }
finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiro) }
if ($senha.Length -lt 12) { throw 'Use pelo menos 12 caracteres para a conta tecnica.' }
if ($senha.Contains("`r") -or $senha.Contains("`n")) { throw 'A senha nao pode conter quebras de linha.' }
$arquivo = Join-Path ([IO.Path]::GetTempPath()) ('vandercake-setup-' + [Guid]::NewGuid().ToString('N') + '.sql')
try {
    New-Item -ItemType File -Path $arquivo | Out-Null
    $acl = New-Object Security.AccessControl.FileSecurity
    $acl.SetAccessRuleProtection($true, $false)
    $regra = New-Object Security.AccessControl.FileSystemAccessRule(
        [Security.Principal.WindowsIdentity]::GetCurrent().User, 'FullControl', 'Allow')
    $acl.AddAccessRule($regra)
    Set-Acl -Path $arquivo -AclObject $acl
    $senhaSql = "'" + $senha.Replace('\','\\').Replace("'","''") + "'"
    # CREATE USER falha se ja existir, sem trocar sua senha.
    $usuario = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\03-usuario-api.sql')).Replace('{{SENHA_ESCAPADA}}', $senhaSql)
    # O GRANT requer o banco. CREATE DATABASE nao apaga uma base existente.
    $estrutura = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\01-estrutura.sql'))
    $iniciais = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\02-dados-iniciais.sql'))
    [IO.File]::WriteAllText($arquivo, $estrutura + "`r`n" + $iniciais + "`r`n" + $usuario, (New-Object Text.UTF8Encoding($false)))
    Write-Host 'Agora digite a senha definitiva de root no prompt do MySQL.'
    $source = $arquivo.Replace('\','/')
    & $MySqlExe --host=127.0.0.1 "--port=$Porta" --user=root --password --default-character-set=utf8mb4 --abort-source-on-error "--execute=SOURCE $source"
    if ($LASTEXITCODE -ne 0) { throw 'O SQL nao foi concluido. A conexao da API nao foi alterada. Confira a mensagem do MySQL.' }
    $cs = New-Object System.Data.Common.DbConnectionStringBuilder
    $cs['Server'] = '127.0.0.1'
    $cs['Port'] = $Porta
    $cs['Database'] = 'vandercake'
    $cs['User ID'] = 'vandercake_app'
    $cs['Password'] = $senha
    $cs['SslMode'] = 'Preferred'
    $json = @{ 'ConnectionStrings:VanderCake' = $cs.ConnectionString } | ConvertTo-Json -Compress
    $OutputEncoding = New-Object Text.UTF8Encoding($false)
    $json | & dotnet user-secrets set --project $api
    if ($LASTEXITCODE -ne 0) { throw 'Banco criado, mas a configuracao da API falhou. Use 03-Configurar-Conexao.ps1.' }
    Write-Host 'Banco e usuario criados; conexao da API configurada. Crie o administrador com --criar-admin.'
}
finally {
    if (Test-Path $arquivo) { Remove-Item $arquivo -Force }
    $senha = $null
    $senhaSql = $null
    $json = $null
    $segredo.Dispose()
}
````

## Scripts/03-Configurar-Conexao.ps1

````powershell
# Use se a conta vandercake_app ja foi criada ou se a senha do banco mudou.
[CmdletBinding()]
param([string]$Servidor = '127.0.0.1', [int]$Porta = 3306)
$ErrorActionPreference = 'Stop'
$api = Join-Path (Split-Path $PSScriptRoot -Parent) 'VanderCake.Api\VanderCake.Api.csproj'
$segredo = Read-Host 'Senha atual do usuario MySQL vandercake_app' -AsSecureString
$ponteiro = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segredo)
try {
    $cs = New-Object System.Data.Common.DbConnectionStringBuilder
    $cs['Server'] = $Servidor
    $cs['Port'] = $Porta
    $cs['Database'] = 'vandercake'
    $cs['User ID'] = 'vandercake_app'
    $cs['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiro)
    $cs['SslMode'] = 'Preferred'
    $json = @{ 'ConnectionStrings:VanderCake' = $cs.ConnectionString } | ConvertTo-Json -Compress
    $OutputEncoding = New-Object Text.UTF8Encoding($false)
    $json | & dotnet user-secrets set --project $api
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao salvar a conexao da API.' }
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiro)
    $segredo.Dispose()
    $json = $null
}
````

## Scripts/04-Iniciar-API.ps1

````powershell
[CmdletBinding()]
param([switch]$RedeLocal)
$ErrorActionPreference = 'Stop'
$api = Join-Path (Split-Path $PSScriptRoot -Parent) 'VanderCake.Api\VanderCake.Api.csproj'
# Development carrega os user-secrets do usuario que executar este terminal.
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$endereco = if ($RedeLocal) { 'http://0.0.0.0:5080' } else { 'http://localhost:5080' }
& dotnet run --project $api --no-launch-profile -- --urls $endereco
if ($LASTEXITCODE -ne 0) { throw 'A API encerrou com erro. Confira as mensagens acima.' }
````

## Scripts/05-Testar.ps1

````powershell
# Testes locais de regressao. Integracao MySQL possui instrucoes proprias no tutorial.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
& dotnet build (Join-Path $raiz 'VanderCake.Api\VanderCake.Api.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Falha na compilacao da API.' }
& dotnet run --project (Join-Path $raiz 'VanderCake.Mobile\tests\VanderCake.Tests\VanderCake.Tests.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes do Mobile.' }
Push-Location (Join-Path $raiz 'VanderCake.Web')
try {
    & npm test
    if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes da Web.' }
}
finally { Pop-Location }
````

## TestesIntegracao/Program.cs

````csharp
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using MySqlConnector;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Services;

// Cria um banco exclusivo e inicia outra API, sem tocar no banco vandercake da loja.
var conexao = Environment.GetEnvironmentVariable("VANDERCAKE_TEST_MYSQL");
if (string.IsNullOrWhiteSpace(conexao))
{
    Console.WriteLine("Defina VANDERCAKE_TEST_MYSQL com uma conexão administrativa de TESTES, sem Database.");
    return;
}
var raiz = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var nomeBanco = "vandercake_teste_" + Guid.NewGuid().ToString("N");
var cs = new MySqlConnectionStringBuilder(conexao) { Database = "", AllowUserVariables = true, DateTimeKind = MySqlDateTimeKind.Utc };
await using var db = new MySqlConnection(cs.ConnectionString);
await db.OpenAsync();
foreach (var arquivo in new[] { "01-estrutura.sql", "02-dados-iniciais.sql" })
    await db.ExecuteAsync((await File.ReadAllTextAsync(Path.Combine(raiz, "Banco", arquivo))).Replace("vandercake", nomeBanco));
await db.ChangeDatabaseAsync(nomeBanco);
cs.Database = nomeBanco;
int testes = 0;
void Ok(bool condicao, string descricao)
{
    if (!condicao) throw new Exception("FALHOU: " + descricao);
    testes++;
    Console.WriteLine("OK: " + descricao);
}
Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE()") == 21,
    "Os scripts criam as 21 tabelas no MySQL real");
Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ParcelasCondicaoPagamento") == 6, "Condições de pagamento com 1, 2 e 3 parcelas");
string senha = "Teste-integracao-2026!";
var hash = new PasswordHasher<object>().HashPassword(new object(), senha);
await db.ExecuteAsync("""
    INSERT INTO Administradores (Nome,Documento,Rua,Bairro,Cep,Numero,CidadeId,Telefone,Email,SenhaHash)
    VALUES ('Segundo administrador','11111111112','Rua Teste','Centro','85800000','2',1,'45999999998','admin2@teste.local',@hash)
    """, new { hash });
string usuarioApi = "vc_teste_" + Guid.NewGuid().ToString("N")[..20];
string senhaBancoApi = "Teste;\"'\\Á-" + Guid.NewGuid().ToString("N");
string senhaSql = "'" + senhaBancoApi.Replace("\\", "\\\\").Replace("'", "''") + "'";
var sqlUsuario = (await File.ReadAllTextAsync(Path.Combine(raiz, "Banco/03-usuario-api.sql")))
    .Replace("'vandercake_app'", $"'{usuarioApi}'").Replace("vandercake.*", nomeBanco + ".*")
    .Replace("{{SENHA_ESCAPADA}}", senhaSql);
await db.ExecuteAsync(sqlUsuario);
var csApi = new MySqlConnectionStringBuilder(cs.ConnectionString) { UserID = usuarioApi, Password = senhaBancoApi };
await using (var restrita = new MySqlConnection(csApi.ConnectionString))
{
    await restrita.OpenAsync();
    try { await restrita.ExecuteAsync("ALTER TABLE VersoesBanco ADD ColunaProibida INT"); Ok(false, "Conta tecnica nao pode alterar esquema"); }
    catch (MySqlException e) when (e.Number == 1142) { Ok(true, "Conta tecnica conecta mas nao pode alterar a estrutura do banco"); }
}
var bootstrapInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
{
    WorkingDirectory = Path.Combine(raiz, "VanderCake.Api"), RedirectStandardInput = true,
    RedirectStandardOutput = true, RedirectStandardError = true
};
bootstrapInfo.ArgumentList.Add(Path.Combine(raiz, "VanderCake.Api/bin/Debug/net10.0/VanderCake.Api.dll"));
bootstrapInfo.ArgumentList.Add("--criar-admin");
bootstrapInfo.Environment["ConnectionStrings__VanderCake"] = csApi.ConnectionString;
bootstrapInfo.Environment["Seguranca__PastaChaves"] = Path.Combine(Path.GetTempPath(), nomeBanco + "-chaves");
using (var bootstrap = Process.Start(bootstrapInfo)!)
{
    var saida = bootstrap.StandardOutput.ReadToEndAsync(); var erro = bootstrap.StandardError.ReadToEndAsync();
    await bootstrap.StandardInput.WriteAsync(string.Join(Environment.NewLine, new[]
    {
        "Administrador de teste", "11111111111", "Rua Teste", "1", "Centro", "85800000", "1",
        "45999999999", "admin@teste.local", senha, senha, ""
    }));
    bootstrap.StandardInput.Close(); await bootstrap.WaitForExitAsync();
    Ok(bootstrap.ExitCode == 0 && await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Administradores WHERE Email='admin@teste.local'") == 1,
        "Comando --criar-admin cadastra o administrador usando a conta tecnica");
    if (bootstrap.ExitCode != 0) Console.Error.WriteLine(await saida + await erro);
}
var tcp = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
tcp.Start(); int porta = ((IPEndPoint)tcp.LocalEndpoint).Port; tcp.Stop();
var url = $"http://127.0.0.1:{porta}";
var processo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
{
    WorkingDirectory = Path.Combine(raiz, "VanderCake.Api"), RedirectStandardOutput = true, RedirectStandardError = true
};
processo.ArgumentList.Add(Path.Combine(raiz, "VanderCake.Api/bin/Debug/net10.0/VanderCake.Api.dll"));
processo.ArgumentList.Add("--urls"); processo.ArgumentList.Add(url);
processo.Environment["ConnectionStrings__VanderCake"] = csApi.ConnectionString;
processo.Environment["Seguranca__PastaChaves"] = Path.Combine(Path.GetTempPath(), nomeBanco + "-chaves");
processo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
using var apiProcesso = Process.Start(processo)!;
var logs = new ConcurrentQueue<string>();
apiProcesso.OutputDataReceived += (_, e) => { if (e.Data is not null) logs.Enqueue(e.Data); };
apiProcesso.ErrorDataReceived += (_, e) => { if (e.Data is not null) logs.Enqueue(e.Data); };
apiProcesso.BeginOutputReadLine(); apiProcesso.BeginErrorReadLine();
using var http = new HttpClient { BaseAddress = new Uri(url + "/api/"), Timeout = TimeSpan.FromSeconds(20) };
try
{
    bool pronto = false;
    for (int i = 0; i < 100 && !pronto; i++)
    {
        try { pronto = (await http.GetAsync("saude")).IsSuccessStatusCode; } catch (HttpRequestException) { }
        if (!pronto) await Task.Delay(100);
    }
    Ok(pronto, "API inicia e verifica a versão do banco");
    async Task<JsonNode> Chamar(string metodo, string rota, object? corpo = null, string? token = null, int esperado = 200)
    {
        using var req = new HttpRequestMessage(new HttpMethod(metodo), rota);
        if (corpo is not null) req.Content = JsonContent.Create(corpo);
        if (token is not null) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var resp = await http.SendAsync(req);
        string texto = await resp.Content.ReadAsStringAsync();
        if ((int)resp.StatusCode != esperado) throw new Exception($"{metodo} {rota}: esperado {esperado}, recebido {(int)resp.StatusCode}: {texto}");
        return string.IsNullOrWhiteSpace(texto) ? new JsonObject() : JsonNode.Parse(texto)!;
    }
    async Task<JsonNode> Login(string email, bool vendedor = false) => await Chamar("POST", $"auth/{(vendedor ? "vendedores" : "administradores")}/login", new { email, senha });
    string Token(JsonNode s) => s["accessToken"]!.GetValue<string>();
    int Id(JsonNode n) => n["id"]!.GetValue<int>();
    await Chamar("GET", "admin/painel", esperado: 401);
    await Chamar("POST", "auth/administradores/login", new { email = "admin@teste.local", senha = "incorreta" }, esperado: 401);
    Ok(true, "Rotas protegidas e senha incorreta recusadas");
    var login = await Login("admin@teste.local"); var admin = Token(login);
    var admin2 = Token(await Login("admin2@teste.local"));
    var renovada = await Chamar("POST", "auth/renovar", new { refreshToken = login["refreshToken"]!.GetValue<string>() });
    await Chamar("GET", "admin/painel", token: Token(renovada));
    await Chamar("POST", "auth/renovar", new { refreshToken = "invalido" }, esperado: 401);
    Ok(true, "Renovação válida funciona e token inválido é recusado");
    JsonObject Pessoa(string nome, string doc, string email, int vendedorId = 0) => new()
    {
        ["nome"] = nome, ["documento"] = doc, ["rua"] = "Rua das Flores", ["bairro"] = "Centro",
        ["cep"] = "85800000", ["numero"] = "10", ["cidadeId"] = 1, ["telefone"] = "45999999999",
        ["email"] = email, ["senha"] = senha, ["vendedorId"] = vendedorId, ["ativo"] = true
    };
    var v1 = await Chamar("POST", "admin/vendedores", Pessoa("Vendedor Um", "22222222221", "v1@teste.local"), admin);
    var v2 = await Chamar("POST", "admin/vendedores", Pessoa("Vendedor Dois", "22222222222", "v2@teste.local"), admin);
    var vendedor = Token(await Login("v1@teste.local", true));
    var outro = Token(await Login("v2@teste.local", true));
    Ok(v1["ativo"]!.GetValue<bool>() && v1["senhaHash"] is null, "Vendedor nasce ativo e hash não é exposto");
    await Chamar("GET", "admin/painel", token: vendedor, esperado: 403);
    await Chamar("GET", "mobile/sincronizacao", token: admin, esperado: 403);
    Ok(true, "Perfis de vendedor e administrador são separados");
    var invalida = Pessoa("Inválido", "22222222223", "invalido@teste.local"); invalida["cidadeId"] = 999999;
    await Chamar("POST", "admin/vendedores", invalida, admin, 422);
    await Chamar("POST", "admin/vendedores", Pessoa("Repetido", "222.222.222-21", "outro@teste.local"), admin, 409);
    await Chamar("POST", "admin/cidades", new { nome = "Sem estado", estadoId = 999999 }, admin, 422);
    Ok(true, "Cidade inexistente, documento duplicado e cidade sem estado são recusados");
    var estado = await Chamar("POST", "admin/estados", new { nome = "Estado de teste", uf = "ZZ" }, admin);
    var cidade = await Chamar("POST", "admin/cidades", new { nome = "Cidade de teste", estadoId = Id(estado) }, admin);
    Ok(cidade["estadoId"]!.GetValue<int>() == Id(estado), "Nova cidade preserva a chave estrangeira do estado");
    var clientes = new List<int>();
    for (int i = 1; i <= 3; i++) clientes.Add(Id(await Chamar("POST", "admin/clientes", Pessoa("Padaria " + i, "3333333333" + i, $"c{i}@teste.local", Id(v1)), admin)));
    int clienteOutro = Id(await Chamar("POST", "admin/clientes", Pessoa("Outra carteira", "33333333334", "c4@teste.local", Id(v2)), admin));
    var novoMobile = await Chamar("POST", "clientes", Pessoa("Cadastro mobile", "33333333335", "c5@teste.local", Id(v2)), vendedor);
    Ok(novoMobile["vendedorId"]!.GetValue<int>() == Id(v1), "Cliente cadastrado pelo Mobile usa o vendedor do token");
    var catalogo = await Chamar("GET", "admin/painel", token: admin);
    int categoria = Id(catalogo["categorias"]!.AsArray()[0]!);
    async Task<JsonNode> Produto(int estoque, int centavos = 1000) => await Chamar("POST", "admin/produtos", new { nome = "Chocolate de teste", descricao = "Pacote", categoriaId = categoria, precoCentavos = centavos, estoque }, admin);
    var produto = await Produto(11); int produtoId = Id(produto);
    var mobileApi = new ApiService(new HttpClient { BaseAddress = new Uri(url + "/api/") }) { Token = vendedor };
    var sincronizado = await mobileApi.SincronizarAsync();
    Ok(sincronizado.Clientes.All(c => c.VendedorId == Id(v1)) && sincronizado.CondicoesPagamento.Count == 3 && sincronizado.Produtos.Any(p => p.Preco == 10m),
        "Classes e cliente HTTP reais do Mobile desserializam o catálogo e isolam a carteira");
    object Pedido(int clienteId, int quantidade, int pId = 0, decimal preco = 10m, int condicao = 3) => new
    {
        clienteId, dataHoraCriacao = DateTimeOffset.UtcNow, condicaoPagamentoId = condicao,
        itens = new[] { new { produtoId = pId == 0 ? produtoId : pId, quantidadeSolicitada = quantidade, precoUnitario = preco } },
        parcelas = Enumerable.Range(1, condicao == 1 ? 1 : condicao == 2 ? 2 : 3).Select(n => new { numeroParcela = n, formaPagamentoId = n }).ToArray()
    };
    object Lote(params object[] pedidos) => new { identificadorEnvio = Guid.NewGuid().ToString(), dataHoraCriacao = DateTimeOffset.UtcNow, pedidos };
    var corpo = Lote(Pedido(clientes[0], 10), Pedido(clientes[1], 6), Pedido(clientes[2], 4));
    var recebidos = await Task.WhenAll(Chamar("POST", "pedidos-gerais", corpo, vendedor), Chamar("POST", "pedidos-gerais", corpo, vendedor));
    int loteId = Id(recebidos[0]);
    Ok(loteId == Id(recebidos[1]) && await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Pedidos") == 3, "Envios simultâneos com a mesma chave não duplicam pedidos");
    await Chamar("GET", "pedidos-gerais/por-identificador/" + recebidos[0]["identificadorEnvio"]!.GetValue<string>(), token: outro, esperado: 404);
    var divergente = JsonSerializer.SerializeToNode(corpo)!; divergente["pedidos"]![0]!["itens"]![0]!["quantidadeSolicitada"] = 11;
    await Chamar("POST", "pedidos-gerais", divergente, vendedor, 409);
    Ok(true, "Chave repetida com outro conteúdo e consulta por outro vendedor são recusadas");
    var errado = Lote(Pedido(clientes[0], 1, preco: 9m));
    var rejeicao = await Chamar("POST", "pedidos-gerais", errado, vendedor, 409);
    Ok(rejeicao["loteNaoGravado"]!.GetValue<bool>(), "Preço divergente recebe rejeição definitiva sem gravar lote parcial");
    await Chamar("POST", "pedidos-gerais", errado, vendedor, 409);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1), Pedido(clienteOutro, 1)), vendedor, 422);
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM PedidoGeral") == 1, "Lote inválido é atômico e repetição da rejeição permanece segura");
    var previa = await Chamar("GET", $"admin/pedidos-gerais/{loteId}/previa", token: admin);
    var quotas = previa["pedidos"]!.AsArray().Select(p => p!["itens"]![0]!["quantidadeAtendida"]!.GetValue<int>()).ToArray();
    Ok(quotas.SequenceEqual(new[] { 6, 3, 2 }) && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@produtoId", new { produtoId }) == 11,
        "Prévia distribui 11 unidades entre demandas 10/6/4 como 6/3/2 sem baixar estoque");
    var confirmar = new { tokenPrevia = previa["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() };
    await Task.WhenAll(Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", confirmar, admin),
        Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", new { confirmar.tokenPrevia, identificadorOperacao = Guid.NewGuid().ToString() }, admin2));
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Vendas") == 3 && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@produtoId", new { produtoId }) == 0,
        "Duas confirmações simultâneas geram uma venda por pedido e baixam o estoque uma vez");
    var final = (await mobileApi.SincronizarAsync()).PedidosGerais.Single();
    Ok(final.Status == "Processado" && final.Pedidos.All(p => p.Status == "ConfirmadoComCorte") && final.Pedidos.Sum(p => p.ValorTotalAtendido) == 110m,
        "Mobile recebe automaticamente os status, cortes e valores da confirmação");
    Ok(final.Pedidos.All(p => p.Parcelas.Select(x => x.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }) && p.Parcelas.Sum(x => x.ValorFinal) == p.ValorTotalAtendido),
        "Pix, boleto e dinheiro permanecem independentes e parcelas fecham o total após o corte");
    Ok(final.Pedidos.Single(p => p.ClienteId == clientes[2]).Parcelas.Select(p => p.ValorFinal).SequenceEqual(new decimal?[] { 6.66m, 6.66m, 6.68m }),
        "Confirmação conserva centavos e ajusta somente a última parcela");
    var dataLoja = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")).Date;
    Ok(final.Pedidos.All(p => p.Parcelas.All(x => x.DataVencimento?.Date == dataLoja.AddDays(x.DiasAposConfirmacao))), "Vencimentos usam dias corridos após a confirmação na data da loja");
    await Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", confirmar, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{loteId}/cancelamento", new { motivo = "Já confirmado", versao = 2, identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ContasReceber") == 9, "Repetir confirmação não duplica parcelas; venda confirmada não passa pelo cancelamento de pendentes");
    await Chamar("PUT", $"admin/produtos/{produtoId}", new { nome = "Antigo", categoriaId = categoria, precoCentavos = 1000, estoque = 11, versao = 1, ativo = true }, admin, 409);
    Ok(true, "Cadastro antigo não sobrescreve estoque atualizado por venda");
    var pZero = await Produto(0);
    var loteZero = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pZero))), vendedor);
    var prevZero = await Chamar("GET", $"admin/pedidos-gerais/{Id(loteZero)}/previa", token: admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(loteZero)}/confirmacao", new { tokenPrevia = prevZero["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() }, admin);
    Ok((await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(loteZero)).Pedidos.Single().Status == "NaoAtendido" &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Vendas") == 3, "Estoque zero registra não atendimento sem criar cobrança");
    var pNovo = await Produto(5, 3333);
    var cancelavel = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 2, Id(pNovo), 33.33m)), vendedor);
    var cancelamento = new { motivo = "Solicitação do cliente", versao = 1, identificadorOperacao = Guid.NewGuid().ToString() };
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/cancelamento", cancelamento, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/cancelamento", cancelamento, admin);
    Ok((await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(cancelavel)).Status == "Cancelado" &&
        await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(pNovo) }) == 5, "Cancelamento repetido atualiza o Mobile e conserva o estoque");
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/confirmacao", new { tokenPrevia = "qualquer", identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    var stale = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 2, Id(pNovo), 33.33m)), vendedor);
    var prevStale = await Chamar("GET", $"admin/pedidos-gerais/{Id(stale)}/previa", token: admin);
    await Chamar("PUT", $"admin/produtos/{Id(pNovo)}", new { nome = "Atualizado", categoriaId = categoria, precoCentavos = 3333, estoque = 4, versao = 1, ativo = true }, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(stale)}/confirmacao", new { tokenPrevia = prevStale["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    Ok(await db.ExecuteScalarAsync<string>("SELECT Status FROM PedidoGeral WHERE Id=@id", new { id = Id(stale) }) == "AguardandoConfirmacao", "Prévia desatualizada recusa confirmação inteira");
    var concorrente = await Produto(5);
    var a = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 4, Id(concorrente), condicao: 1)), vendedor);
    var b = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[1], 4, Id(concorrente), condicao: 2)), vendedor);
    var pa = await Chamar("GET", $"admin/pedidos-gerais/{Id(a)}/previa", token: admin);
    var pb = await Chamar("GET", $"admin/pedidos-gerais/{Id(b)}/previa", token: admin);
    async Task<int> TentarConfirmar(int id, JsonNode previaAtual)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"admin/pedidos-gerais/{id}/confirmacao")
        {
            Content = JsonContent.Create(new { tokenPrevia = previaAtual["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() })
        };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin);
        using var resp = await http.SendAsync(req);
        return (int)resp.StatusCode;
    }
    var concorrencia = await Task.WhenAll(TentarConfirmar(Id(a), pa), TentarConfirmar(Id(b), pb));
    Ok(concorrencia.Order().SequenceEqual(new[] { 200, 409 }) && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(concorrente) }) == 1,
        "Lotes diferentes disputando estoque não vendem a mesma unidade duas vezes");
    int restanteId = concorrencia[0] == 409 ? Id(a) : Id(b);
    var novaPrevia = await Chamar("GET", $"admin/pedidos-gerais/{restanteId}/previa", token: admin);
    Ok(await TentarConfirmar(restanteId, novaPrevia) == 200 && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(concorrente) }) == 0,
        "Após rever a prévia, o segundo lote recebe somente o saldo restante");
    var centavo = await Produto(1, 1);
    var minimo = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(centavo), 0.01m)), vendedor);
    var pm = await Chamar("GET", $"admin/pedidos-gerais/{Id(minimo)}/previa", token: admin);
    await TentarConfirmar(Id(minimo), pm);
    var parcelaMinima = (await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(minimo)).Pedidos.Single().Parcelas;
    Ok(parcelaMinima.Select(p => p.ValorFinal).SequenceEqual(new decimal?[] { 0m, 0m, 0.01m }) &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ContasReceber WHERE PedidoId=@id", new { id = minimo["pedidos"]![0]!["id"]!.GetValue<int>() }) == 1,
        "Total de um centavo mantém as três escolhas e não cria cobranças de zero");
    // Hub real: eventos posteriores ao marco não podem vazar para outra carteira.
    long marco = await db.ExecuteScalarAsync<long>("SELECT COALESCE(MAX(Id),0) FROM EventosAlteracao");
    var eventosLoja = new ConcurrentBag<JsonElement>(); var eventosV1 = new ConcurrentBag<JsonElement>(); var eventosV2 = new ConcurrentBag<JsonElement>();
    async Task<HubConnection> Hub(string token, ConcurrentBag<JsonElement> eventos)
    {
        var h = new HubConnectionBuilder().WithUrl(url + "/hubs/atualizacoes", o => o.AccessTokenProvider = () => Task.FromResult<string?>(token)).Build();
        h.On("DadosAlterados", (JsonElement e) => eventos.Add(e)); await h.StartAsync(); return h;
    }
    await using var hLoja = await Hub(admin, eventosLoja);
    await using var hV1 = await Hub(vendedor, eventosV1);
    await using var hV2 = await Hub(outro, eventosV2);
    var paraAvisar = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor);
    bool Tem(ConcurrentBag<JsonElement> eventos) => eventos.Any(e => e.GetProperty("id").GetInt64() > marco && e.GetProperty("tipo").GetString() == "Pedidos");
    for (int i = 0; i < 40 && !(Tem(eventosLoja) && Tem(eventosV1)); i++) await Task.Delay(100);
    Ok(Tem(eventosLoja) && Tem(eventosV1) && !Tem(eventosV2), "SignalR real avisa loja e vendedor responsável, sem avisar outra carteira sobre seus pedidos");
    Ok(eventosLoja.All(e => !e.TryGetProperty("cliente", out _) && !e.TryGetProperty("pedidos", out _)), "Avisos contêm somente identificador/tipo/data, sem dados pessoais");
    var caminhoSqlite = Path.Combine(Path.GetTempPath(), nomeBanco + ".sqlite");
    var bancoLocal = new BancoLocal(caminhoSqlite); var seguro = new SessaoMemoria();
    var sistema = new AplicacaoService(bancoLocal, mobileApi, seguro);
    await sistema.InicializarAsync(); await sistema.EntrarAsync("v1@teste.local", senha); await sistema.AtualizarAsync();
    mobileApi.Token = "acesso-expirado-para-teste";
    await sistema.AtualizarAsync();
    Ok(sistema.TemSessao && mobileApi.Token != "acesso-expirado-para-teste" && (await seguro.LerAsync())!.Contains("refreshToken"),
        "Mobile renova acesso recusado, repete a consulta e persiste a nova sessão segura");
    await sistema.AbrirCarrinhoAsync(clientes[0]); await sistema.AdicionarProdutoAsync(clientes[0], Id(pNovo), 1);
    await using var tempoReal = new TempoRealService(sistema, url + "/api/");
    await tempoReal.IniciarAsync();
    await Chamar("POST", $"admin/pedidos-gerais/{Id(paraAvisar)}/cancelamento", new { motivo = "Teste atualização", versao = 1, identificadorOperacao = Guid.NewGuid().ToString() }, admin);
    bool refletiu = false;
    for (int i = 0; i < 60 && !refletiu; i++)
    {
        refletiu = (await sistema.ObterPedidosClienteAsync(clientes[0])).Any(p => p.Pedido.Id == paraAvisar["pedidos"]![0]!["id"]!.GetValue<int>() && p.Pedido.Status == "Cancelado");
        if (!refletiu) await Task.Delay(100);
    }
    Ok(refletiu && (await sistema.ObterCarrinhoAsync(clientes[0])).Itens.Count == 1, "Serviço real do Mobile atualiza o SQLite via SignalR sem apagar o carrinho aberto");
    var conta = await db.QuerySingleAsync<VanderCake.Api.Models.ContaReceber>("SELECT * FROM ContasReceber WHERE PedidoId=@id ORDER BY Id LIMIT 1",
        new { id = final.Pedidos.Single(p => p.ClienteId == clientes[0]).Id });
    await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", esperado: 401);
    await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: vendedor, esperado: 403);
    await Chamar("POST", "admin/contas-receber/2147483647/recebimento", token: admin, esperado: 404);
    Ok(true, "Somente administradores registram recebimento de uma parcela existente");
    long marcoPagamento = await db.ExecuteScalarAsync<long>("SELECT MAX(Id) FROM EventosAlteracao");
    var baixas = await Task.WhenAll(
        Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin),
        Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin2));
    var baixada = await db.QuerySingleAsync<VanderCake.Api.Models.ContaReceber>("SELECT * FROM ContasReceber WHERE Id=@Id", conta);
    Ok(baixas.All(bx => bx["status"]!.GetValue<string>() == "Paga") && baixada.DataPagamento?.Date == dataLoja &&
        baixas.Select(bx => bx["administradorBaixaId"]!.GetValue<int>()).Distinct().Count() == 1 && baixada.Valor == conta.Valor,
        "Dois recebimentos simultâneos mantêm uma baixa integral, uma data e um responsável");
    var repetida = await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin2);
    Ok(repetida["administradorBaixaId"]!.GetValue<int>() == baixada.AdministradorBaixaId &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM EventosAlteracao WHERE Tipo='Pagamentos' AND Id>@marcoPagamento", new { marcoPagamento }) == 1,
        "Repetir recebimento conserva a primeira baixa e não gera outro evento financeiro");
    bool refletiuBaixa = false;
    for (int i = 0; i < 60 && !refletiuBaixa; i++)
    {
        var atualizado = (await sistema.ObterPedidosClienteAsync(clientes[0])).Single(p => p.Pedido.Id == conta.PedidoId).Pedido;
        refletiuBaixa = atualizado.Parcelas.Any(p => p.NumeroParcela == 1 && p.Situacao == "Paga" && p.DataPagamento?.Date == dataLoja && p.FormaPagamentoId == 1);
        if (!refletiuBaixa) await Task.Delay(100);
    }
    Ok(refletiuBaixa && (await sistema.ObterCarrinhoAsync(clientes[0])).Itens.Count == 1,
        "Recebimento chega ao SQLite do Mobile por SignalR com data e meio da parcela, preservando o carrinho");
    Ok(eventosV1.Any(e => e.GetProperty("id").GetInt64() > marcoPagamento && e.GetProperty("tipo").GetString() == "Pagamentos") &&
        !eventosV2.Any(e => e.GetProperty("id").GetInt64() > marcoPagamento && e.GetProperty("tipo").GetString() == "Pagamentos"),
        "Aviso de recebimento é destinado à carteira correta");
    await tempoReal.PararAsync();
    var formaExtra = await Chamar("POST", "admin/formas-pagamento", new { descricao = "Forma de teste", ativo = true }, admin);
    await Chamar("PUT", $"admin/formas-pagamento/{Id(formaExtra)}", new { descricao = "Forma revisada", ativo = false, versao = 1 }, admin);
    await Chamar("PUT", $"admin/formas-pagamento/{Id(formaExtra)}", new { descricao = "Edição antiga", ativo = true, versao = 1 }, admin, 409);
    var condicaoExtra = await Chamar("POST", "admin/condicoes-pagamento", new { descricao = "Condição de teste", dias = new[] { 7, 14 }, ativo = true }, admin);
    await Chamar("PUT", $"admin/condicoes-pagamento/{Id(condicaoExtra)}", new { descricao = "Condição revisada", dias = new[] { 0 }, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "admin/condicoes-pagamento", new { descricao = "Prazo inválido", dias = new[] { 30 }, ativo = true }, admin, 422);
    var cadastros = await mobileApi.SincronizarAsync();
    Ok(cadastros.FormasPagamento.Any(f => f.Id == Id(formaExtra) && !f.Ativo && f.Descricao == "Forma revisada") &&
        cadastros.CondicoesPagamento.Any(c => c.Id == Id(condicaoExtra) && !c.Ativo && c.Parcelas.Single().DiasAposConfirmacao == 0),
        "Condições e formas são cadastradas/alteradas; versão antiga e prazo não previsto são recusados");
    string descricaoForma = await db.ExecuteScalarAsync<string>("SELECT Descricao FROM FormasPagamento WHERE Id=1") ?? "";
    string descricaoCondicao = await db.ExecuteScalarAsync<string>("SELECT Descricao FROM CondicoesPagamento WHERE Id=3") ?? "";
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = "Reescrever histórico", ativo = true, versao = 1 }, admin, 409);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 0 }, ativo = true, versao = 1 }, admin, 409);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 7, 14, 21 }, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor, 422);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 7, 14, 21 }, ativo = true, versao = 2 }, admin);
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = descricaoForma, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor, 422);
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = descricaoForma, ativo = true, versao = 2 }, admin);
    Ok(true, "Cadastros usados preservam descrição/prazo; desativação impede novas escolhas sem alterar o histórico");
    var categoriaInativa = await Chamar("POST", "admin/categorias", new { nome = "Categoria inativa", ativo = false }, admin);
    await Chamar("POST", "admin/produtos", new { nome = "Produto inválido", categoriaId = Id(categoriaInativa), precoCentavos = 100, estoque = 1 }, admin, 422);
    var categoriaAtual = await db.QuerySingleAsync<VanderCake.Api.Models.Categoria>("SELECT * FROM Categorias WHERE Id=@categoria", new { categoria });
    await Chamar("PUT", $"admin/categorias/{categoria}", new { nome = categoriaAtual.Nome, ativo = false, versao = categoriaAtual.Versao }, admin, 409);
    Ok(true, "Categoria inativa não recebe produto e categoria com produtos ativos não pode ser desativada");
    var antes = Pessoa("Vendedor Um", "22222222221", "v1@teste.local"); antes["versao"] = 1; antes["senha"] = "Nova-senha-2026!";
    await Chamar("PUT", $"admin/vendedores/{Id(v1)}", antes, admin);
    await Chamar("GET", "mobile/sincronizacao", token: vendedor, esperado: 401);
    Ok(true, "Alterar senha revoga tokens antigos");
    var vendedor2Atual = (await Chamar("GET", "admin/painel", token: admin))["vendedores"]!.AsArray().Single(v => Id(v!) == Id(v2))!;
    await Chamar("PATCH", $"admin/vendedores/{Id(v2)}/atividade", new { ativo = false, versao = vendedor2Atual["versao"]!.GetValue<long>() }, admin);
    await Chamar("GET", "mobile/sincronizacao", token: outro, esperado: 401);
    await Chamar("POST", "auth/vendedores/login", new { email = "v2@teste.local", senha }, esperado: 401);
    Ok(true, "Desligar vendedor bloqueia sessão aberta e novo login sem apagar a carteira");
    try { await db.ExecuteAsync("INSERT INTO Cidades (Nome,EstadoId) VALUES ('Órfã',999999)"); Ok(false, "FK deve recusar órfão"); }
    catch (MySqlException e) when (e.Number == 1452) { Ok(true, "Chave estrangeira também protege inserções SQL diretas"); }
    try { await db.ExecuteAsync("UPDATE Produtos SET Estoque=-1 WHERE Id=@produtoId", new { produtoId }); Ok(false, "CHECK deve recusar negativo"); }
    catch (MySqlException e) when (e.Number == 3819) { Ok(true, "CHECK do MySQL impede estoque negativo"); }
    if (Environment.GetEnvironmentVariable("VANDERCAKE_TESTAR_WEB_HTTP") == "1")
    {
        var nodeInfo = new ProcessStartInfo("node") { WorkingDirectory = raiz };
        nodeInfo.ArgumentList.Add(Path.Combine(raiz, "TestesIntegracao/web-http.mjs"));
        nodeInfo.Environment["VANDERCAKE_TEST_URL"] = url;
        using var node = Process.Start(nodeInfo)!;
        await node.WaitForExitAsync();
        Ok(node.ExitCode == 0, "Cliente JavaScript real da Web passou pelos testes HTTP/SignalR");
    }
    await bancoLocal.FecharAsync();
    Console.WriteLine($"\n{testes} verificações de integração concluídas. Banco isolado: {nomeBanco}. Usuario isolado: {usuarioApi}");
}
catch
{
    Console.Error.WriteLine(string.Join(Environment.NewLine, logs));
    throw;
}
finally
{
    if (!apiProcesso.HasExited) { apiProcesso.Kill(entireProcessTree: true); await apiProcesso.WaitForExitAsync(); }
    // Banco de testes mantido para inspeção. Nenhum DROP automático.
}

sealed class SessaoMemoria : ISessaoSegura
{
    private string? _json;
    public Task<string?> LerAsync() => Task.FromResult(_json);
    public Task SalvarAsync(string conteudo) { _json = conteudo; return Task.CompletedTask; }
    public Task LimparAsync() { _json = null; return Task.CompletedTask; }
}
````

## TestesIntegracao/TestesIntegracao.csproj

````xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="../VanderCake.Api/VanderCake.Api.csproj" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Models/**/*.cs" Link="Mobile/Models/%(Filename)%(Extension)" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Contracts/**/*.cs" Link="Mobile/Contracts/%(Filename)%(Extension)" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Rules/**/*.cs" Link="Mobile/Rules/%(Filename)%(Extension)" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Data/**/*.cs" Link="Mobile/Data/%(Filename)%(Extension)" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Services/*.cs" Exclude="../VanderCake.Mobile/src/VanderCake.Mobile/Services/SecureStorageSessao.cs" Link="Mobile/Services/%(Filename)%(Extension)" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Helpers/ApiException.cs" Link="Mobile/Helpers/ApiException.cs" />
        <Compile Include="../VanderCake.Mobile/src/VanderCake.Mobile/Helpers/JsonDados.cs" Link="Mobile/Helpers/JsonDados.cs" />
        <PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.14" />
        <PackageReference Include="sqlite-net-pcl" Version="1.11.285" />
    </ItemGroup>
</Project>
````

## TestesIntegracao/web-http.mjs

````javascript
// Executado pelo runner C# com a API real. Stubs somente para armazenamento/eventos do navegador.
import assert from "node:assert/strict";
import { spawn } from "node:child_process";
import { fileURLToPath } from "node:url";
const origemApi = process.env.VANDERCAKE_TEST_URL;
if (!origemApi) throw new Error("Execute pelo TestesIntegracao, com VANDERCAKE_TESTAR_WEB_HTTP=1.");
// Usa tambem o servidor opcional da Web para verificar o proxy HTTP e WebSocket.
const proxy = spawn(process.execPath, ["scripts/serve.mjs", "--port", "0"], {
    cwd: fileURLToPath(new URL("../VanderCake.Web", import.meta.url)),
    env: { ...process.env, VANDERCAKE_API_URL: origemApi }, stdio: ["ignore", "pipe", "inherit"]
});
process.on("exit", () => proxy.kill());
const origem = await new Promise((resolve, reject) => {
    const timer = setTimeout(() => reject(new Error("Servidor Web nao iniciou")), 10000);
    proxy.stdout.on("data", parte => {
        const porta = parte.toString().match(/localhost:(\d+)/)?.[1];
        if (porta) { clearTimeout(timer); resolve(`http://127.0.0.1:${porta}`); }
    });
    proxy.on("error", reject);
});
const eventos = new EventTarget();
globalThis.self = globalThis;
globalThis.window = eventos;
let destino = "";
globalThis.location = { origin: origem, href: origem + "/index.html", replace: url => { destino = url; } };
globalThis.document = { hidden: false, querySelectorAll: () => [], addEventListener: () => {}, removeEventListener: () => {} };
window.document = document;
window.location = location;
// Exercita o bundle de navegador usando fetch/WebSocket nativos do Node, sem instalar outro cliente SignalR.
Object.defineProperty(process, "release", { value: { ...process.release, name: "teste-web" }, configurable: true });
function armazenamento() {
    const itens = new Map();
    return { getItem: k => itens.get(k) ?? null, setItem: (k, v) => itens.set(k, String(v)), removeItem: k => itens.delete(k) };
}
globalThis.sessionStorage = armazenamento();
globalThis.localStorage = armazenamento();
const api = await import("../VanderCake.Web/dist/js/api.js");
const { repositorio } = await import("../VanderCake.Web/dist/js/dados.js");
const { observarTempoReal } = await import("../VanderCake.Web/dist/js/tempo-real.js");
let testes = 0;
function ok(condicao, descricao) { assert.ok(condicao, descricao); testes++; console.log("OK WEB HTTP: " + descricao); }
try {
    const pagina = await fetch(origem);
    ok(pagina.ok && (await pagina.text()).includes("VanderCake"), "Servidor Web entrega a pagina HTML do projeto");
    await api.entrar("admin@teste.local", "Teste-integracao-2026!");
    ok(api.conectado() && api.sessao().refreshToken, "Login guarda acesso e renovacao no contrato esperado");
    const dados = await repositorio.obterDados();
    ok(dados.produtos.length > 0 && Number.isSafeInteger(dados.produtos[0].precoCentavos), "Painel real retorna moeda em centavos");
    const lote = dados.pedidosGerais.find(l => l.status === "AguardandoConfirmacao");
    ok(lote, "Pendentes da API sao encontrados pela Web");
    const detalhes = await repositorio.obterLote(lote.id);
    const previa = await repositorio.obterPrevia(lote.id);
    ok(detalhes.lote.id === lote.id && previa.tokenPrevia && previa.pedidos[0].parcelas.length === 3, "Detalhes e previa usam os campos consumidos pela pagina");
    const sessao = api.sessao();
    sessionStorage.setItem("vandercake.web.api.sessao.v1", JSON.stringify({ ...sessao, token: "expirado" }));
    await repositorio.obterDados();
    ok(api.sessao().token !== "expirado", "401 renova o acesso e repete a consulta");
    let avisos = 0;
    observarTempoReal(() => avisos++);
    async function aguardar(predicado) {
        for (let i = 0; i < 80 && !predicado(); i++) await new Promise(r => setTimeout(r, 100));
        assert.ok(predicado(), "Aviso SignalR nao recebido no prazo de teste");
    }
    await aguardar(() => avisos > 0);
    const anterior = avisos;
    const categoria = await repositorio.salvar("categorias", { nome: "Categoria HTTP " + Date.now() });
    await aguardar(() => avisos > anterior);
    ok((await repositorio.obterDados()).categorias.some(c => c.id === categoria.id), "Cliente SignalR JavaScript recebe aviso e consulta cadastro confirmado");
    let rejeitou = false;
    try { await repositorio.salvar("cidades", { nome: "Invalida", estadoId: -1 }); }
    catch (erro) { rejeitou = erro.status === 422 && erro.message.length > 0; }
    ok(rejeitou, "Mensagem de validacao da API chega ao frontend");
    const condicao = await repositorio.salvar("condicoes-pagamento", { descricao: "Prazo Web", dias: [7, 14], ativo: true });
    const forma = await repositorio.salvar("formas-pagamento", { descricao: "Meio Web", ativo: true });
    await repositorio.salvar("formas-pagamento", { descricao: "Meio Web revisado", ativo: false, versao: 1 }, forma.id);
    const atualizados = await repositorio.obterDados();
    ok(atualizados.condicoesPagamento.some(c => c.id === condicao.id && c.dias.join("/") === "7/14") &&
        atualizados.formasPagamento.some(f => f.id === forma.id && f.descricao === "Meio Web revisado" && !f.ativo && f.versao === 2),
        "Cadastros auxiliares salvam condicoes e formas usando o contrato da Web");
    const conta = atualizados.contasReceber.find(c => c.status === "Pendente");
    const baixa = await repositorio.receberParcela(conta.id);
    const aposBaixa = await repositorio.obterDados();
    const contaPaga = aposBaixa.contasReceber.find(c => c.id === conta.id);
    const pedido = aposBaixa.pedidosGerais.flatMap(l => l.pedidos).find(p => p.id === conta.pedidoId);
    ok(baixa.status === "Paga" && contaPaga.dataPagamento && contaPaga.valorCentavos === conta.valorCentavos &&
        pedido.parcelas.some(p => p.numeroParcela === conta.numeroParcela && p.situacao === "Paga" && p.dataPagamento === contaPaga.dataPagamento),
        "Recebimento da Web atualiza conta e detalhes do pedido conservando o valor");
    const paginasExtras = await Promise.all(["recebimentos.html", "pagamentos.html"].map(async caminho => {
        const resposta = await fetch(origem + "/" + caminho);
        return resposta.ok && (await resposta.text()).includes('type="module"');
    }));
    ok(paginasExtras.every(Boolean), "Paginas de recebimentos e cadastros auxiliares sao servidas com seus modulos");
    api.sair();
    ok(!api.conectado() && destino === "login.html", "Sair remove a sessao e redireciona para login");
    console.log(`${testes} verificacoes do cliente JavaScript concluidas.`);
    window.dispatchEvent(new Event("pagehide"));
    process.exit(0);
} catch (erro) { console.error(erro); process.exit(1); }
````

## VanderCake.Api/Contracts/AtividadeRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class AtividadeRequest
{
    public bool Ativo { get; set; }
    public long Versao { get; set; }
}
````

## VanderCake.Api/Contracts/CancelarRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class CancelarRequest
{
    public string Motivo { get; set; } = "";
    public long Versao { get; set; }
    public string IdentificadorOperacao { get; set; } = "";
}
````

## VanderCake.Api/Contracts/CategoriaRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class CategoriaRequest
{
    public string Nome { get; set; } = "";
    public long Versao { get; set; }
    public bool Ativo { get; set; } = true;
}
````

## VanderCake.Api/Contracts/CidadeRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class CidadeRequest
{
    public string Nome { get; set; } = "";
    public int EstadoId { get; set; }
    public long Versao { get; set; }
}
````

## VanderCake.Api/Contracts/ConfirmarRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class ConfirmarRequest
{
    public string TokenPrevia { get; set; } = "";
    public string IdentificadorOperacao { get; set; } = "";
}
````

## VanderCake.Api/Contracts/EstadoRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class EstadoRequest
{
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
    public long Versao { get; set; }
}
````

## VanderCake.Api/Contracts/ItemPedidoEnvio.cs

````csharp
namespace VanderCake.Api.Contracts;

public class ItemPedidoEnvio
{
    public int ProdutoId
    {
        get; set;
    }
    public int QuantidadeSolicitada
    {
        get; set;
    }
    public decimal PrecoUnitario
    {
        get; set;
    }
}
````

## VanderCake.Api/Contracts/LoginRequisicao.cs

````csharp
namespace VanderCake.Api.Contracts;

public class LoginRequisicao
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}
````

## VanderCake.Api/Contracts/NovoClienteRequisicao.cs

````csharp
namespace VanderCake.Api.Contracts;

public class NovoClienteRequisicao
{
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId
    {
        get; set;
    }
    public string Telefone { get; set; } = "";
    public string? Email
    {
        get; set;
    }
}
````

## VanderCake.Api/Contracts/PagamentoCadastroRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public sealed class PagamentoCadastroRequest
{
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
    public int[] Dias { get; set; } = [];
}
````

## VanderCake.Api/Contracts/ParcelaEnvio.cs

````csharp
namespace VanderCake.Api.Contracts;

public class ParcelaEnvio
{
    public int NumeroParcela
    {
        get; set;
    }
    public int FormaPagamentoId
    {
        get; set;
    }
}
````

## VanderCake.Api/Contracts/PedidoEnvio.cs

````csharp
namespace VanderCake.Api.Contracts;

public class PedidoEnvio
{
    public int ClienteId
    {
        get; set;
    }
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public int CondicaoPagamentoId
    {
        get; set;
    }
    public List<ItemPedidoEnvio> Itens { get; set; } = [];
    public List<ParcelaEnvio> Parcelas { get; set; } = [];
}
````

## VanderCake.Api/Contracts/PedidoGeralEnvio.cs

````csharp
namespace VanderCake.Api.Contracts;

public class PedidoGeralEnvio
{
    public string IdentificadorEnvio { get; set; } = "";
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public List<PedidoEnvio> Pedidos { get; set; } = [];
}
````

## VanderCake.Api/Contracts/PessoaRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class PessoaRequest
{
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId { get; set; }
    public string Telefone { get; set; } = "";
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public int VendedorId { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
}
````

## VanderCake.Api/Contracts/ProdutoRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class ProdutoRequest
{
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public int CategoriaId { get; set; }
    public long PrecoCentavos { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
}
````

## VanderCake.Api/Contracts/RenovarRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class RenovarRequest
{
    public string RefreshToken { get; set; } = "";
}
````

## VanderCake.Api/Contracts/VersaoRequest.cs

````csharp
namespace VanderCake.Api.Contracts;

public class VersaoRequest
{
    public long Versao { get; set; }
}
````

## VanderCake.Api/Controllers/AdminController.cs

````csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin")]
public sealed class AdminController(LeituraService leitura, CadastroService cadastros, PedidosService pedidos, PagamentosService pagamentos) : ControllerBase
{
    private int AdministradorId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("painel")]
    public async Task<object> Painel() => ProjecaoWeb.Painel(await leitura.ConsultarAsync());

    [HttpGet("pedidos-gerais/{id:int}")]
    public async Task<object> Detalhes(int id) => ProjecaoWeb.Detalhes(await leitura.ConsultarAsync(), id);

    [HttpGet("pedidos-gerais/{id:int}/previa")]
    public Task<object> Previa(int id) => pedidos.PreviaAsync(id);

    [HttpPost("pedidos-gerais/{id:int}/confirmacao")]
    public Task<object> Confirmar(int id, ConfirmarRequest r) => pedidos.DecidirAsync(id, AdministradorId, r, null);

    [HttpPost("pedidos-gerais/{id:int}/cancelamento")]
    public Task<object> Cancelar(int id, CancelarRequest r) => pedidos.DecidirAsync(id, AdministradorId, null, r);

    [HttpPost("produtos")]
    public Task<object> Produto(ProdutoRequest r) => cadastros.SalvarProdutoAsync(r, null);
    [HttpPut("produtos/{id:int}")]
    public Task<object> Produto(int id, ProdutoRequest r) => cadastros.SalvarProdutoAsync(r, id);
    [HttpDelete("produtos/{id:int}")]
    public Task<object> Excluir(int id, [FromBody] VersaoRequest r) => cadastros.ExcluirProdutoAsync(id, r.Versao);

    [HttpPost("vendedores")]
    public async Task<IActionResult> Vendedor(PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, null, true));
    [HttpPut("vendedores/{id:int}")]
    public async Task<IActionResult> Vendedor(int id, PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, id, true));
    [HttpPatch("vendedores/{id:int}/atividade")]
    public Task<object> Atividade(int id, AtividadeRequest r) => cadastros.AtividadeAsync(id, r);

    [HttpPost("clientes")]
    public async Task<IActionResult> Cliente(PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, null, false));
    [HttpPut("clientes/{id:int}")]
    public async Task<IActionResult> Cliente(int id, PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, id, false));

    [HttpPost("categorias")]
    public Task<object> Categoria(CategoriaRequest r) => cadastros.SalvarReferenciaAsync("Categorias", r.Nome, null, null, r.Versao, null, r.Ativo);
    [HttpPut("categorias/{id:int}")]
    public Task<object> Categoria(int id, CategoriaRequest r) => cadastros.SalvarReferenciaAsync("Categorias", r.Nome, null, null, r.Versao, id, r.Ativo);

    [HttpPost("condicoes-pagamento")]
    public Task<object> Condicao(PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, null, true);
    [HttpPut("condicoes-pagamento/{id:int}")]
    public Task<object> Condicao(int id, PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, id, true);
    [HttpPost("formas-pagamento")]
    public Task<object> Forma(PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, null, false);
    [HttpPut("formas-pagamento/{id:int}")]
    public Task<object> Forma(int id, PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, id, false);
    [HttpPost("contas-receber/{id:int}/recebimento")]
    public Task<object> Receber(int id) => pagamentos.ReceberAsync(id, AdministradorId);

    [HttpPost("estados")]
    public Task<object> Estado(EstadoRequest r) => cadastros.SalvarReferenciaAsync("Estados", r.Nome, r.Uf, null, r.Versao, null);
    [HttpPut("estados/{id:int}")]
    public Task<object> Estado(int id, EstadoRequest r) => cadastros.SalvarReferenciaAsync("Estados", r.Nome, r.Uf, null, r.Versao, id);

    [HttpPost("cidades")]
    public Task<object> Cidade(CidadeRequest r) => cadastros.SalvarReferenciaAsync("Cidades", r.Nome, null, r.EstadoId, r.Versao, null);
    [HttpPut("cidades/{id:int}")]
    public Task<object> Cidade(int id, CidadeRequest r) => cadastros.SalvarReferenciaAsync("Cidades", r.Nome, null, r.EstadoId, r.Versao, id);
}
````

## VanderCake.Api/Controllers/AuthController.cs

````csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("login")]
public sealed class AuthController(AutenticacaoService auth) : ControllerBase
{
    [HttpPost("administradores/login")]
    public Task<object> Administrador(LoginRequisicao r) => auth.EntrarAsync("Administrador", r.Email, r.Senha);

    [HttpPost("vendedores/login")]
    public Task<object> Vendedor(LoginRequisicao r) => auth.EntrarAsync("Vendedor", r.Email, r.Senha);

    [HttpPost("renovar")]
    public Task<object> Renovar(RenovarRequest r) => auth.RenovarAsync(r.RefreshToken);
}
````

## VanderCake.Api/Controllers/MobileController.cs

````csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendedor")]
[Route("api")]
public sealed class MobileController(LeituraService leitura, CadastroService cadastros, PedidosService pedidos) : ControllerBase
{
    private int VendedorId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mobile/sincronizacao")]
    public async Task<object> Sincronizar()
    {
        var d = await leitura.ConsultarAsync(VendedorId);
        var vendedor = d.Vendedores.Single();
        return new
        {
            Vendedor = new { vendedor.Id, vendedor.Nome, vendedor.Email, vendedor.Ativo },
            d.Clientes, d.Estados, d.Cidades, d.Categorias, d.Produtos,
            d.CondicoesPagamento, d.FormasPagamento, d.PedidosGerais, SincronizadoEm = DateTime.UtcNow
        };
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> Cadastrar(NovoClienteRequisicao r) => Ok(await cadastros.SalvarPessoaAsync(new PessoaRequest
    {
        Nome = r.Nome, Documento = r.Documento, Rua = r.Rua, Bairro = r.Bairro, Cep = r.Cep,
        Numero = r.Numero, CidadeId = r.CidadeId, Telefone = r.Telefone, Email = r.Email
    }, null, false, VendedorId));

    [HttpPost("pedidos-gerais")]
    public async Task<IActionResult> Enviar(PedidoGeralEnvio r) => Ok(await pedidos.ReceberAsync(VendedorId, r));

    [HttpGet("pedidos-gerais/por-identificador/{identificador}")]
    public async Task<IActionResult> Consultar(string identificador) => Ok(await pedidos.PorIdentificadorAsync(VendedorId, identificador));
}
````

## VanderCake.Api/Hubs/AtualizacoesHub.cs

````csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VanderCake.Api.Hubs;

[Authorize(Roles = "Administrador,Vendedor")]
public sealed class AtualizacoesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // O cliente não escolhe grupos: eles são derivados da identidade autenticada.
        string grupo = Context.User!.IsInRole("Administrador")
            ? "loja" : $"vendedor:{Context.User.FindFirstValue(ClaimTypes.NameIdentifier)}";
        await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
        await base.OnConnectedAsync();
    }
}
````

## VanderCake.Api/Infrastructure/ApiException.cs

````csharp
namespace VanderCake.Api.Infrastructure;

public sealed class ApiException(int status, string mensagem, bool loteNaoGravado = false) : Exception(mensagem)
{
    public int Status { get; } = status;
    public bool LoteNaoGravado { get; } = loteNaoGravado;
}
````

## VanderCake.Api/Infrastructure/Banco.cs

````csharp
using MySqlConnector;

namespace VanderCake.Api.Infrastructure;

public sealed class Banco(IConfiguration configuracao)
{
    public async Task<MySqlConnection> AbrirAsync(CancellationToken ct = default)
    {
        var texto = configuracao.GetConnectionString("VanderCake");
        if (string.IsNullOrWhiteSpace(texto))
            throw new ApiException(503, "Configure ConnectionStrings:VanderCake com dotnet user-secrets ou variável de ambiente.");
        // UUIDs são strings nos contratos do Mobile/Web. Evita converter CHAR(36) em Guid implicitamente.
        var opcoes = new MySqlConnectionStringBuilder(texto)
        {
            DateTimeKind = MySqlDateTimeKind.Utc,
            GuidFormat = MySqlGuidFormat.None
        };
        var conexao = new MySqlConnection(opcoes.ConnectionString);
        try
        {
            await conexao.OpenAsync(ct);
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }
}
````

## VanderCake.Api/Infrastructure/Inicializacao.cs

````csharp
using Dapper;
using Microsoft.AspNetCore.Identity;
using VanderCake.Api.Contracts;

namespace VanderCake.Api.Infrastructure;

public static class Inicializacao
{
    public static async Task CriarAdministradorAsync(IServiceProvider servicos)
    {
        Console.WriteLine("Cadastro do administrador da loja. A senha não será exibida.");
        await using var c = await servicos.GetRequiredService<Banco>().AbrirAsync();
        var cidades = await c.QueryAsync("SELECT c.Id, c.Nome, e.Uf FROM Cidades c JOIN Estados e ON e.Id=c.EstadoId ORDER BY e.Uf,c.Nome");
        foreach (var cidade in cidades) Console.WriteLine($"{cidade.Id}: {cidade.Nome}/{cidade.Uf}");
        var r = new PessoaRequest
        {
            Nome = Ler("Nome completo"), Documento = Ler("CPF ou CNPJ (somente números)"),
            Rua = Ler("Rua"), Numero = Ler("Número"), Bairro = Ler("Bairro"),
            Cep = Ler("CEP"), CidadeId = int.Parse(Ler("ID da cidade da lista acima")),
            Telefone = Ler("Telefone com DDD"), Email = Ler("E-mail de acesso"), Senha = LerSenha()
        };
        Console.Write("Repita a senha: ");
        Regras.Exigir(r.Senha == LerSenha(false), "As senhas não conferem.");
        Regras.ValidarPessoa(r, true);
        Regras.ValidarSenha(r.Senha);
        var hash = servicos.GetRequiredService<IPasswordHasher<object>>().HashPassword(new object(), r.Senha!);
        var p = new DynamicParameters(r);
        p.Add("SenhaHash", hash);
        try
        {
            await c.ExecuteAsync("""
                INSERT INTO Administradores (Nome,Documento,Rua,Numero,Bairro,Cep,CidadeId,Telefone,Email,SenhaHash)
                VALUES (@Nome,@Documento,@Rua,@Numero,@Bairro,@Cep,@CidadeId,@Telefone,@Email,@SenhaHash)
                """, p);
            Console.WriteLine("Administrador criado. Inicie a API e use esse e-mail e senha na WEB.");
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062)
        { Console.WriteLine("O e-mail ou documento já está cadastrado. Nenhuma conta foi sobrescrita."); Environment.ExitCode = 1; }
    }

    private static string Ler(string campo) { Console.Write($"{campo}: "); return Console.ReadLine()?.Trim() ?? ""; }
    private static string LerSenha(bool titulo = true)
    {
        if (titulo) Console.Write("Senha (8 a 128 caracteres): ");
        if (Console.IsInputRedirected) return Console.ReadLine() ?? "";
        var texto = new System.Text.StringBuilder();
        while (true)
        {
            var tecla = Console.ReadKey(true);
            if (tecla.Key == ConsoleKey.Enter) break;
            if (tecla.Key == ConsoleKey.Backspace) { if (texto.Length > 0) texto.Length--; }
            else if (!char.IsControl(tecla.KeyChar) && texto.Length < 128) texto.Append(tecla.KeyChar);
        }
        Console.WriteLine();
        return texto.ToString();
    }
}
````

## VanderCake.Api/Infrastructure/Regras.cs

````csharp
using System.Net.Mail;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using VanderCake.Api.Contracts;
using VanderCake.Api.Models;

namespace VanderCake.Api.Infrastructure;

public static class Regras
{
    public const decimal LimiteDinheiro = 9999999999.99m;
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static void Exigir(bool condicao, string mensagem, int status = 422)
    {
        if (!condicao) throw new ApiException(status, mensagem);
    }

    public static string Texto(string? valor, string campo, int maximo, bool opcional = false)
    {
        var texto = (valor ?? "").Trim();
        Exigir((opcional || texto.Length > 0) && texto.Length <= maximo, $"{campo}: preencha com até {maximo} caracteres.");
        return texto;
    }

    public static string Digitos(string? valor) => Regex.Replace(valor ?? "", @"\D", "");
    public static string Uuid(string? valor)
    {
        Exigir(Guid.TryParseExact(valor, "D", out var id) && id != Guid.Empty, "Identificador da operação inválido.");
        return id.ToString("D");
    }
    public static long Centavos(decimal valor) => checked((long)(valor * 100m));
    public static long? Centavos(decimal? valor) => valor.HasValue ? Centavos(valor.Value) : null;
    public static string Hash(object valor) => Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(valor, Json)));
    public static T Copiar<T>(T valor) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(valor, Json), Json)!;

    public static decimal[] Parcelar(decimal total, int quantidade)
    {
        Exigir(total >= 0 && total <= LimiteDinheiro && decimal.Round(total, 2) == total && quantidade is >= 1 and <= 3, "Total ou parcelas inválidos.");
        long centavos = Centavos(total);
        long parte = centavos / quantidade;
        return Enumerable.Range(0, quantidade).Select(i => (i == quantidade - 1 ? centavos - parte * (quantidade - 1) : parte) / 100m).ToArray();
    }

    public static Dictionary<int, int> Ratear(int estoque, IReadOnlyList<(int ClienteId, int Quantidade)> demandas)
    {
        Exigir(estoque >= 0 && demandas.Count > 0 && demandas.All(x => x.Quantidade > 0) && demandas.Select(x => x.ClienteId).Distinct().Count() == demandas.Count, "Demanda ou estoque inválido.");
        var total = demandas.Aggregate(BigInteger.Zero, (s, x) => s + x.Quantidade);
        var saldo = BigInteger.Min(estoque, total);
        var quotas = demandas.Select(x => new { x.ClienteId, Quantidade = (int)(saldo * x.Quantidade / total), Resto = saldo * x.Quantidade % total }).ToList();
        var resultado = quotas.ToDictionary(x => x.ClienteId, x => x.Quantidade);
        int sobra = (int)saldo - resultado.Values.Sum();
        foreach (var quota in quotas.OrderByDescending(x => x.Resto).ThenBy(x => x.ClienteId).Take(sobra))
            resultado[quota.ClienteId]++;
        return resultado;
    }

    public static void ValidarPessoa(PessoaRequest p, bool emailObrigatorio)
    {
        p.Nome = Texto(p.Nome, "Nome", 100);
        p.Documento = Digitos(p.Documento);
        Exigir(p.Documento.Length is 11 or 14, "Informe um CPF ou CNPJ com 11 ou 14 dígitos.");
        p.Rua = Texto(p.Rua, "Rua", 100);
        p.Bairro = Texto(p.Bairro, "Bairro", 60);
        p.Numero = Texto(p.Numero, "Número", 10);
        p.Cep = Digitos(p.Cep);
        Exigir(p.Cep.Length == 8, "O CEP deve conter 8 dígitos.");
        p.Telefone = Digitos(p.Telefone);
        Exigir(p.Telefone.Length is 10 or 11, "Informe telefone com DDD.");
        p.Email = Texto(p.Email, "E-mail", 150, !emailObrigatorio).ToLowerInvariant();
        Exigir((!emailObrigatorio && p.Email.Length == 0) || (MailAddress.TryCreate(p.Email, out var email) && email.Address == p.Email), "E-mail inválido.");
        if (p.Email.Length == 0) p.Email = null;
        Exigir(p.CidadeId > 0, "Selecione uma cidade.");
    }
    public static void ValidarSenha(string? senha) => Exigir(senha is { Length: >= 8 and <= 128 }, "A senha deve ter entre 8 e 128 caracteres.");

    public static PedidoGeralEnvio Canonico(PedidoGeralEnvio e)
    {
        e.IdentificadorEnvio = Uuid(e.IdentificadorEnvio);
        Exigir(e.Pedidos is { Count: > 0 and <= 500 }, "Envie de 1 a 500 pedidos no lote.");
        Exigir(e.Pedidos.All(p => p is not null && p.Itens is { Count: > 0 and <= 500 } && p.Parcelas is { Count: >= 1 and <= 3 }), "Itens ou parcelas ausentes.");
        e.DataHoraCriacao = e.DataHoraCriacao.ToUniversalTime();
        e.Pedidos = e.Pedidos.OrderBy(p => p.ClienteId).ToList();
        foreach (var p in e.Pedidos)
        {
            Exigir(p.Itens.All(i => i is not null) && p.Parcelas.All(i => i is not null), "Itens ou parcelas inválidos.");
            p.DataHoraCriacao = p.DataHoraCriacao.ToUniversalTime();
            p.Itens = p.Itens.OrderBy(i => i.ProdutoId).ToList();
            p.Parcelas = p.Parcelas.OrderBy(i => i.NumeroParcela).ToList();
        }
        return e;
    }
}
````

## VanderCake.Api/Models/Administrador.cs

````csharp
using System.Text.Json.Serialization;
namespace VanderCake.Api.Models;

public class Administrador : Pessoa
{
    [JsonIgnore]
    public string SenhaHash { get; set; } = "";
    [JsonIgnore]
    public int VersaoSeguranca { get; set; } = 1;
}
````

## VanderCake.Api/Models/Categoria.cs

````csharp
namespace VanderCake.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; } = 1;
}
````

## VanderCake.Api/Models/Cidade.cs

````csharp
namespace VanderCake.Api.Models;

public class Cidade
{
    public int Id { get; set; }
    public int EstadoId { get; set; }
    public string Nome { get; set; } = "";
    public long Versao { get; set; } = 1;
}
````

## VanderCake.Api/Models/Cliente.cs

````csharp
namespace VanderCake.Api.Models;
public class Cliente : Pessoa
{
    public int VendedorId { get; set; }
}
````

## VanderCake.Api/Models/CondicaoPagamento.cs

````csharp
namespace VanderCake.Api.Models;

public class CondicaoPagamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public long Versao { get; set; } = 1;
    public bool Ativo { get; set; } = true;
    public List<RegraParcela> Parcelas { get; set; } = [];
}
````

## VanderCake.Api/Models/ContaReceber.cs

````csharp
namespace VanderCake.Api.Models;

public class ContaReceber
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int PedidoId { get; set; }
    public int ParcelaPedidoId { get; set; }
    public int NumeroParcela { get; set; }
    public int FormaPagamentoId { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTime? DataPagamento { get; set; }
    public int? AdministradorBaixaId { get; set; }
}
````

## VanderCake.Api/Models/ControleEnvio.cs

````csharp
namespace VanderCake.Api.Models;

public class ControleEnvio
{
    public string IdentificadorEnvio { get; set; } = "";
    public int VendedorId { get; set; }
    public string ConteudoHash { get; set; } = "";
    public string Status { get; set; } = "";
    public int? PedidoGeralId { get; set; }
    public int? CodigoRejeicao { get; set; }
    public string? MotivoRejeicao { get; set; }
}
````

## VanderCake.Api/Models/DadosLoja.cs

````csharp
namespace VanderCake.Api.Models;

public class DadosLoja
{
    public List<Estado> Estados { get; set; } = [];
    public List<Cidade> Cidades { get; set; } = [];
    public List<Categoria> Categorias { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];
    public List<Vendedor> Vendedores { get; set; } = [];
    public List<Cliente> Clientes { get; set; } = [];
    public List<CondicaoPagamento> CondicoesPagamento { get; set; } = [];
    public List<FormaPagamento> FormasPagamento { get; set; } = [];
    public List<PedidoGeral> PedidosGerais { get; set; } = [];
    public List<Venda> Vendas { get; set; } = [];
    public List<ContaReceber> ContasReceber { get; set; } = [];
}
````

## VanderCake.Api/Models/Estado.cs

````csharp
namespace VanderCake.Api.Models;

public class Estado
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
    public long Versao { get; set; } = 1;
}
````

## VanderCake.Api/Models/EventoAlteracao.cs

````csharp
namespace VanderCake.Api.Models;

public class EventoAlteracao
{
    public long Id { get; set; }
    public string Tipo { get; set; } = "";
    public int? VendedorId { get; set; }
    public DateTime CriadoEm { get; set; }
}
````

## VanderCake.Api/Models/FormaPagamento.cs

````csharp
namespace VanderCake.Api.Models;

public class FormaPagamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public long Versao { get; set; } = 1;
    public bool Ativo { get; set; } = true;
}
````

## VanderCake.Api/Models/ItemPedido.cs

````csharp
namespace VanderCake.Api.Models;

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = "";
    public int QuantidadeSolicitada { get; set; }
    public int? QuantidadeAtendida { get; set; }
    public decimal PrecoUnitario { get; set; }
}
````

## VanderCake.Api/Models/ItemVenda.cs

````csharp
namespace VanderCake.Api.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int PedidoId { get; set; }
    public int ItemPedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = "";
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
````

## VanderCake.Api/Models/Operacao.cs

````csharp
namespace VanderCake.Api.Models;

public class Operacao
{
    public string Identificador { get; set; } = "";
    public int PedidoGeralId { get; set; }
    public int AdministradorId { get; set; }
    public string Acao { get; set; } = "";
    public string ConteudoHash { get; set; } = "";
}
````

## VanderCake.Api/Models/ParcelaPedido.cs

````csharp
namespace VanderCake.Api.Models;

public class ParcelaPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int NumeroParcela { get; set; }
    public int DiasAposConfirmacao { get; set; }
    public int FormaPagamentoId { get; set; }
    public string FormaPagamentoNome { get; set; } = "";
    public decimal ValorPrevisto { get; set; }
    public decimal? ValorFinal { get; set; }
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string Situacao { get; set; } = "Prevista";
}
````

## VanderCake.Api/Models/Pedido.cs

````csharp
namespace VanderCake.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public int PedidoGeralId { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = "";
    public DateTime DataHoraCriacao { get; set; }
    public int CondicaoPagamentoId { get; set; }
    public string CondicaoPagamentoDescricao { get; set; } = "";
    public string Status { get; set; } = "Pendente";
    public decimal ValorTotalSolicitado { get; set; }
    public decimal? ValorTotalAtendido { get; set; }
    public List<ItemPedido> Itens { get; set; } = [];
    public List<ParcelaPedido> Parcelas { get; set; } = [];
}
````

## VanderCake.Api/Models/PedidoGeral.cs

````csharp
namespace VanderCake.Api.Models;

public class PedidoGeral
{
    public int Id { get; set; }
    public string IdentificadorEnvio { get; set; } = "";
    public int VendedorId { get; set; }
    public string VendedorNome { get; set; } = "";
    public DateTime DataHoraCriacao { get; set; }
    public DateTime DataHoraRecebimento { get; set; }
    public DateTime? DataHoraProcessamento { get; set; }
    public int? AdministradorId { get; set; }
    public string? AdministradorNome { get; set; }
    public string Status { get; set; } = "AguardandoConfirmacao";
    public string? MotivoCancelamento { get; set; }
    public long Versao { get; set; } = 1;
    public List<Pedido> Pedidos { get; set; } = [];
}
````

## VanderCake.Api/Models/Pessoa.cs

````csharp
using System.Text.Json.Serialization;

namespace VanderCake.Api.Models;

public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId { get; set; }
    public string CidadeNome { get; set; } = "";
    public string Uf { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Email { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; } = 1;
}
````

## VanderCake.Api/Models/Previa.cs

````csharp
namespace VanderCake.Api.Models;

public class Previa
{
    public string TokenPrevia { get; set; } = "";
    public List<Pedido> Pedidos { get; set; } = [];
    public decimal TotalSolicitado { get; set; }
    public decimal TotalAtendido { get; set; }
    public long UnidadesCortadas { get; set; }
}
````

## VanderCake.Api/Models/Produto.cs

````csharp
namespace VanderCake.Api.Models;

public class Produto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; } = 1;
}
````

## VanderCake.Api/Models/RegraParcela.cs

````csharp
namespace VanderCake.Api.Models;

public class RegraParcela
{
    public int Id { get; set; }
    public int CondicaoPagamentoId { get; set; }
    public int NumeroParcela { get; set; }
    public int DiasAposConfirmacao { get; set; }
}
````

## VanderCake.Api/Models/Venda.cs

````csharp
namespace VanderCake.Api.Models;

public class Venda
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public DateTime DataHoraConfirmacao { get; set; }
    public decimal ValorTotal { get; set; }
    public List<ItemVenda> Itens { get; set; } = [];
}
````

## VanderCake.Api/Models/Vendedor.cs

````csharp
using System.Text.Json.Serialization;
namespace VanderCake.Api.Models;

public class Vendedor : Pessoa
{
    [JsonIgnore]
    public string SenhaHash { get; set; } = "";
    [JsonIgnore]
    public int VersaoSeguranca { get; set; } = 1;
}
````

## VanderCake.Api/Program.cs

````csharp
using System.Text.Json;
using System.Threading.RateLimiting;
using Dapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using MySqlConnector;
using VanderCake.Api.Hubs;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Services;

var builder = WebApplication.CreateBuilder(args.Where(a => a != "--criar-admin").ToArray());
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddSingleton<Banco>();
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<CadastroService>();
builder.Services.AddScoped<LeituraService>();
builder.Services.AddScoped<RateioService>();
builder.Services.AddScoped<PedidosService>();
builder.Services.AddScoped<PagamentosService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<EventosService>();
var pastaChaves = builder.Configuration["Seguranca:PastaChaves"];
if (string.IsNullOrWhiteSpace(pastaChaves))
    pastaChaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VanderCake", "ApiKeys");
Directory.CreateDirectory(pastaChaves);
var protecao = builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(pastaChaves)).SetApplicationName("VanderCake.Api.v1");
if (OperatingSystem.IsWindows()) protecao.ProtectKeysWithDpapi();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme).AddBearerToken(o =>
{
    o.BearerTokenExpiration = TimeSpan.FromHours(builder.Configuration.GetValue("Seguranca:HorasAcesso", 8));
    o.RefreshTokenExpiration = TimeSpan.FromDays(builder.Configuration.GetValue("Seguranca:DiasRenovacao", 30));
});
builder.Services.AddAuthorization();
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddRateLimiter(o =>
{
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(ctx.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    o.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        await ctx.HttpContext.Response.WriteAsJsonAsync(new { mensagem = "Muitas tentativas. Aguarde um minuto e tente novamente." }, ct);
    };
});

var app = builder.Build();
if (args.Contains("--criar-admin"))
{
    using var escopo = app.Services.CreateScope();
    try { await Inicializacao.CriarAdministradorAsync(escopo.ServiceProvider); }
    catch (Exception ex) when (ex is ApiException or FormatException or MySqlException)
    {
        Console.Error.WriteLine(ex is MySqlException ? "Confira a conexão, as tabelas e a cidade informada. Nenhuma conta existente foi alterada." : ex.Message);
        Environment.ExitCode = 1;
    }
    return;
}

app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (Exception ex) when (!ctx.Response.HasStarted)
    {
        int status = 500;
        string mensagem = "Não foi possível concluir a operação. Tente novamente.";
        bool loteNaoGravado = false;
        if (ex is ApiException regra) { status = regra.Status; mensagem = regra.Message; loteNaoGravado = regra.LoteNaoGravado; }
        else if (ex is MySqlException sql)
        {
            (status, mensagem) = sql.Number switch
            {
                1062 => (409, "Já existe um registro com esses dados. Atualize a lista antes de repetir."),
                1451 => (409, "Este registro está vinculado a outros dados e não pode ser excluído."),
                1452 => (422, "Um dos registros vinculados não existe. Atualize os cadastros."),
                1205 or 1213 => (409, "Outra operação alterou os mesmos dados. Atualize e tente novamente."),
                3819 or 1406 or 1048 or 1264 => (422, "Um dos valores não atende às regras do cadastro."),
                _ => (503, "O banco não está disponível ou sua estrutura precisa ser verificada.")
            };
        }
        if (status >= 500) app.Logger.LogError("Falha na operação {Metodo} {Caminho}. Tipo: {Tipo}", ctx.Request.Method, ctx.Request.Path, ex.GetType().Name);
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(new { mensagem, loteNaoGravado });
    }
});
app.UseRouting();
app.UseCors();
app.UseStatusCodePages(async estado =>
{
    var ctx = estado.HttpContext;
    if (!ctx.Request.Path.StartsWithSegments("/api")) return;
    var mensagem = ctx.Response.StatusCode switch
    {
        401 => "Entre novamente para continuar.",
        403 => "Seu perfil não permite esta operação.",
        404 => "Recurso não encontrado.",
        _ => "A operação não pôde ser concluída."
    };
    await ctx.Response.WriteAsJsonAsync(new { mensagem });
});
// O transporte WebSocket/SSE usa o token na consulta. Nunca registrar a query string.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/hubs/atualizacoes") && ctx.Request.Query.TryGetValue("access_token", out var token))
        ctx.Request.Headers.Authorization = $"Bearer {token}";
    await next();
});
app.UseAuthentication();
app.Use(async (ctx, next) =>
{
    if (ctx.User.Identity?.IsAuthenticated == true &&
        !await ctx.RequestServices.GetRequiredService<AutenticacaoService>().VigenteAsync(ctx.User))
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsJsonAsync(new { mensagem = "Conta inativa ou sessão revogada. Entre novamente." });
        return;
    }
    await next();
});
app.UseAuthorization();
app.UseRateLimiter();
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.XContentTypeOptions = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    if (ctx.Request.Path.StartsWithSegments("/api")) ctx.Response.Headers.CacheControl = "no-store";
    await next();
});
var arquivosWeb = Path.Combine(AppContext.BaseDirectory, "wwwroot");
if (Directory.Exists(arquivosWeb))
{
    var provedor = new PhysicalFileProvider(arquivosWeb);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = provedor });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = provedor });
}
app.MapControllers();
app.MapHub<AtualizacoesHub>("/hubs/atualizacoes", o => o.CloseOnAuthenticationExpiration = true);
app.MapGet("/api/saude", async (Banco banco) =>
{
    await using var c = await banco.AbrirAsync();
    var versao = await c.ExecuteScalarAsync<int>("SELECT MAX(Versao) FROM VersoesBanco");
    Regras.Exigir(versao == 1, "A versão do banco não é compatível com esta API.", 503);
    return Results.Ok(new { status = "Disponível", versaoBanco = versao });
});
await app.RunAsync();
````

## VanderCake.Api/Properties/launchSettings.json

````json
{
    "profiles": {
        "VanderCake.Api": {
            "commandName": "Project",
            "launchBrowser": false,
            "applicationUrl": "http://localhost:5080",
            "environmentVariables": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    }
}
````

## VanderCake.Api/Services/AutenticacaoService.cs

````csharp
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class AutenticacaoService(Banco banco, IPasswordHasher<object> senhas,
    IOptionsMonitor<BearerTokenOptions> opcoes)
{
    private static string Tabela(string perfil) => perfil switch
    {
        "Administrador" => "Administradores",
        "Vendedor" => "Vendedores",
        _ => throw new ApiException(401, "Sessão inválida.")
    };

    public async Task<object> EntrarAsync(string perfil, string? email, string? senha)
    {
        email = Regras.Texto(email, "E-mail", 150).ToLowerInvariant();
        Regras.Exigir(senha is { Length: > 0 and <= 128 }, "E-mail ou senha inválidos.", 401);
        await using var c = await banco.AbrirAsync();
        var conta = await c.QuerySingleOrDefaultAsync<Vendedor>($"SELECT * FROM {Tabela(perfil)} WHERE Email=@email", new { email });
        Regras.Exigir(conta is not null && conta.Ativo && senhas.VerifyHashedPassword(new object(), conta.SenhaHash, senha!) != PasswordVerificationResult.Failed,
            "E-mail ou senha inválidos, ou conta inativa.", 401);
        return Emitir(conta!, perfil);
    }

    public async Task<bool> VigenteAsync(ClaimsPrincipal principal)
    {
        var perfil = principal.FindFirstValue(ClaimTypes.Role);
        if (perfil is not ("Administrador" or "Vendedor") ||
            !int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ||
            !int.TryParse(principal.FindFirstValue("versaoSeguranca"), out int versao)) return false;
        await using var c = await banco.AbrirAsync();
        return await c.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Tabela(perfil)} WHERE Id=@id AND Ativo=1 AND VersaoSeguranca=@versao", new { id, versao }) == 1;
    }

    public async Task<object> RenovarAsync(string refreshToken)
    {
        Regras.Exigir(!string.IsNullOrWhiteSpace(refreshToken) && refreshToken.Length <= 10000, "Sessão inválida.", 401);
        AuthenticationTicket? ticket = null;
        try { ticket = opcoes.Get(BearerTokenDefaults.AuthenticationScheme).RefreshTokenProtector.Unprotect(refreshToken); }
        catch (Exception ex) when (ex is System.Security.Cryptography.CryptographicException or ArgumentException or FormatException) { }
        Regras.Exigir(ticket is not null && ticket.Properties.ExpiresUtc > DateTimeOffset.UtcNow && await VigenteAsync(ticket.Principal),
            "A sessão não pode ser renovada. Entre novamente.", 401);
        string perfil = ticket!.Principal.FindFirstValue(ClaimTypes.Role)!;
        int id = int.Parse(ticket.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await using var c = await banco.AbrirAsync();
        var conta = await c.QuerySingleAsync<Vendedor>($"SELECT * FROM {Tabela(perfil)} WHERE Id=@id", new { id });
        return Emitir(conta, perfil);
    }

    private object Emitir(Vendedor conta, string perfil)
    {
        var config = opcoes.Get(BearerTokenDefaults.AuthenticationScheme);
        var agora = DateTimeOffset.UtcNow;
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, conta.Id.ToString()),
            new Claim(ClaimTypes.Name, conta.Nome),
            new Claim(ClaimTypes.Role, perfil),
            new Claim("versaoSeguranca", conta.VersaoSeguranca.ToString())
        }, BearerTokenDefaults.AuthenticationScheme));
        var acesso = new AuthenticationTicket(principal, new AuthenticationProperties { ExpiresUtc = agora + config.BearerTokenExpiration }, BearerTokenDefaults.AuthenticationScheme);
        var renovacao = new AuthenticationTicket(principal, new AuthenticationProperties { ExpiresUtc = agora + config.RefreshTokenExpiration }, BearerTokenDefaults.AuthenticationScheme);
        string accessToken = config.BearerTokenProtector.Protect(acesso);
        string refreshToken = config.RefreshTokenProtector.Protect(renovacao);
        var expiresIn = (int)config.BearerTokenExpiration.TotalSeconds;
        var pessoa = new { conta.Id, conta.Nome, conta.Email, conta.Ativo };
        if (perfil == "Administrador") return new { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresIn = expiresIn, Administrador = pessoa };
        return new { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresIn = expiresIn, Vendedor = pessoa };
    }
}
````

## VanderCake.Api/Services/CadastroService.cs

````csharp
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class CadastroService(Banco banco, IPasswordHasher<object> senhas)
{
    public async Task<object> SalvarProdutoAsync(ProdutoRequest r, int? id)
    {
        r.Nome = Regras.Texto(r.Nome, "Nome do produto", 100);
        r.Descricao = Regras.Texto(r.Descricao, "Descrição", 4000, true);
        Regras.Exigir(r.PrecoCentavos > 0 && r.PrecoCentavos <= 999999999999L && r.Estoque >= 0, "Preço ou estoque inválido.");
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var anterior = await c.QuerySingleOrDefaultAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id FOR UPDATE", new { id }, tx)
                ?? throw new ApiException(404, "Produto não encontrado.");
            Regras.Exigir(anterior.Versao == r.Versao, "O produto ou estoque mudou. Reabra o cadastro.", 409);
        }
        Regras.Exigir((await c.ExecuteScalarAsync<int?>("SELECT Id FROM Categorias WHERE Id=@CategoriaId AND Ativo=1 FOR SHARE", r, tx)).HasValue, "Selecione uma categoria ativa.");
        var parametros = new { id, r.Nome, r.Descricao, r.CategoriaId, Preco = r.PrecoCentavos / 100m, r.Estoque, r.Ativo };
        if (id is null)
            id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO Produtos (Nome, Descricao, CategoriaId, Preco, Estoque, Ativo)
                VALUES (@Nome,@Descricao,@CategoriaId,@Preco,@Estoque,1); SELECT LAST_INSERT_ID();
                """, parametros, tx);
        else
            await c.ExecuteAsync("""
                UPDATE Produtos SET Nome=@Nome, Descricao=@Descricao, CategoriaId=@CategoriaId,
                    Preco=@Preco, Estoque=@Estoque, Ativo=@Ativo, Versao=Versao+1 WHERE Id=@id
                """, parametros, tx);
        var produto = await c.QuerySingleAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id", new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return ProjecaoWeb.Produto(produto);
    }

    public async Task<Pessoa> SalvarPessoaAsync(PessoaRequest r, int? id, bool vendedor, int? autorMobile = null)
    {
        Regras.ValidarPessoa(r, vendedor);
        if (vendedor && (id is null || !string.IsNullOrEmpty(r.Senha))) Regras.ValidarSenha(r.Senha);
        if (autorMobile.HasValue) r.VendedorId = autorMobile.Value;
        string tabela = vendedor ? "Vendedores" : "Clientes";
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        Pessoa? anterior = null;
        Cliente? clienteAnterior = null;
        if (id.HasValue)
        {
            if (vendedor) anterior = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR UPDATE", new { id }, tx);
            else anterior = clienteAnterior = await c.QuerySingleOrDefaultAsync<Cliente>("SELECT * FROM Clientes WHERE Id=@id FOR UPDATE", new { id }, tx);
            Regras.Exigir(anterior is not null, "Cadastro não encontrado.", 404);
            Regras.Exigir(anterior!.Versao == r.Versao, "O cadastro mudou. Reabra-o antes de alterar.", 409);
        }
        Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Cidades WHERE Id=@CidadeId", r, tx) == 1, "Selecione uma cidade cadastrada.");
        if (!vendedor)
        {
            var responsavel = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR SHARE", new { id = r.VendedorId }, tx);
            Regras.Exigir(responsavel is not null && (responsavel.Ativo || clienteAnterior?.VendedorId == r.VendedorId), "Selecione um vendedor ativo.");
        }
        var p = new DynamicParameters(r);
        p.Add("id", id);
        if (vendedor && !string.IsNullOrEmpty(r.Senha)) p.Add("SenhaHash", senhas.HashPassword(new object(), r.Senha));
        if (id is null)
        {
            var extra = vendedor ? "SenhaHash" : "VendedorId";
            id = await c.ExecuteScalarAsync<int>($"""
                INSERT INTO {tabela} (Nome,Documento,Rua,Bairro,Cep,Numero,CidadeId,Telefone,Email,Ativo,{extra})
                VALUES (@Nome,@Documento,@Rua,@Bairro,@Cep,@Numero,@CidadeId,@Telefone,@Email,1,@{extra});
                SELECT LAST_INSERT_ID();
                """, p, tx);
        }
        else
        {
            string extra = vendedor
                ? string.IsNullOrEmpty(r.Senha) ? "" : ", SenhaHash=@SenhaHash, VersaoSeguranca=VersaoSeguranca+1"
                : ", VendedorId=@VendedorId, Ativo=@Ativo";
            await c.ExecuteAsync($"""
                UPDATE {tabela} SET Nome=@Nome, Documento=@Documento, Rua=@Rua, Bairro=@Bairro,
                    Cep=@Cep, Numero=@Numero, CidadeId=@CidadeId, Telefone=@Telefone, Email=@Email,
                    Versao=Versao+1 {extra} WHERE Id=@id
                """, p, tx);
        }
        Pessoa resultado;
        if (vendedor)
            resultado = await c.QuerySingleAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id", new { id }, tx);
        else
            resultado = await c.QuerySingleAsync<Cliente>("""
                SELECT cl.*, ci.Nome AS CidadeNome, e.Uf FROM Clientes cl
                JOIN Cidades ci ON ci.Id=cl.CidadeId JOIN Estados e ON e.Id=ci.EstadoId WHERE cl.Id=@id
                """, new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, vendedor ? "Vendedores" : "Clientes", vendedor ? id : r.VendedorId);
        if (clienteAnterior is not null && clienteAnterior.VendedorId != r.VendedorId)
            await EventosService.RegistrarAsync(c, tx, "Clientes", clienteAnterior.VendedorId);
        await tx.CommitAsync();
        return resultado;
    }

    public async Task<object> AtividadeAsync(int id, AtividadeRequest r)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var vendedor = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Vendedor não encontrado.");
        Regras.Exigir(vendedor.Versao == r.Versao, "O vendedor mudou. Atualize a lista.", 409);
        await c.ExecuteAsync("UPDATE Vendedores SET Ativo=@Ativo, Versao=Versao+1, VersaoSeguranca=VersaoSeguranca+1 WHERE Id=@id", new { id, r.Ativo }, tx);
        await EventosService.RegistrarAsync(c, tx, "Vendedores", id);
        await tx.CommitAsync();
        return new { Id = id, r.Ativo };
    }

    public async Task<object> ExcluirProdutoAsync(int id, long versao)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var produto = await c.QuerySingleOrDefaultAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Produto não encontrado.");
        Regras.Exigir(produto.Versao == versao, "O produto mudou. Atualize a lista.", 409);
        bool usado = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ItemPedido WHERE ProdutoId=@id", new { id }, tx) > 0;
        await c.ExecuteAsync(usado ? "UPDATE Produtos SET Ativo=0,Versao=Versao+1 WHERE Id=@id" : "DELETE FROM Produtos WHERE Id=@id", new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return new { Desativado = usado };
    }

    public async Task<object> SalvarReferenciaAsync(string tipo, string nome, string? uf, int? estadoId, long versao, int? id, bool ativo = true)
    {
        Regras.Exigir(tipo is "Categorias" or "Estados" or "Cidades", "Cadastro inválido.");
        nome = Regras.Texto(nome, "Nome", tipo == "Categorias" ? 60 : 100);
        if (tipo == "Estados")
        {
            uf = Regras.Texto(uf, "UF", 2).ToUpperInvariant();
            Regras.Exigir(System.Text.RegularExpressions.Regex.IsMatch(uf, "^[A-Z]{2}$"), "Informe uma UF com duas letras.");
        }
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var atual = await c.QuerySingleOrDefaultAsync<long?>($"SELECT Versao FROM {tipo} WHERE Id=@id FOR UPDATE", new { id }, tx);
            Regras.Exigir(atual.HasValue, "Cadastro não encontrado.", 404);
            Regras.Exigir(atual == versao, "O cadastro mudou. Atualize-o.", 409);
        }
        if (tipo == "Cidades")
            Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Estados WHERE Id=@estadoId", new { estadoId }, tx) == 1, "Selecione um estado cadastrado.");
        if (tipo == "Categorias" && !ativo && id.HasValue)
            Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Produtos WHERE CategoriaId=@id AND Ativo=1", new { id }, tx) == 0,
                "Desative ou mova os produtos ativos desta categoria antes de desativá-la.", 409);
        var p = new { id, nome, uf, estadoId, ativo };
        if (id is null)
        {
            string colunas = tipo switch { "Estados" => "Nome,Uf", "Cidades" => "Nome,EstadoId", _ => "Nome,Ativo" };
            string valores = tipo switch { "Estados" => "@nome,@uf", "Cidades" => "@nome,@estadoId", _ => "@nome,@ativo" };
            id = await c.ExecuteScalarAsync<int>($"INSERT INTO {tipo} ({colunas}) VALUES ({valores}); SELECT LAST_INSERT_ID();", p, tx);
        }
        else
        {
            string extra = tipo switch { "Estados" => ",Uf=@uf", "Cidades" => ",EstadoId=@estadoId", _ => ",Ativo=@ativo" };
            await c.ExecuteAsync($"UPDATE {tipo} SET Nome=@nome,Versao=Versao+1{extra} WHERE Id=@id", p, tx);
        }
        object resultado = tipo switch
        {
            "Estados" => await c.QuerySingleAsync<Estado>("SELECT * FROM Estados WHERE Id=@id", new { id }, tx),
            "Cidades" => await c.QuerySingleAsync<Cidade>("SELECT * FROM Cidades WHERE Id=@id", new { id }, tx),
            _ => await c.QuerySingleAsync<Categoria>("SELECT * FROM Categorias WHERE Id=@id", new { id }, tx)
        };
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return resultado;
    }
}
````

## VanderCake.Api/Services/EventosService.cs

````csharp
using Dapper;
using Microsoft.AspNetCore.SignalR;
using MySqlConnector;
using VanderCake.Api.Hubs;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class EventosService(Banco banco, IHubContext<AtualizacoesHub> hub, ILogger<EventosService> logger) : BackgroundService
{
    public static Task RegistrarAsync(MySqlConnection c, MySqlTransaction tx, string tipo, int? vendedorId = null) =>
        c.ExecuteAsync("INSERT INTO EventosAlteracao (Tipo,VendedorId,CriadoEm) VALUES (@tipo,@vendedorId,UTC_TIMESTAMP(6))", new { tipo, vendedorId }, tx);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var c = await banco.AbrirAsync(stoppingToken);
                var eventos = await c.QueryAsync<EventoAlteracao>("SELECT * FROM EventosAlteracao WHERE PublicadoEm IS NULL ORDER BY Id LIMIT 100");
                foreach (var evento in eventos)
                {
                    var aviso = new { evento.Id, evento.Tipo, evento.CriadoEm };
                    if (evento.VendedorId.HasValue)
                        await hub.Clients.Groups("loja", $"vendedor:{evento.VendedorId}").SendAsync("DadosAlterados", aviso, stoppingToken);
                    else
                        await hub.Clients.All.SendAsync("DadosAlterados", aviso, stoppingToken);
                    // Se o processo cair antes desta marca, o aviso pode repetir. Os clientes recarregam dados idempotentemente.
                    await c.ExecuteAsync("UPDATE EventosAlteracao SET PublicadoEm=UTC_TIMESTAMP(6) WHERE Id=@Id", evento);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogWarning("A publicação de atualizações será repetida. Tipo da falha: {Tipo}", ex.GetType().Name);
                try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
````

## VanderCake.Api/Services/LeituraService.cs

````csharp
using System.Data;
using Dapper;
using MySqlConnector;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class LeituraService(Banco banco)
{
    public async Task<DadosLoja> ConsultarAsync(int? vendedorId = null)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var dados = await LerAsync(c, tx, vendedorId);
        await tx.CommitAsync();
        return dados;
    }

    public static async Task<DadosLoja> LerAsync(MySqlConnection c, MySqlTransaction tx, int? vendedorId = null)
    {
        var dados = new DadosLoja
        {
            Estados = (await c.QueryAsync<Estado>("SELECT * FROM Estados ORDER BY Nome", transaction: tx)).AsList(),
            Cidades = (await c.QueryAsync<Cidade>("SELECT * FROM Cidades ORDER BY Nome", transaction: tx)).AsList(),
            Categorias = (await c.QueryAsync<Categoria>("SELECT * FROM Categorias ORDER BY Nome", transaction: tx)).AsList(),
            Produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos ORDER BY Id", transaction: tx)).AsList(),
            Vendedores = (await c.QueryAsync<Vendedor>("SELECT * FROM Vendedores WHERE @vendedorId IS NULL OR Id = @vendedorId", new { vendedorId }, tx)).AsList(),
            Clientes = (await c.QueryAsync<Cliente>("""
                SELECT cl.*, ci.Nome AS CidadeNome, e.Uf
                FROM Clientes cl JOIN Cidades ci ON ci.Id = cl.CidadeId JOIN Estados e ON e.Id = ci.EstadoId
                WHERE @vendedorId IS NULL OR cl.VendedorId = @vendedorId ORDER BY cl.Id
                """, new { vendedorId }, tx)).AsList(),
            CondicoesPagamento = (await c.QueryAsync<CondicaoPagamento>("SELECT * FROM CondicoesPagamento ORDER BY Id", transaction: tx)).AsList(),
            FormasPagamento = (await c.QueryAsync<FormaPagamento>("SELECT * FROM FormasPagamento ORDER BY Id", transaction: tx)).AsList(),
            PedidosGerais = (await c.QueryAsync<PedidoGeral>("""
                SELECT pg.*, v.Nome AS VendedorNome, a.Nome AS AdministradorNome
                FROM PedidoGeral pg JOIN Vendedores v ON v.Id = pg.VendedorId
                LEFT JOIN Administradores a ON a.Id = pg.AdministradorId
                WHERE @vendedorId IS NULL OR pg.VendedorId = @vendedorId ORDER BY pg.Id
                """, new { vendedorId }, tx)).AsList()
        };
        var regras = (await c.QueryAsync<RegraParcela>("SELECT * FROM ParcelasCondicaoPagamento ORDER BY NumeroParcela", transaction: tx)).AsList();
        foreach (var condicao in dados.CondicoesPagamento)
            condicao.Parcelas = regras.Where(p => p.CondicaoPagamentoId == condicao.Id).ToList();
        if (dados.PedidosGerais.Count == 0) return dados;
        var lotes = dados.PedidosGerais.Select(x => x.Id).ToArray();
        var pedidos = (await c.QueryAsync<Pedido>("""
            SELECT p.*, cl.Nome AS ClienteNome, cp.Descricao AS CondicaoPagamentoDescricao
            FROM Pedidos p JOIN Clientes cl ON cl.Id = p.ClienteId
            JOIN CondicoesPagamento cp ON cp.Id = p.CondicaoPagamentoId
            WHERE p.PedidoGeralId IN @lotes ORDER BY p.Id
            """, new { lotes }, tx)).AsList();
        if (pedidos.Count == 0) return dados;
        var ids = pedidos.Select(x => x.Id).ToArray();
        var itens = (await c.QueryAsync<ItemPedido>("SELECT * FROM ItemPedido WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        var parcelas = (await c.QueryAsync<ParcelaPedido>("""
            SELECT pp.*, f.Descricao AS FormaPagamentoNome, cr.Valor AS ValorFinal, cr.DataVencimento, cr.DataPagamento,
                   COALESCE(cr.Status, 'Prevista') AS Situacao
            FROM ParcelasPedido pp JOIN FormasPagamento f ON f.Id = pp.FormaPagamentoId
            LEFT JOIN ContasReceber cr ON cr.ParcelaPedidoId = pp.Id
            WHERE pp.PedidoId IN @ids ORDER BY pp.PedidoId, pp.NumeroParcela
            """, new { ids }, tx)).AsList();
        dados.Vendas = (await c.QueryAsync<Venda>("SELECT * FROM Vendas WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        dados.ContasReceber = (await c.QueryAsync<ContaReceber>("""
            SELECT cr.*, pp.NumeroParcela, pp.FormaPagamentoId FROM ContasReceber cr
            JOIN ParcelasPedido pp ON pp.Id = cr.ParcelaPedidoId
            WHERE cr.PedidoId IN @ids ORDER BY cr.Id
            """, new { ids }, tx)).AsList();
        var vendidos = (await c.QueryAsync<ItemVenda>("SELECT * FROM ItemVenda WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        foreach (var venda in dados.Vendas) venda.Itens = vendidos.Where(i => i.VendaId == venda.Id).ToList();
        foreach (var p in pedidos)
        {
            p.Itens = itens.Where(i => i.PedidoId == p.Id).ToList();
            p.Parcelas = parcelas.Where(i => i.PedidoId == p.Id).ToList();
            if (p.Status is "Confirmado" or "ConfirmadoComCorte" or "NaoAtendido")
            {
                foreach (var parcela in p.Parcelas.Where(x => x.ValorFinal is null))
                {
                    parcela.ValorFinal = 0;
                    parcela.Situacao = "SemCobranca";
                }
            }
            if (p.Status == "Cancelado")
                foreach (var parcela in p.Parcelas) parcela.Situacao = "Cancelada";
        }
        foreach (var lote in dados.PedidosGerais) lote.Pedidos = pedidos.Where(p => p.PedidoGeralId == lote.Id).ToList();
        return dados;
    }
}
````

## VanderCake.Api/Services/PagamentosService.cs

````csharp
using System.Data;
using Dapper;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class PagamentosService(Banco banco, IConfiguration configuracao)
{
    public async Task<object> SalvarAsync(PagamentoCadastroRequest r, int? id, bool condicao)
    {
        r.Descricao = Regras.Texto(r.Descricao, "Descrição", condicao ? 60 : 50);
        if (condicao)
            Regras.Exigir(r.Dias is not null && new[] { "0", "7/14", "7/14/21" }.Contains(string.Join('/', r.Dias)),
                "Selecione à vista, 7/14 ou 7/14/21 dias.");
        string tabela = condicao ? "CondicoesPagamento" : "FormasPagamento";
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var anterior = await c.QuerySingleOrDefaultAsync<FormaPagamento>($"SELECT * FROM {tabela} WHERE Id=@id FOR UPDATE", new { id }, tx)
                ?? throw new ApiException(404, "Cadastro não encontrado.");
            Regras.Exigir(anterior.Versao == r.Versao, "O cadastro mudou. Atualize e tente novamente.", 409);
            bool usado = await c.ExecuteScalarAsync<int>(condicao
                ? "SELECT COUNT(*) FROM Pedidos WHERE CondicaoPagamentoId=@id"
                : "SELECT COUNT(*) FROM ParcelasPedido WHERE FormaPagamentoId=@id", new { id }, tx) > 0;
            var diasAtuais = condicao
                ? (await c.QueryAsync<int>("SELECT DiasAposConfirmacao FROM ParcelasCondicaoPagamento WHERE CondicaoPagamentoId=@id ORDER BY NumeroParcela", new { id }, tx)).ToArray()
                : [];
            Regras.Exigir(!usado || (anterior.Descricao == r.Descricao && (!condicao || diasAtuais.SequenceEqual(r.Dias))),
                "Este cadastro já está no histórico. Mantenha descrição/prazos e altere somente a atividade, ou cadastre outro.", 409);
            await c.ExecuteAsync($"UPDATE {tabela} SET Descricao=@Descricao,Ativo=@Ativo,Versao=Versao+1 WHERE Id=@id", new { id, r.Descricao, r.Ativo }, tx);
            if (condicao && !diasAtuais.SequenceEqual(r.Dias))
            {
                await c.ExecuteAsync("DELETE FROM ParcelasCondicaoPagamento WHERE CondicaoPagamentoId=@id", new { id }, tx);
                for (int i = 0; i < r.Dias.Length; i++)
                    await c.ExecuteAsync("INSERT INTO ParcelasCondicaoPagamento (CondicaoPagamentoId,NumeroParcela,DiasAposConfirmacao) VALUES (@id,@numero,@dias)", new { id, numero = i + 1, dias = r.Dias[i] }, tx);
            }
        }
        else
        {
            id = await c.ExecuteScalarAsync<int>($"INSERT INTO {tabela} (Descricao,Ativo) VALUES (@Descricao,@Ativo); SELECT LAST_INSERT_ID();", r, tx);
            if (condicao)
                for (int i = 0; i < r.Dias.Length; i++)
                    await c.ExecuteAsync("INSERT INTO ParcelasCondicaoPagamento (CondicaoPagamentoId,NumeroParcela,DiasAposConfirmacao) VALUES (@id,@numero,@dias)", new { id, numero = i + 1, dias = r.Dias[i] }, tx);
        }
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return new { Id = id };
    }

    public async Task<object> ReceberAsync(int id, int administradorId)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var conta = await c.QuerySingleOrDefaultAsync<ContaReceber>("SELECT * FROM ContasReceber WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Parcela não encontrada.");
        if (conta.Status == "Pendente")
        {
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(configuracao["Loja:FusoHorario"] ?? "America/Sao_Paulo");
            conta.Status = "Paga";
            conta.DataPagamento = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fuso).Date;
            conta.AdministradorBaixaId = administradorId;
            await c.ExecuteAsync("UPDATE ContasReceber SET Status='Paga',DataPagamento=@DataPagamento,AdministradorBaixaId=@AdministradorBaixaId WHERE Id=@Id", conta, tx);
            int vendedorId = await c.ExecuteScalarAsync<int>("SELECT pg.VendedorId FROM Pedidos p JOIN PedidoGeral pg ON pg.Id=p.PedidoGeralId WHERE p.Id=@PedidoId", conta, tx);
            await EventosService.RegistrarAsync(c, tx, "Pagamentos", vendedorId);
        }
        // Repetir uma baixa não troca data, responsável, valor ou meio de pagamento.
        await tx.CommitAsync();
        return new { conta.Id, conta.Status, DataPagamento = conta.DataPagamento?.ToString("yyyy-MM-dd"), conta.AdministradorBaixaId };
    }
}
````

## VanderCake.Api/Services/PedidosService.cs

````csharp
using System.Data;
using Dapper;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class PedidosService(Banco banco, LeituraService leitura, RateioService rateio, IConfiguration configuracao)
{
    public async Task<PedidoGeral> ReceberAsync(int vendedorId, PedidoGeralEnvio entrada)
    {
        var envio = Regras.Canonico(entrada);
        string hash = Regras.Hash(envio);
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        // O índice único faz requisições simultâneas do mesmo UUID aguardarem esta transação.
        await c.ExecuteAsync("""
            INSERT IGNORE INTO ControleEnvios (IdentificadorEnvio,VendedorId,ConteudoHash,Status,CriadoEm)
            VALUES (@IdentificadorEnvio,@vendedorId,@hash,'EmProcessamento',UTC_TIMESTAMP(6))
            """, new { envio.IdentificadorEnvio, vendedorId, hash }, tx);
        var controle = await c.QuerySingleAsync<ControleEnvio>(
            "SELECT * FROM ControleEnvios WHERE IdentificadorEnvio=@IdentificadorEnvio FOR UPDATE", envio, tx);
        Regras.Exigir(controle.VendedorId == vendedorId, "Identificador de envio indisponível.", 409);
        Regras.Exigir(controle.ConteudoHash == hash, "O identificador já foi utilizado com outro conteúdo. Verifique o envio original.", 409);
        if (controle.Status == "Rejeitado")
        {
            await tx.CommitAsync();
            throw new ApiException(controle.CodigoRejeicao ?? 422, controle.MotivoRejeicao ?? "Envio rejeitado.", true);
        }
        if (controle.Status == "Aceito")
        {
            await tx.CommitAsync();
            return await PorIdentificadorAsync(vendedorId, envio.IdentificadorEnvio);
        }

        var lote = new PedidoGeral
        {
            VendedorId = vendedorId, IdentificadorEnvio = envio.IdentificadorEnvio,
            DataHoraCriacao = envio.DataHoraCriacao.UtcDateTime, DataHoraRecebimento = DateTime.UtcNow
        };
        try
        {
            var vendedor = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@vendedorId FOR SHARE", new { vendedorId }, tx);
            Regras.Exigir(vendedor?.Ativo == true, "Vendedor inativo.", 401);
            lote.VendedorNome = vendedor!.Nome;
            Regras.Exigir(envio.DataHoraCriacao.Year >= 2000 && envio.DataHoraCriacao <= DateTimeOffset.UtcNow.AddMinutes(5), "Data de criação do envio inválida.");
            Regras.Exigir(envio.Pedidos.Select(p => p.ClienteId).Distinct().Count() == envio.Pedidos.Count, "Só pode existir um pedido por cliente neste lote.");
            var condicoesIds = envio.Pedidos.Select(p => p.CondicaoPagamentoId).Distinct().Order().ToArray();
            var formasIds = envio.Pedidos.SelectMany(p => p.Parcelas).Select(p => p.FormaPagamentoId).Distinct().Order().ToArray();
            // Uma edição de pagamento não pode mudar o significado de uma parcela durante o envio.
            await c.QueryAsync<int>("SELECT Id FROM CondicoesPagamento WHERE Id IN @condicoesIds ORDER BY Id FOR SHARE", new { condicoesIds }, tx);
            await c.QueryAsync<int>("SELECT Id FROM FormasPagamento WHERE Id IN @formasIds ORDER BY Id FOR SHARE", new { formasIds }, tx);
            var dados = await LeituraService.LerAsync(c, tx, vendedorId);
            // Bloqueios compartilhados preservam preço/atividade/carteira até o fim do recebimento.
            var clientesIds = envio.Pedidos.Select(p => p.ClienteId).Distinct().Order().ToArray();
            var clientes = (await c.QueryAsync<Cliente>("SELECT * FROM Clientes WHERE Id IN @clientesIds ORDER BY Id FOR SHARE", new { clientesIds }, tx)).ToDictionary(p => p.Id);
            var produtosIds = envio.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct().Order().ToArray();
            var produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos WHERE Id IN @produtosIds ORDER BY Id FOR SHARE", new { produtosIds }, tx)).ToDictionary(p => p.Id);
            foreach (var requisicao in envio.Pedidos)
            {
                Regras.Exigir(clientes.TryGetValue(requisicao.ClienteId, out var cliente) && cliente.Ativo && cliente.VendedorId == vendedorId,
                    "Um cliente não está ativo na sua carteira. Atualize os dados.");
                Regras.Exigir(requisicao.DataHoraCriacao.Year >= 2000 && requisicao.DataHoraCriacao <= DateTimeOffset.UtcNow.AddMinutes(5), "Data do pedido inválida.");
                var condicao = dados.CondicoesPagamento.SingleOrDefault(x => x.Id == requisicao.CondicaoPagamentoId && x.Ativo);
                Regras.Exigir(condicao is not null, "Condição de pagamento inválida.");
                Regras.Exigir(new[] { "0", "7/14", "7/14/21" }.Contains(string.Join('/', condicao!.Parcelas.Select(p => p.DiasAposConfirmacao))), "Prazos de pagamento incompatíveis.");
                Regras.Exigir(requisicao.Parcelas.Select(p => p.NumeroParcela).SequenceEqual(condicao.Parcelas.Select(p => p.NumeroParcela)), "Informe a forma de pagamento de cada parcela da condição escolhida.");
                Regras.Exigir(requisicao.Itens.Select(i => i.ProdutoId).Distinct().Count() == requisicao.Itens.Count, "Produto repetido no mesmo pedido.");
                var pedido = new Pedido
                {
                    ClienteId = cliente!.Id, ClienteNome = cliente.Nome, DataHoraCriacao = requisicao.DataHoraCriacao.UtcDateTime,
                    CondicaoPagamentoId = condicao.Id, CondicaoPagamentoDescricao = condicao.Descricao
                };
                foreach (var item in requisicao.Itens)
                {
                    Regras.Exigir(item.QuantidadeSolicitada > 0, "A quantidade deve ser positiva.");
                    Regras.Exigir(produtos.TryGetValue(item.ProdutoId, out var produto) && produto.Ativo, "Um produto está indisponível. Atualize o catálogo.");
                    Regras.Exigir(item.PrecoUnitario == produto!.Preco, $"O preço de {produto.Nome} mudou. Revise o carrinho.", 409);
                    pedido.Itens.Add(new ItemPedido
                    {
                        ProdutoId = produto.Id, NomeProduto = produto.Nome,
                        PrecoUnitario = produto.Preco, QuantidadeSolicitada = item.QuantidadeSolicitada
                    });
                }
                pedido.ValorTotalSolicitado = pedido.Itens.Sum(i => i.PrecoUnitario * i.QuantidadeSolicitada);
                Regras.Exigir(pedido.ValorTotalSolicitado is > 0 and <= Regras.LimiteDinheiro, "Total do pedido fora do limite.");
                var valores = Regras.Parcelar(pedido.ValorTotalSolicitado, condicao.Parcelas.Count);
                for (int i = 0; i < valores.Length; i++)
                {
                    var forma = dados.FormasPagamento.SingleOrDefault(f => f.Id == requisicao.Parcelas[i].FormaPagamentoId && f.Ativo);
                    Regras.Exigir(forma is not null, "Forma de pagamento inválida em uma parcela.");
                    pedido.Parcelas.Add(new ParcelaPedido
                    {
                        NumeroParcela = i + 1, DiasAposConfirmacao = condicao.Parcelas[i].DiasAposConfirmacao,
                        FormaPagamentoId = forma!.Id, FormaPagamentoNome = forma.Descricao, ValorPrevisto = valores[i]
                    });
                }
                lote.Pedidos.Add(pedido);
            }
        }
        catch (ApiException e) when (e.Status is 400 or 409 or 422)
        {
            // Nenhum pedido foi inserido: a rejeição fica permanente para este UUID.
            await c.ExecuteAsync("""
                UPDATE ControleEnvios SET Status='Rejeitado',CodigoRejeicao=@Status,MotivoRejeicao=@Message
                WHERE IdentificadorEnvio=@IdentificadorEnvio
                """, new { e.Status, e.Message, envio.IdentificadorEnvio }, tx);
            await tx.CommitAsync();
            throw new ApiException(e.Status, e.Message, true);
        }

        lote.Id = await c.ExecuteScalarAsync<int>("""
            INSERT INTO PedidoGeral (IdentificadorEnvio,VendedorId,DataHoraCriacao,DataHoraRecebimento)
            VALUES (@IdentificadorEnvio,@VendedorId,@DataHoraCriacao,@DataHoraRecebimento); SELECT LAST_INSERT_ID();
            """, lote, tx);
        foreach (var pedido in lote.Pedidos)
        {
            pedido.PedidoGeralId = lote.Id;
            pedido.Id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO Pedidos (PedidoGeralId,ClienteId,CondicaoPagamentoId,DataHoraCriacao,ValorTotalSolicitado)
                VALUES (@PedidoGeralId,@ClienteId,@CondicaoPagamentoId,@DataHoraCriacao,@ValorTotalSolicitado); SELECT LAST_INSERT_ID();
                """, pedido, tx);
            foreach (var item in pedido.Itens)
            {
                item.PedidoId = pedido.Id;
                item.Id = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO ItemPedido (PedidoId,ProdutoId,NomeProduto,QuantidadeSolicitada,PrecoUnitario)
                    VALUES (@PedidoId,@ProdutoId,@NomeProduto,@QuantidadeSolicitada,@PrecoUnitario); SELECT LAST_INSERT_ID();
                    """, item, tx);
            }
            foreach (var parcela in pedido.Parcelas)
            {
                parcela.PedidoId = pedido.Id;
                parcela.Id = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO ParcelasPedido (PedidoId,NumeroParcela,DiasAposConfirmacao,FormaPagamentoId,ValorPrevisto)
                    VALUES (@PedidoId,@NumeroParcela,@DiasAposConfirmacao,@FormaPagamentoId,@ValorPrevisto); SELECT LAST_INSERT_ID();
                    """, parcela, tx);
            }
        }
        await c.ExecuteAsync("UPDATE ControleEnvios SET Status='Aceito',PedidoGeralId=@Id WHERE IdentificadorEnvio=@IdentificadorEnvio", lote, tx);
        await EventosService.RegistrarAsync(c, tx, "Pedidos", vendedorId);
        await tx.CommitAsync();
        return lote;
    }

    public async Task<PedidoGeral> PorIdentificadorAsync(int vendedorId, string identificador)
    {
        var uuid = Regras.Uuid(identificador);
        var d = await leitura.ConsultarAsync(vendedorId);
        return d.PedidosGerais.SingleOrDefault(l => l.IdentificadorEnvio == uuid)
            ?? throw new ApiException(404, "Envio não encontrado nesta conta.");
    }

    public async Task<object> PreviaAsync(int id)
    {
        var d = await leitura.ConsultarAsync();
        var lote = d.PedidosGerais.SingleOrDefault(l => l.Id == id) ?? throw new ApiException(404, "Envio não encontrado.");
        var ids = lote.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).ToHashSet();
        return ProjecaoWeb.Previa(rateio.Gerar(lote, d.Produtos.Where(p => ids.Contains(p.Id)).ToList()));
    }

    public async Task<object> DecidirAsync(int id, int administradorId, ConfirmarRequest? confirmar, CancelarRequest? cancelar)
    {
        bool confirma = confirmar is not null;
        string acao = confirma ? "Confirmar" : "Cancelar";
        string chave = Regras.Uuid(confirmar?.IdentificadorOperacao ?? cancelar?.IdentificadorOperacao);
        string hash = confirma ? Regras.Hash(new { id, acao, confirmar!.TokenPrevia }) : Regras.Hash(new { id, acao, cancelar!.Motivo, cancelar.Versao });
        if (!confirma) cancelar!.Motivo = Regras.Texto(cancelar.Motivo, "Motivo do cancelamento", 255);
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var bloqueado = await c.QuerySingleOrDefaultAsync<PedidoGeral>("SELECT * FROM PedidoGeral WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Envio não encontrado.");
        var operacao = await c.QuerySingleOrDefaultAsync<Operacao>("SELECT * FROM Operacoes WHERE Identificador=@chave FOR UPDATE", new { chave }, tx);
        if (operacao is not null)
        {
            Regras.Exigir(operacao.PedidoGeralId == id && operacao.AdministradorId == administradorId && operacao.Acao == acao && operacao.ConteudoHash == hash,
                "Esta chave de operação já foi utilizada com outros dados.", 409);
            await tx.CommitAsync();
            return new { Id = id, bloqueado.Status };
        }
        if (bloqueado.Status != "AguardandoConfirmacao")
        {
            Regras.Exigir((confirma && bloqueado.Status == "Processado") ||
                (!confirma && bloqueado.Status == "Cancelado" && bloqueado.MotivoCancelamento == cancelar!.Motivo), "O envio já recebeu uma decisão diferente.", 409);
        }
        else if (confirma)
        {
            var dados = await LeituraService.LerAsync(c, tx);
            var lote = dados.PedidosGerais.Single(l => l.Id == id);
            var ids = lote.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct().Order().ToArray();
            var produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos WHERE Id IN @ids ORDER BY Id FOR UPDATE", new { ids }, tx)).AsList();
            rateio.Validar(confirmar!.TokenPrevia, lote, produtos);
            var previa = rateio.Gerar(lote, produtos);
            var agora = DateTime.UtcNow;
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(configuracao["Loja:FusoHorario"] ?? "America/Sao_Paulo");
            var dataBase = TimeZoneInfo.ConvertTimeFromUtc(agora, fuso).Date;
            foreach (var p in previa.Pedidos)
            {
                p.Status = p.ValorTotalAtendido == 0 ? "NaoAtendido" : p.Itens.Any(i => i.QuantidadeAtendida < i.QuantidadeSolicitada) ? "ConfirmadoComCorte" : "Confirmado";
                await c.ExecuteAsync("UPDATE Pedidos SET Status=@Status,ValorTotalAtendido=@ValorTotalAtendido WHERE Id=@Id", p, tx);
                foreach (var i in p.Itens)
                    await c.ExecuteAsync("UPDATE ItemPedido SET QuantidadeAtendida=@QuantidadeAtendida WHERE Id=@Id", i, tx);
                if (p.ValorTotalAtendido == 0) continue;
                var vendaId = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO Vendas (PedidoId,DataHoraConfirmacao,ValorTotal) VALUES (@Id,@agora,@ValorTotalAtendido);
                    SELECT LAST_INSERT_ID();
                    """, new { p.Id, agora, p.ValorTotalAtendido }, tx);
                foreach (var i in p.Itens.Where(i => i.QuantidadeAtendida > 0))
                    await c.ExecuteAsync("""
                        INSERT INTO ItemVenda (VendaId,PedidoId,ItemPedidoId,ProdutoId,NomeProduto,Quantidade,PrecoUnitario)
                        VALUES (@vendaId,@PedidoId,@Id,@ProdutoId,@NomeProduto,@QuantidadeAtendida,@PrecoUnitario)
                        """, new { vendaId, i.PedidoId, i.Id, i.ProdutoId, i.NomeProduto, i.QuantidadeAtendida, i.PrecoUnitario }, tx);
                foreach (var parcela in p.Parcelas.Where(x => x.ValorFinal > 0))
                    await c.ExecuteAsync("""
                        INSERT INTO ContasReceber (VendaId,PedidoId,ParcelaPedidoId,DataVencimento,Valor)
                        VALUES (@vendaId,@PedidoId,@Id,@vencimento,@ValorFinal)
                        """, new { vendaId, parcela.PedidoId, parcela.Id, vencimento = dataBase.AddDays(parcela.DiasAposConfirmacao), parcela.ValorFinal }, tx);
            }
            foreach (var produto in produtos)
            {
                int quantidade = previa.Pedidos.SelectMany(p => p.Itens).Where(i => i.ProdutoId == produto.Id).Sum(i => i.QuantidadeAtendida!.Value);
                if (quantidade > 0)
                    await c.ExecuteAsync("UPDATE Produtos SET Estoque=Estoque-@quantidade,Versao=Versao+1 WHERE Id=@Id", new { quantidade, produto.Id }, tx);
            }
            await c.ExecuteAsync("""
                UPDATE PedidoGeral SET Status='Processado',AdministradorId=@administradorId,
                    DataHoraProcessamento=@agora,Versao=Versao+1 WHERE Id=@id
                """, new { id, administradorId, agora }, tx);
            await EventosService.RegistrarAsync(c, tx, "Pedidos", lote.VendedorId);
            await EventosService.RegistrarAsync(c, tx, "Catalogo");
        }
        else
        {
            Regras.Exigir(bloqueado.Versao == cancelar!.Versao, "O envio mudou. Atualize os detalhes.", 409);
            await c.ExecuteAsync("UPDATE Pedidos SET Status='Cancelado' WHERE PedidoGeralId=@id", new { id }, tx);
            await c.ExecuteAsync("""
                UPDATE PedidoGeral SET Status='Cancelado',MotivoCancelamento=@Motivo,AdministradorId=@administradorId,
                    DataHoraProcessamento=UTC_TIMESTAMP(6),Versao=Versao+1 WHERE Id=@id
                """, new { id, cancelar.Motivo, administradorId }, tx);
            await EventosService.RegistrarAsync(c, tx, "Pedidos", bloqueado.VendedorId);
        }
        await c.ExecuteAsync("""
            INSERT INTO Operacoes (Identificador,PedidoGeralId,AdministradorId,Acao,ConteudoHash,CriadoEm)
            VALUES (@chave,@id,@administradorId,@acao,@hash,UTC_TIMESTAMP(6))
            """, new { chave, id, administradorId, acao, hash }, tx);
        await tx.CommitAsync();
        return new { Id = id, Status = confirma ? "Processado" : "Cancelado" };
    }
}
````

## VanderCake.Api/Services/ProjecaoWeb.cs

````csharp
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

// O banco e o Mobile usam decimal. A Web recebe inteiros em centavos.
public static class ProjecaoWeb
{
    public static object Produto(Produto p) => new
    {
        p.Id, p.Nome, p.CategoriaId, p.Descricao, p.Estoque, p.Ativo, p.Versao,
        PrecoCentavos = Regras.Centavos(p.Preco)
    };

    public static object Pedido(Pedido p) => new
    {
        p.Id, p.ClienteId, p.ClienteNome, p.DataHoraCriacao, p.CondicaoPagamentoId,
        p.CondicaoPagamentoDescricao, p.Status,
        ValorTotalSolicitadoCentavos = Regras.Centavos(p.ValorTotalSolicitado),
        ValorTotalAtendidoCentavos = Regras.Centavos(p.ValorTotalAtendido),
        Itens = p.Itens.Select(i => new
        {
            i.ProdutoId, i.NomeProduto, i.QuantidadeSolicitada, i.QuantidadeAtendida,
            PrecoUnitarioCentavos = Regras.Centavos(i.PrecoUnitario)
        }),
        Parcelas = p.Parcelas.Select(i => new
        {
            i.NumeroParcela, i.DiasAposConfirmacao, i.FormaPagamentoId, i.FormaPagamentoNome,
            ValorPrevistoCentavos = Regras.Centavos(i.ValorPrevisto),
            ValorFinalCentavos = Regras.Centavos(i.ValorFinal),
            DataVencimento = i.DataVencimento?.ToString("yyyy-MM-dd"), i.Situacao,
            DataPagamento = i.DataPagamento?.ToString("yyyy-MM-dd")
        })
    };

    public static object Lote(PedidoGeral l) => new
    {
        l.Id, l.IdentificadorEnvio, l.VendedorId, l.VendedorNome, l.DataHoraCriacao,
        l.DataHoraRecebimento, l.DataHoraProcessamento, l.AdministradorId, l.AdministradorNome,
        l.Status, l.MotivoCancelamento, l.Versao, Pedidos = l.Pedidos.Select(Pedido)
    };

    public static object Venda(Venda v) => new
    {
        v.Id, v.PedidoId, v.DataHoraConfirmacao, ValorTotalCentavos = Regras.Centavos(v.ValorTotal),
        Itens = v.Itens.Select(i => new
        {
            i.ProdutoId, i.NomeProduto, QuantidadeAtendida = i.Quantidade,
            PrecoUnitarioCentavos = Regras.Centavos(i.PrecoUnitario)
        })
    };

    public static object Conta(ContaReceber c) => new
    {
        c.Id, c.VendaId, c.PedidoId, c.NumeroParcela, c.FormaPagamentoId,
        DataVencimento = c.DataVencimento.ToString("yyyy-MM-dd"),
        ValorCentavos = Regras.Centavos(c.Valor), c.Status,
        DataPagamento = c.DataPagamento?.ToString("yyyy-MM-dd"), c.AdministradorBaixaId
    };

    public static object Painel(DadosLoja d) => new
    {
        d.Estados, d.Cidades, d.Categorias, Produtos = d.Produtos.Select(Produto),
        d.Vendedores, d.Clientes,
        CondicoesPagamento = d.CondicoesPagamento.Select(c => new { c.Id, c.Descricao, c.Ativo, c.Versao, Dias = c.Parcelas.Select(p => p.DiasAposConfirmacao) }),
        d.FormasPagamento, PedidosGerais = d.PedidosGerais.Select(Lote),
        Vendas = d.Vendas.Select(Venda), ContasReceber = d.ContasReceber.Select(Conta)
    };

    public static object Detalhes(DadosLoja d, int id)
    {
        var l = d.PedidosGerais.SingleOrDefault(x => x.Id == id) ?? throw new ApiException(404, "Pedido Geral não encontrado.");
        var pedidos = l.Pedidos.Select(p => p.Id).ToHashSet();
        var clientes = l.Pedidos.Select(p => p.ClienteId).ToHashSet();
        return new
        {
            Lote = Lote(l), Clientes = d.Clientes.Where(c => clientes.Contains(c.Id)), d.Cidades, d.Estados,
            Vendas = d.Vendas.Where(v => pedidos.Contains(v.PedidoId)).Select(Venda),
            ContasReceber = d.ContasReceber.Where(c => pedidos.Contains(c.PedidoId)).Select(Conta)
        };
    }

    public static object Previa(Previa p) => new
    {
        p.TokenPrevia, Pedidos = p.Pedidos.Select(Pedido),
        TotalSolicitadoCentavos = Regras.Centavos(p.TotalSolicitado),
        TotalAtendidoCentavos = Regras.Centavos(p.TotalAtendido), p.UnidadesCortadas
    };
}
````

## VanderCake.Api/Services/RateioService.cs

````csharp
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class RateioService(IDataProtectionProvider protecao)
{
    private readonly IDataProtector _protetor = protecao.CreateProtector("VanderCake.Previa.v1");
    private sealed record Chave(string Hash, DateTimeOffset Expira);

    public static string Assinatura(PedidoGeral lote, IReadOnlyList<Produto> produtos) => Regras.Hash(new
    {
        lote.Id, lote.Versao,
        Pedidos = lote.Pedidos.OrderBy(p => p.Id).Select(p => new
        {
            p.Id, p.ClienteId, p.CondicaoPagamentoId,
            Itens = p.Itens.OrderBy(i => i.ProdutoId).Select(i => new { i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario }),
            Parcelas = p.Parcelas.OrderBy(i => i.NumeroParcela).Select(i => new { i.NumeroParcela, i.DiasAposConfirmacao, i.FormaPagamentoId })
        }),
        Estoques = produtos.OrderBy(p => p.Id).Select(p => new { p.Id, p.Estoque, p.Versao })
    });

    public Previa Gerar(PedidoGeral lote, IReadOnlyList<Produto> produtos)
    {
        Regras.Exigir(lote.Status == "AguardandoConfirmacao", "Este lote já recebeu uma decisão.", 409);
        var pedidos = Regras.Copiar(lote.Pedidos);
        foreach (var produtoId in pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct())
        {
            var produto = produtos.Single(p => p.Id == produtoId);
            var demandas = pedidos.Where(p => p.Itens.Any(i => i.ProdutoId == produtoId))
                .Select(p => (p.ClienteId, p.Itens.Single(i => i.ProdutoId == produtoId).QuantidadeSolicitada)).ToList();
            var atendimento = Regras.Ratear(produto.Estoque, demandas);
            foreach (var pedido in pedidos)
            {
                var item = pedido.Itens.SingleOrDefault(i => i.ProdutoId == produtoId);
                if (item is not null) item.QuantidadeAtendida = atendimento[pedido.ClienteId];
            }
        }
        foreach (var pedido in pedidos)
        {
            pedido.ValorTotalAtendido = pedido.Itens.Sum(i => i.PrecoUnitario * i.QuantidadeAtendida!.Value);
            var parcelas = Regras.Parcelar(pedido.ValorTotalAtendido.Value, pedido.Parcelas.Count);
            for (int i = 0; i < parcelas.Length; i++) pedido.Parcelas[i].ValorFinal = parcelas[i];
        }
        return new Previa
        {
            TokenPrevia = _protetor.Protect(JsonSerializer.Serialize(new Chave(Assinatura(lote, produtos), DateTimeOffset.UtcNow.AddMinutes(10)))),
            Pedidos = pedidos,
            TotalSolicitado = pedidos.Sum(p => p.ValorTotalSolicitado),
            TotalAtendido = pedidos.Sum(p => p.ValorTotalAtendido!.Value),
            UnidadesCortadas = pedidos.SelectMany(p => p.Itens).Sum(i => (long)i.QuantidadeSolicitada - i.QuantidadeAtendida!.Value)
        };
    }

    public void Validar(string token, PedidoGeral lote, IReadOnlyList<Produto> produtos)
    {
        Chave? chave;
        try { chave = JsonSerializer.Deserialize<Chave>(_protetor.Unprotect(token)); }
        catch (Exception e) when (e is System.Security.Cryptography.CryptographicException or JsonException or ArgumentException)
        {
            throw new ApiException(409, "A prévia não é válida. Atualize os detalhes.");
        }
        Regras.Exigir(chave is not null && chave.Expira > DateTimeOffset.UtcNow && chave.Hash == Assinatura(lote, produtos),
            "O estoque ou a prévia mudou. Confira os dados atualizados antes de confirmar.", 409);
    }
}
````

## VanderCake.Api/VanderCake.Api.csproj

````xml
<Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <UserSecretsId>vandercake-api-local-v1</UserSecretsId>
        <StaticWebAssetsEnabled>false</StaticWebAssetsEnabled>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="MySqlConnector" Version="2.4.0" />
        <PackageReference Include="Dapper" Version="2.1.66" />
        <Content Include="../VanderCake.Web/dist/**/*" Link="wwwroot/%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" CopyToPublishDirectory="PreserveNewest" />
    </ItemGroup>
</Project>
````

## VanderCake.Api/appsettings.json

````json
{
    "ConnectionStrings": {
        "VanderCake": ""
    },
    "Cors": {
        "Origens": [
            "http://localhost:5500",
            "http://127.0.0.1:5500"
        ]
    },
    "Seguranca": {
        "HorasAcesso": 8,
        "DiasRenovacao": 30,
        "PastaChaves": ""
    },
    "Loja": {
        "FusoHorario": "America/Sao_Paulo"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*"
}
````

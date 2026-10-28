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

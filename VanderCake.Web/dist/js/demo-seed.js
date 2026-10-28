import { confirmarLote, gerarPrevia, dividirCentavos } from "./regras.js";

export const CONTA_DEMO = Object.freeze({ email: "admin@vandercake.local", senha: "VanderCake123!" });

export function criarDemonstracao() {
    const data = (dias, hora = 10) => {
        const d = new Date();
        d.setDate(d.getDate() - dias);
        d.setHours(hora, 24, 0, 0);
        return d.toISOString();
    };
    const endereco = { rua: "Rua das Panificadoras", numero: "120", bairro: "Centro", cep: "85851000", cidadeId: 1,
        telefone: "45000000000", ativo: true, versao: 1 };
    const base = {
        schema: 1, revisao: 1,
        estados: [{ id: 1, nome: "Paraná", uf: "PR", versao: 1 }, { id: 2, nome: "Santa Catarina", uf: "SC", versao: 1 }],
        cidades: [{ id: 1, nome: "Foz do Iguaçu", estadoId: 1, versao: 1 }, { id: 2, nome: "Cascavel", estadoId: 1, versao: 1 },
            { id: 3, nome: "Curitiba", estadoId: 1, versao: 1 }, { id: 4, nome: "Joinville", estadoId: 2, versao: 1 }],
        categorias: ["Farinhas", "Chocolates", "Laticínios", "Fermentos", "Confeitaria"].map((nome, i) => ({ id: i + 1, nome, ativo: true, versao: 1 })),
        produtos: [
            { id: 1, nome: "Farinha de trigo · 25 kg", categoriaId: 1, precoCentavos: 8950, estoque: 40, descricao: "Farinha especial para pães e massas." },
            { id: 2, nome: "Chocolate meio amargo · 1 kg", categoriaId: 2, precoCentavos: 3290, estoque: 30, descricao: "Cobertura para a rotina da confeitaria." },
            { id: 3, nome: "Creme de leite · 1 L", categoriaId: 3, precoCentavos: 1890, estoque: 28, descricao: "Creme culinário para doces e recheios." },
            { id: 4, nome: "Fermento biológico · 500 g", categoriaId: 4, precoCentavos: 1290, estoque: 45, descricao: "Fermento seco para panificação." },
            { id: 5, nome: "Açúcar de confeiteiro · 1 kg", categoriaId: 5, precoCentavos: 890, estoque: 36, descricao: "Açúcar fino para coberturas." },
            { id: 6, nome: "Granulado de chocolate · 1 kg", categoriaId: 5, precoCentavos: 2150, estoque: 22, descricao: "Finalização de bolos e doces." }
        ].map(p => ({ ...p, ativo: true, versao: 1 })),
        vendedores: [
            { ...endereco, id: 1, nome: "Ana Martins", documento: "00000000001", email: "ana@exemplo.test" },
            { ...endereco, id: 2, nome: "Rafael Costa", documento: "00000000002", email: "rafael@exemplo.test", cidadeId: 2 },
            { ...endereco, id: 3, nome: "Beatriz Lima", documento: "00000000003", email: "beatriz@exemplo.test", ativo: false }
        ],
        clientes: [
            { ...endereco, id: 10, nome: "Padaria Trigo & Cia", documento: "00000000000010", email: "trigo@exemplo.test", vendedorId: 1 },
            { ...endereco, id: 11, nome: "Confeitaria Doce Ponto", documento: "00000000000011", email: "doceponto@exemplo.test", vendedorId: 1, numero: "284" },
            { ...endereco, id: 12, nome: "Padaria Bom Dia", documento: "00000000000012", email: "bomdia@exemplo.test", vendedorId: 2, cidadeId: 2 },
            { ...endereco, id: 13, nome: "Ateliê de Bolos", documento: "00000000000013", email: "atelie@exemplo.test", vendedorId: 2, cidadeId: 2 },
            { ...endereco, id: 14, nome: "Empório dos Pães", documento: "00000000000014", email: "emporio@exemplo.test", vendedorId: 1 }
        ],
        condicoesPagamento: [{ id: 1, descricao: "À vista", dias: [0] }, { id: 2, descricao: "7/14 dias", dias: [7, 14] },
            { id: 3, descricao: "7/14/21 dias", dias: [7, 14, 21] }],
        formasPagamento: ["Pix", "Boleto bancário", "Dinheiro em espécie", "Cartão de crédito", "Cartão de débito"]
            .map((descricao, i) => ({ id: i + 1, descricao })),
        pedidosGerais: [], vendas: [], contasReceber: []
    };
    let pedidoId = 2031;
    function pedido(clienteId, condicaoId, produtos, dias) {
        const itens = produtos.map(([produtoId, quantidadeSolicitada]) => {
            const produto = base.produtos.find(p => p.id === produtoId);
            return { produtoId, nomeProduto: produto.nome, quantidadeSolicitada, quantidadeAtendida: null, precoUnitarioCentavos: produto.precoCentavos };
        });
        const total = itens.reduce((s, i) => s + i.quantidadeSolicitada * i.precoUnitarioCentavos, 0);
        const condicao = base.condicoesPagamento.find(c => c.id === condicaoId);
        const valores = dividirCentavos(total, condicao.dias.length);
        return { id: pedidoId++, clienteId, clienteNome: base.clientes.find(c => c.id === clienteId).nome,
            dataHoraCriacao: data(dias), status: "Pendente", condicaoPagamentoId: condicaoId,
            condicaoPagamentoDescricao: condicao.descricao, valorTotalSolicitadoCentavos: total,
            valorTotalAtendidoCentavos: null, itens,
            parcelas: condicao.dias.map((prazo, i) => ({ numeroParcela: i + 1, diasAposConfirmacao: prazo,
                formaPagamentoId: i + 1, formaPagamentoNome: base.formasPagamento[i].descricao,
                valorPrevistoCentavos: valores[i], valorFinalCentavos: null, dataVencimento: null })) };
    }
    function lote(id, vendedorId, pedidos, dias) {
        return { id, identificadorEnvio: `00000000-0000-4000-8000-${String(id).padStart(12, "0")}`,
            vendedorId, vendedorNome: base.vendedores.find(v => v.id === vendedorId).nome,
            dataHoraRecebimento: data(dias, 11), dataHoraCriacao: data(dias), dataHoraProcessamento: null,
            status: "AguardandoConfirmacao", motivoCancelamento: null, versao: 1, pedidos };
    }
    const passado = lote(1040, 1, [pedido(10, 3, [[1, 2], [2, 3]], 3), pedido(11, 2, [[3, 4]], 3), pedido(14, 1, [[5, 6]], 3)], 3);
    base.pedidosGerais.push(passado);
    confirmarLote(base, passado.id, gerarPrevia(passado, base.produtos).tokenPrevia, new Date(data(3, 14)));
    const cancelado = lote(1041, 2, [pedido(12, 1, [[4, 8]], 2)], 2);
    cancelado.status = "Cancelado";
    cancelado.motivoCancelamento = "Cliente solicitou o cancelamento antes da confirmação.";
    cancelado.dataHoraProcessamento = data(2, 15);
    cancelado.pedidos[0].status = "Cancelado";
    base.pedidosGerais.push(cancelado,
        lote(1042, 1, [pedido(10, 3, [[1, 10], [2, 5]], 1), pedido(11, 2, [[1, 6], [2, 3]], 1)], 1),
        lote(1043, 2, [pedido(12, 1, [[3, 6], [4, 4]], 0), pedido(13, 3, [[3, 8], [6, 3]], 0)], 0),
        lote(1044, 1, [pedido(14, 2, [[5, 10], [4, 6]], 0)], 0));
    base.produtos[0].estoque = 12;
    base.produtos[1].estoque = 7;
    return base;
}

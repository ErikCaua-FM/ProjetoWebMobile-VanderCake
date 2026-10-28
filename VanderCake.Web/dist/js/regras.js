// Regras puras do simulador local. Na integração real, a API é a autoridade.
export const LIMITE_CENTAVOS = 999999999999;
export const normalizar = valor => String(valor ?? "").normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "").toLowerCase().trim();
export const digitos = valor => String(valor ?? "").replace(/\D/g, "");
export const copiar = valor => structuredClone(valor);
export function dentroPeriodo(valor, inicio, fim) {
    if (!inicio && !fim) return true;
    const data = new Date(valor);
    if (!Number.isFinite(data.getTime())) return false;
    const partes = Object.fromEntries(new Intl.DateTimeFormat("pt-BR", {
        timeZone: "America/Sao_Paulo", year: "numeric", month: "2-digit", day: "2-digit"
    }).formatToParts(data).map(p => [p.type, p.value]));
    const dia = `${partes.year}-${partes.month}-${partes.day}`;
    return (!inicio || dia >= inicio) && (!fim || dia <= fim);
}

export function novoIdentificador() {
    if (typeof crypto.randomUUID === "function") return crypto.randomUUID();
    const bytes = crypto.getRandomValues(new Uint8Array(16));
    bytes[6] = (bytes[6] & 15) | 64;
    bytes[8] = (bytes[8] & 63) | 128;
    const hex = [...bytes].map(b => b.toString(16).padStart(2, "0")).join("");
    return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`;
}

export class Conflito extends Error {
    constructor(mensagem) { super(mensagem); this.name = "Conflito"; this.status = 409; }
}

export function lerCentavos(texto) {
    const valor = String(texto).trim();
    if (!/^\d+(?:[,.]\d{1,2})?$/.test(valor))
        throw new Error("Informe o preço como 49,90, sem separador de milhar.");
    const [inteiros, fracao = ""] = valor.replace(",", ".").split(".");
    const centavos = Number(inteiros) * 100 + Number(fracao.padEnd(2, "0"));
    if (!Number.isSafeInteger(centavos) || centavos < 0 || centavos > LIMITE_CENTAVOS)
        throw new Error("Preço fora do limite permitido.");
    return centavos;
}

export function dividirCentavos(total, quantidade) {
    if (!Number.isSafeInteger(total) || total < 0 || total > LIMITE_CENTAVOS ||
        ![1, 2, 3].includes(quantidade)) throw new Error("Total ou parcelas inválidos.");
    const parte = Math.floor(total / quantidade);
    return Array.from({ length: quantidade }, (_, i) =>
        i === quantidade - 1 ? total - parte * (quantidade - 1) : parte);
}

export function ratear(estoque, demandas) {
    if (!Number.isSafeInteger(estoque) || estoque < 0 || demandas.length === 0 ||
        demandas.some(d => !Number.isSafeInteger(d.quantidade) || d.quantidade <= 0) ||
        new Set(demandas.map(d => d.clienteId)).size !== demandas.length)
        throw new Error("Estoque ou demanda inválidos.");
    const total = demandas.reduce((soma, d) => soma + BigInt(d.quantidade), 0n);
    const saldo = BigInt(estoque) < total ? BigInt(estoque) : total;
    const quotas = demandas.map(d => ({
        clienteId: d.clienteId,
        quantidade: Number(saldo * BigInt(d.quantidade) / total),
        resto: saldo * BigInt(d.quantidade) % total
    }));
    let restantes = Number(saldo) - quotas.reduce((soma, q) => soma + q.quantidade, 0);
    const prioridade = [...quotas].sort((a, b) =>
        a.resto === b.resto ? a.clienteId - b.clienteId : a.resto > b.resto ? -1 : 1);
    for (const quota of prioridade) {
        if (restantes-- <= 0) break;
        quota.quantidade++;
    }
    return quotas.map(({ clienteId, quantidade }) => ({ clienteId, quantidade }));
}

function somarItens(itens, campo) {
    const total = itens.reduce((soma, item) => {
        if (!Number.isSafeInteger(item.precoUnitarioCentavos) || item.precoUnitarioCentavos <= 0)
            throw new Error("Preço inválido no pedido recebido.");
        return soma + BigInt(item[campo]) * BigInt(item.precoUnitarioCentavos);
    }, 0n);
    if (total > BigInt(LIMITE_CENTAVOS)) throw new Error("Total do pedido fora do limite.");
    return Number(total);
}

export function gerarPrevia(lote, produtos) {
    if (lote.status !== "AguardandoConfirmacao") throw new Error("Este lote não está pendente.");
    if (!lote.pedidos.length || new Set(lote.pedidos.map(p => p.clienteId)).size !== lote.pedidos.length)
        throw new Error("O lote deve possuir um pedido por cliente.");
    const pedidos = copiar(lote.pedidos);
    const ids = [...new Set(pedidos.flatMap(p => p.itens.map(i => i.produtoId)))].sort((a, b) => a - b);
    const estoqueUsado = [];
    for (const pedido of pedidos) {
        if (!pedido.itens.length || new Set(pedido.itens.map(i => i.produtoId)).size !== pedido.itens.length)
            throw new Error("Itens inválidos no pedido recebido.");
        const prazos = pedido.parcelas.map(p => p.diasAposConfirmacao).join("/");
        if (!["0", "7/14", "7/14/21"].includes(prazos) || pedido.parcelas.some((p, i) =>
            p.numeroParcela !== i + 1 || !Number.isInteger(p.formaPagamentoId) || p.formaPagamentoId <= 0))
            throw new Error("Pagamento inválido no pedido recebido.");
    }
    for (const id of ids) {
        const produto = produtos.find(p => p.id === id);
        if (!produto) throw new Error("Produto do pedido não encontrado no estoque.");
        const demandas = pedidos.filter(p => p.itens.some(i => i.produtoId === id)).map(p => ({
            clienteId: p.clienteId,
            quantidade: p.itens.find(i => i.produtoId === id).quantidadeSolicitada
        }));
        const distribuicao = ratear(produto.estoque, demandas);
        estoqueUsado.push([id, produto.estoque, produto.versao]);
        for (const pedido of pedidos) {
            const item = pedido.itens.find(i => i.produtoId === id);
            if (item) item.quantidadeAtendida = distribuicao.find(d => d.clienteId === pedido.clienteId).quantidade;
        }
    }
    for (const pedido of pedidos) {
        pedido.valorTotalSolicitadoCentavos = somarItens(pedido.itens, "quantidadeSolicitada");
        pedido.valorTotalAtendidoCentavos = somarItens(pedido.itens, "quantidadeAtendida");
        const valores = dividirCentavos(pedido.valorTotalAtendidoCentavos, pedido.parcelas.length);
        pedido.parcelas.forEach((p, i) => { p.valorFinalCentavos = valores[i]; });
    }
    return {
        // Em produção, este token será emitido e validado pelo servidor.
        tokenPrevia: JSON.stringify([lote.id, lote.versao, lote.pedidos, estoqueUsado]),
        pedidos,
        totalSolicitadoCentavos: pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0),
        totalAtendidoCentavos: pedidos.reduce((s, p) => s + p.valorTotalAtendidoCentavos, 0),
        unidadesCortadas: pedidos.reduce((s, p) => s + p.itens.reduce((n, i) =>
            n + i.quantidadeSolicitada - i.quantidadeAtendida, 0), 0)
    };
}

export const proximoId = lista => Math.max(0, ...lista.map(i => i.id)) + 1;
const encontrarLote = (base, id) => base.pedidosGerais.find(l => l.id === id) ??
    (() => { throw new Error("Pedido Geral não encontrado."); })();

export function adicionarDias(data, dias) {
    const dia = new Date(`${data}T12:00:00Z`);
    dia.setUTCDate(dia.getUTCDate() + dias);
    return dia.toISOString().slice(0, 10);
}

export function confirmarLote(base, id, token, agora = new Date()) {
    const lote = encontrarLote(base, id);
    if (lote.status === "Processado") return lote; // Uma confirmação já feita não baixa novamente.
    const previa = gerarPrevia(lote, base.produtos);
    if (previa.tokenPrevia !== token)
        throw new Conflito("O estoque mudou. Confira a nova prévia antes de confirmar.");
    const dataConfirmacao = agora.toISOString();
    const dataBase = new Intl.DateTimeFormat("en-CA", { timeZone: "America/Sao_Paulo" }).format(agora);
    const vendas = [];
    const contas = [];
    let vendaId = proximoId(base.vendas);
    let contaId = proximoId(base.contasReceber);
    for (const pedido of previa.pedidos) {
        const corte = pedido.itens.some(i => i.quantidadeAtendida < i.quantidadeSolicitada);
        pedido.status = pedido.valorTotalAtendidoCentavos === 0 ? "NaoAtendido" :
            corte ? "ConfirmadoComCorte" : "Confirmado";
        pedido.parcelas.forEach(p => {
            p.dataVencimento = pedido.valorTotalAtendidoCentavos > 0 ? adicionarDias(dataBase, p.diasAposConfirmacao) : null;
        });
        if (pedido.valorTotalAtendidoCentavos === 0) continue;
        const venda = { id: vendaId++, pedidoId: pedido.id, dataHoraConfirmacao: dataConfirmacao,
            valorTotalCentavos: pedido.valorTotalAtendidoCentavos,
            itens: pedido.itens.filter(i => i.quantidadeAtendida > 0).map(i => copiar(i)) };
        vendas.push(venda);
        for (const parcela of pedido.parcelas.filter(p => p.valorFinalCentavos > 0)) {
            contas.push({ id: contaId++, vendaId: venda.id, pedidoId: pedido.id,
                numeroParcela: parcela.numeroParcela, formaPagamentoId: parcela.formaPagamentoId,
                dataVencimento: parcela.dataVencimento, valorCentavos: parcela.valorFinalCentavos, status: "Pendente" });
        }
    }
    // Todas as validações e contas são feitas antes de alterar o estado.
    for (const produto of base.produtos) {
        const quantidade = previa.pedidos.flatMap(p => p.itens).filter(i => i.produtoId === produto.id)
            .reduce((s, i) => s + i.quantidadeAtendida, 0);
        if (quantidade) { produto.estoque -= quantidade; produto.versao++; }
    }
    lote.pedidos = previa.pedidos;
    lote.status = "Processado";
    lote.dataHoraProcessamento = dataConfirmacao;
    lote.administradorNome = "Administrador de demonstração";
    lote.versao++;
    base.vendas.push(...vendas);
    base.contasReceber.push(...contas);
    return lote;
}

export function cancelarLote(base, id, motivo, versao, agora = new Date()) {
    const lote = encontrarLote(base, id);
    if (lote.status === "Cancelado") return lote;
    if (lote.status !== "AguardandoConfirmacao") throw new Conflito("Este lote já foi processado.");
    if (lote.versao !== versao) throw new Conflito("O pedido mudou. Atualize os detalhes.");
    const texto = String(motivo ?? "").trim();
    if (!texto || texto.length > 255) throw new Error("Informe o motivo do cancelamento, com até 255 caracteres.");
    lote.status = "Cancelado";
    lote.motivoCancelamento = texto;
    lote.dataHoraProcessamento = agora.toISOString();
    lote.administradorNome = "Administrador de demonstração";
    lote.versao++;
    lote.pedidos.forEach(p => { p.status = "Cancelado"; });
    return lote;
}

function texto(valor, campo, max, opcional = false) {
    const resultado = String(valor ?? "").trim();
    if ((!opcional && !resultado) || resultado.length > max)
        throw new Error(`${campo}: preencha com até ${max} caracteres.`);
    return resultado;
}
function inteiro(valor, campo, minimo = 1) {
    if (!Number.isSafeInteger(valor) || valor < minimo || valor > 2147483647)
        throw new Error(`${campo}: informe um número inteiro válido.`);
    return valor;
}
function unico(lista, campo, valor, id, nome = campo) {
    if (lista.some(x => x.id !== id && normalizar(x[campo]) === normalizar(valor)))
        throw new Error(`Já existe um cadastro com este ${nome}.`);
}

export function salvarCadastro(base, tipo, entrada, id = null) {
    const lista = base[tipo];
    if (!["produtos", "vendedores", "clientes", "categorias", "estados", "cidades"].includes(tipo))
        throw new Error("Cadastro inválido.");
    const atual = id === null ? null : lista.find(x => x.id === id);
    if (id !== null && !atual) throw new Error("Cadastro não encontrado.");
    if (atual && atual.versao !== entrada.versao)
        throw new Conflito("Este cadastro mudou em outra tela. Reabra-o antes de salvar.");
    let valores = {};
    if (tipo === "estados") {
        const nome = texto(entrada.nome, "Nome do estado", 100);
        const uf = texto(entrada.uf, "UF", 2).toUpperCase();
        if (!/^[A-Z]{2}$/.test(uf)) throw new Error("A UF deve conter duas letras.");
        unico(lista, "uf", uf, id, "UF");
        valores = { nome, uf };
    } else if (tipo === "cidades") {
        const nome = texto(entrada.nome, "Nome da cidade", 100);
        const estadoId = inteiro(entrada.estadoId, "Estado");
        if (!base.estados.some(e => e.id === estadoId)) throw new Error("Selecione um estado cadastrado.");
        unico(lista.filter(c => c.estadoId === estadoId), "nome", nome, id, "nome de cidade neste estado");
        valores = { nome, estadoId };
    } else if (tipo === "categorias") {
        const nome = texto(entrada.nome, "Categoria", 60);
        unico(lista, "nome", nome, id, "nome de categoria");
        valores = { nome, ativo: true };
    } else if (tipo === "produtos") {
        const nome = texto(entrada.nome, "Nome do produto", 100);
        const categoriaId = inteiro(entrada.categoriaId, "Categoria");
        if (!base.categorias.some(c => c.id === categoriaId && c.ativo)) throw new Error("Selecione uma categoria ativa.");
        const precoCentavos = entrada.precoCentavos;
        if (!Number.isSafeInteger(precoCentavos) || precoCentavos <= 0 || precoCentavos > LIMITE_CENTAVOS)
            throw new Error("Informe um preço positivo com até duas casas decimais.");
        valores = { nome, categoriaId, precoCentavos,
            estoque: inteiro(entrada.estoque, "Estoque", 0),
            descricao: texto(entrada.descricao, "Descrição", 4000, true), ativo: atual ? Boolean(entrada.ativo) : true };
    } else {
        const nome = texto(entrada.nome, "Nome", 100);
        const documento = digitos(entrada.documento);
        if (!(tipo === "clientes" ? [11, 14] : [11]).includes(documento.length))
            throw new Error(tipo === "clientes" ? "Informe CPF com 11 ou CNPJ com 14 dígitos." : "Informe o CPF com 11 dígitos.");
        if (lista.some(p => p.id !== id && digitos(p.documento) === documento))
            throw new Error("Este documento já está cadastrado.");
        const cidadeId = inteiro(entrada.cidadeId, "Cidade");
        if (!base.cidades.some(c => c.id === cidadeId)) throw new Error("Selecione uma cidade cadastrada.");
        const cep = digitos(entrada.cep);
        if (cep.length !== 8) throw new Error("Informe um CEP com 8 dígitos.");
        const telefone = digitos(entrada.telefone);
        if (telefone.length < 10 || telefone.length > 13) throw new Error("Informe um telefone com DDD.");
        const email = texto(entrada.email, "E-mail", 150, tipo === "clientes").toLowerCase();
        if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) throw new Error("Informe um e-mail válido.");
        valores = { nome, documento, cidadeId, cep, telefone, email,
            rua: texto(entrada.rua, "Rua", 100), numero: texto(entrada.numero, "Número", 10),
            bairro: texto(entrada.bairro, "Bairro", 60), ativo: atual?.ativo ?? true };
        if (tipo === "vendedores") {
            unico(lista, "email", email, id, "e-mail");
            if ((!atual || entrada.senha) && (String(entrada.senha ?? "").length < 8 || entrada.senha.length > 128))
                throw new Error("A senha deve ter de 8 a 128 caracteres.");
            // O simulador não armazena senhas de vendedores. O hash pertence à futura API.
        } else {
            const vendedorId = inteiro(entrada.vendedorId, "Vendedor");
            const vendedor = base.vendedores.find(v => v.id === vendedorId);
            if (!vendedor || (!vendedor.ativo && atual?.vendedorId !== vendedorId))
                throw new Error("Selecione um vendedor ativo.");
            valores.vendedorId = vendedorId;
            valores.ativo = atual ? Boolean(entrada.ativo) : true;
        }
    }
    const registro = { ...valores, id: atual?.id ?? proximoId(lista), versao: (atual?.versao ?? 0) + 1 };
    if (atual) lista[lista.indexOf(atual)] = registro;
    else lista.push(registro);
    return registro;
}

export function alterarAtividade(base, id, ativo, versao) {
    const vendedor = base.vendedores.find(v => v.id === id);
    if (!vendedor) throw new Error("Vendedor não encontrado.");
    if (vendedor.versao !== versao) throw new Conflito("O cadastro mudou. Atualize o painel.");
    vendedor.ativo = Boolean(ativo);
    vendedor.versao++;
    return vendedor;
}

export function excluirProduto(base, id, versao) {
    const produto = base.produtos.find(p => p.id === id);
    if (!produto) throw new Error("Produto não encontrado.");
    if (produto.versao !== versao) throw new Conflito("O produto mudou. Atualize o painel.");
    const utilizado = base.pedidosGerais.some(l => l.pedidos.some(p => p.itens.some(i => i.produtoId === id)));
    if (utilizado) { produto.ativo = false; produto.versao++; }
    else base.produtos = base.produtos.filter(p => p.id !== id);
    return { desativado: utilizado };
}

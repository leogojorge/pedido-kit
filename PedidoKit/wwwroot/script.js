const tableBody = document.querySelector("#kitsTable tbody");
const form = document.getElementById("kitForm");
const kitIdInput = document.getElementById("kitId");

async function fetchKits() {
    const response = await fetch("/api/kits");
    const kits = await response.json();
    let count = 1;

    tableBody.innerHTML = "";
    kits.forEach(kit => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td id="id-${kit.id}"         item-id=${kit.id}>${kit.id}</td>
            <td id="nome-${kit.id}"       item-id=${kit.id}>${kit.nome}</td>
            <td id="descricao-${kit.id}"  item-id=${kit.id}>${kit.descricao}</td>
            <td id="peca-${kit.id}"       item-id=${kit.id}>${kit.pecas}</td>
            <td id="quantidade-${kit.id}" item-id=${kit.id}>
                <input type="number" id="quantidade-input-${kit.id}" item-id=${kit.id} name="quantidade-input" tabindex="${count}"/>
            </td>
            <td id="total-${kit.id}" item-id=${kit.id}>${kit.total}</td>
            <td>
                <button onclick="editKit(${kit.id})">Editar</button>
                <button onclick="deleteKit(${kit.id})" style="background:#f55;color:white;">Excluir</button>
            </td>
        `;
        tableBody.appendChild(row);
        count++;
    });
    let generateOrderButtom = document.getElementById("gerar_pedido_buttom");
    generateOrderButtom.setAttribute("tabIndex", count);
}

async function editKit(id) {
    const response = await fetch(`/api/kits/${id}`);
    const kit = await response.json();

    kitIdInput.value = kit.id;
    document.getElementById("nome").value = kit.nome;
    document.getElementById("descricao").value = kit.descricao;
    document.getElementById("pecas").value = kit.pecas;
    document.getElementById("total").value = kit.total;
}

async function deleteKit(id) {
    if (!confirm("Deseja realmente excluir este kit?")) return;

    const response = await fetch(`/api/kits/${id}`, { method: "DELETE" });
    if (response.ok) {
        alert("Kit excluído com sucesso!");
        fetchKits();
    } else {
        alert("Erro ao excluir kit");
    }
}

async function generateOrder() {
    const quantityInputs = document.querySelectorAll('[name="quantidade-input"]');

    if (!quantityInputs || quantityInputs.length === 0) return;

    const resultMap = new Map();
    let precoTotal = 0;

    quantityInputs.forEach(input => {
        const quantity = parseFloat(input.value);

        if (!quantity || quantity < 1) return;

        const itemId = input.getAttribute("item-id");
        const pecaEle = document.getElementById("peca-" + itemId);
        const precoTotalKitEl = document.getElementById("total-" + itemId);

        if (!pecaEle || !precoTotalKitEl) return;

        const precoKit = parseFloat(precoTotalKitEl.innerText.replace(",", "."));
        precoTotal += precoKit * quantity;

        const pecas = pecaEle.innerText.trim().split(/\s+/);

        pecas.forEach(item => {
            const [rawQtd, rawCod] = item.split("-");
            const qtd = parseFloat(rawQtd);
            const pecaCod = rawCod?.trim();

            if (!qtd || !pecaCod) return;

            const finalQtd = qtd * quantity;

            if (resultMap.has(pecaCod))
                resultMap.set(pecaCod, resultMap.get(pecaCod) + finalQtd);
            else
                resultMap.set(pecaCod, finalQtd);
            
        });
    });

    const pedido = document.getElementById("pedido");
    const valorTotal = document.getElementById("valor-total");

    let html = "";

    resultMap.forEach((qtd, cod) => {
        html += `${qtd} - ${cod}<br/>`;
    });

    pedido.innerHTML = html;
    valorTotal.innerText = precoTotal.toFixed(2);

    copyToClipBoard();
}

async function copyToClipBoard() {
    var copyText = document.getElementById("pedido");

    navigator.clipboard.writeText(copyText.innerText);
}

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const kit = {
        nome: document.getElementById("nome").value,
        descricao: document.getElementById("descricao").value,
        pecas: document.getElementById("pecas").value,
        total: document.getElementById("total").value
    };

    const id = kitIdInput.value;

    const method = id ? "PUT" : "POST";
    const url = id ? `/api/kits/${id}` : "/api/kits";

    const response = await fetch(url, {
        method: method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(kit)
    });

    if (response.ok) {
        alert("Kit salvo com sucesso!");
        form.reset();
        fetchKits();
    } else {
        alert("Erro ao salvar kit");
    }
});

fetchKits();

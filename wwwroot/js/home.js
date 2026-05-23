/* =========================================================
   VARIABLES GLOBALES
========================================================= */

let selectedColors = [];

let cart = [];

let totalSpent = 0;


/* =========================================================
   MODAL DE CARTAS
========================================================= */

// Abre el modal con la informacion de una carta.
function openModal(name, image, price, rarity, type, set){

    document.getElementById("modalCardName").innerText = name;

    document.getElementById("modalCardImage").src = image;

    document.getElementById("modalCardPrice").innerText = price;

    document.getElementById("modalRarity").innerText = rarity;

    document.getElementById("modalType").innerText = type;

    document.getElementById("modalSet").innerText = set;

    document.getElementById("cardModal").style.display = "flex";

}

// Cierra el modal de detalle de carta.
function closeModal(){

    document.getElementById("cardModal").style.display = "none";

}


/* =========================================================
   ELEMENTOS DEL CATÁLOGO
========================================================= */

const searchInput =
    document.getElementById("catalogSearch");

const rarityFilter =
    document.getElementById("rarityFilter");

const typeFilter =
    document.getElementById("typeFilter");

const cards =
    document.querySelectorAll(".mtg-card");

const colorButtons =
    document.querySelectorAll(".color-chip");


/* =========================================================
   FILTRO DE COLORES
========================================================= */

if(colorButtons.length > 0){

    colorButtons.forEach(button => {

        // Activa o desactiva el color seleccionado.
        button.addEventListener("click", function(){

            const color = button.dataset.color;

            if(selectedColors.includes(color)){

                selectedColors =
                    selectedColors.filter(c => c !== color);

                button.classList.remove("active");

            }
            else{

                selectedColors.push(color);

                button.classList.add("active");

            }

            filterCards();

        });

    });

}


/* =========================================================
   FILTRAR CARTAS
========================================================= */

// Aplica los filtros activos del catalogo.
function filterCards(){

    if(!searchInput || !rarityFilter || !typeFilter)
        return;

    const searchValue =
        searchInput.value.toLowerCase();

    const rarityValue =
        rarityFilter.value;

    const typeValue =
        typeFilter.value;

    cards.forEach(card => {

        const cardName =
            card.dataset.name.toLowerCase();

        const cardRarity =
            card.dataset.rarity;

        const cardTypes =
            card.dataset.type
                .split(",")
                .map(type => type.trim().toLowerCase());

        const cardColors =
            card.dataset.colors.split(",");

        const matchesSearch =
            cardName.includes(searchValue);

        const matchesRarity =
            rarityValue === "" ||
            cardRarity === rarityValue;

        const matchesType =
            typeValue === "" ||
            cardTypes.includes(typeValue.toLowerCase());

        const matchesColors =
            selectedColors.length === 0 ||
            selectedColors.some(color =>
                cardColors.includes(color)
            );

        if(
            matchesSearch &&
            matchesRarity &&
            matchesType &&
            matchesColors
        ){

            card.style.display = "block";

        }
        else{

            card.style.display = "none";

        }

    });

}


/* =========================================================
   EVENTOS CATÁLOGO
========================================================= */

// Escucha cambios en el buscador del catalogo.
if(searchInput){

    searchInput.addEventListener(
        "input",
        filterCards
    );

}

// Escucha cambios en el filtro de rareza.
if(rarityFilter){

    rarityFilter.addEventListener(
        "change",
        filterCards
    );

}

// Escucha cambios en el filtro de tipo.
if(typeFilter){

    typeFilter.addEventListener(
        "change",
        filterCards
    );

}


/* =========================================================
   PRESUPUESTOS
========================================================= */

// Agrega una carta al carrito o aumenta su cantidad.
function addToCart(name, price){

    const existing =
        cart.find(item => item.name === name);

    if(existing){

        existing.quantity++;

    }
    else{

        cart.push({
            name,
            price,
            quantity:1
        });

    }

    renderCart();

}


// Renderiza las cartas agregadas y recalcula el total.
function renderCart(){

    const cartItems =
        document.getElementById("cartItems");

    const cartTotal =
        document.getElementById("cartTotal");

    if(!cartItems || !cartTotal)
        return;

    cartItems.innerHTML = "";

    totalSpent = 0;

    cart.forEach((item,index)=>{

        totalSpent +=
            item.price * item.quantity;

        cartItems.innerHTML += `

            <div class="cart-item">

                <div class="cart-item-top">

                    <span>${item.name}</span>

                    <button
                        class="qty-btn"
                        onclick="removeItem(${index})">

                        🗑

                    </button>

                </div>

                <div class="cart-controls">

                    <div class="qty-controls">

                        <button
                            class="qty-btn"
                            onclick="changeQty(${index},-1)">
                            -
                        </button>

                        <span style="color:white">
                            ${item.quantity}
                        </span>

                        <button
                            class="qty-btn"
                            onclick="changeQty(${index},1)">
                            +
                        </button>

                    </div>

                    <span style="color:#ffd700;font-weight:700">
                        $${(item.price * item.quantity).toFixed(2)}
                    </span>

                </div>

            </div>

        `;

    });

    if(cart.length === 0){

        cartItems.innerHTML = `

            <p class="empty-cart">
                Tu carrito está vacío
            </p>

        `;

    }

    cartTotal.innerText =
        `$${totalSpent.toFixed(2)}`;

    updateBudget();

}


// Cambia la cantidad de una carta dentro del carrito.
function changeQty(index,delta){

    cart[index].quantity += delta;

    if(cart[index].quantity <= 0){

        cart.splice(index,1);

    }

    renderCart();

}


// Elimina una carta del carrito.
function removeItem(index){

    cart.splice(index,1);

    renderCart();

}


// Actualiza gasto, restante, barra/grafica y persistencia.
function updateBudget(){

    const budgetInput =
        document.getElementById("budgetInput");

    const spentText =
        document.getElementById("spentText");

    const remainingText =
        document.getElementById("remainingText");

    const progress =
        document.getElementById("progressFill");

    if(
        !budgetInput ||
        !spentText ||
        !remainingText
    ){
        return;
    }

    const budget =
        parseFloat(budgetInput.value) || 0;

    const remaining =
        budget - totalSpent;

    const percent =
        budget > 0
        ? (totalSpent / budget) * 100
        : 0;

    spentText.innerText =
        `Gastado: $${totalSpent.toFixed(2)}`;

    remainingText.innerText =
        `Restante: $${remaining.toFixed(2)}`;

    if(progress){

        progress.style.width =
            `${Math.min(percent,100)}%`;

        if(percent > 100){

            progress.style.background =
                "#f44336";

        }
        else if(percent > 75){

            progress.style.background =
                "#ff9800";

        }
        else{

            progress.style.background =
                "#4caf50";

        }

    }

    if(typeof window.updateBudgetChart === "function"){

        window.updateBudgetChart({
            budget,
            totalSpent,
            remaining,
            percent
        });

    }

    if(typeof window.persistBudgetState === "function"){

        window.persistBudgetState();

    }

}

/* ====================================================
   BASE DE DATOS CARTAS
==================================================== */

const mtgCards = [

    {
        id:"gishath",
        name:"Gishath, Sun's Avatar",
        image:"/imagenes/GishathSunsAvatar__94123.jpg",
        price:"$11.99",
        rarity:"Mítica",
        type:"Legendary Creature",
        set:"Ixalan"
    },

    {
        id:"ureni",
        name:"Ureni of the Unwritten",
        image:"/imagenes/UreniOfTheUnwritten009__88381.jpg",
        price:"$17.99",
        rarity:"Mítica",
        type:"Legendary Creature",
        set:"Tarkir"
    }

];


/* ====================================================
   SEARCH AUTOCOMPLETE
==================================================== */

// Busca cartas de ejemplo por nombre.
function searchCards(text){

    const results =
        document.getElementById("searchResults");

    if(!results)
        return;

    results.innerHTML = "";

    if(text.trim() === "")
        return;

    const filtered =
        mtgCards.filter(card =>
            card.name
                .toLowerCase()
                .includes(text.toLowerCase())
        );

    filtered.forEach(card => {

        results.innerHTML += `

            <div
                class="search-item"
                onclick="goToCard('${card.id}')">

                ${card.name}

            </div>

        `;

    });

    /* SI ESCRIBIÓ EL NOMBRE EXACTO */

    const exact =
        mtgCards.find(card =>
            card.name.toLowerCase() ===
            text.toLowerCase()
        );

    if(exact){

        goToCard(exact.id);

    }

}


/* ====================================================
   REDIRECCIÓN
==================================================== */

// Redirige al detalle de una carta de ejemplo.
function goToCard(id){

    window.location.href =
        `/Cartas/Detalle/${id}`;

}


/* ====================================================
   CARGAR DETALLE
==================================================== */

if(typeof cardId !== "undefined"){

    const card =
        mtgCards.find(c => c.id === cardId);

    if(card){

        document.getElementById("detailName")
            .innerText = card.name;

        document.getElementById("detailImage")
            .src = card.image;

        document.getElementById("detailPrice")
            .innerText = card.price;

        document.getElementById("detailRarity")
            .innerText = card.rarity;

        document.getElementById("detailType")
            .innerText = card.type;

        document.getElementById("detailSet")
            .innerText = card.set;

    }

}

/* ====================================================
   MODAL DE EDICIÓN
==================================================== */

// Abre el modal de edicion con los datos de la carta.
function openEditModal(
    id,
    nombre,
    precio,
    rarezaId,
    tipo,
    coleccion,
    imagen
){

    document.getElementById("editId").value = id;

    document.getElementById("editNombre").value = nombre;

    document.getElementById("editPrecio").value = precio;

    document.getElementById("editRarezaId").value = rarezaId;

    document.getElementById("editTipo").value = tipo;

    document.getElementById("editColeccion").value = coleccion;

    document.getElementById("editImagen").value = imagen;

    document.getElementById("editModal").style.display =
        "flex";

}

// Cierra el modal de edicion.
function closeEditModal(){

    document.getElementById("editModal").style.display =
        "none";

}

function openModal(name, image, price, rarity, type, set){

    document.getElementById("modalCardName").innerText = name;

    document.getElementById("modalCardImage").src = image;

    document.getElementById("modalCardPrice").innerText = price;

    document.getElementById("modalRarity").innerText = rarity;

    document.getElementById("modalType").innerText = type;

    document.getElementById("modalSet").innerText = set;

    document.getElementById("cardModal").style.display = "flex";

}

function closeModal(){

    document.getElementById("cardModal").style.display = "none";

}

const searchInput = document.getElementById("catalogSearch");

const rarityFilter = document.getElementById("rarityFilter");

const typeFilter = document.getElementById("typeFilter");

const cards = document.querySelectorAll(".mtg-card");

const colorButtons = document.querySelectorAll(".color-chip");

let selectedColors = [];

/* FILTRO DE COLORES */

colorButtons.forEach(button => {

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

/* FILTRAR CARTAS */

function filterCards(){

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

        const cardType =
            card.dataset.type;

        const cardColors =
            card.dataset.colors.split(",");

        const matchesSearch =
            cardName.includes(searchValue);

        const matchesRarity =
            rarityValue === "" ||
            cardRarity === rarityValue;

        const matchesType =
            typeValue === "" ||
            cardType === typeValue;

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

/* EVENTOS */

searchInput.addEventListener(
    "input",
    filterCards
);

rarityFilter.addEventListener(
    "change",
    filterCards
);

typeFilter.addEventListener(
    "change",
    filterCards
);
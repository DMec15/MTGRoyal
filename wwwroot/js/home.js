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
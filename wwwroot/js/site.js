document.getElementById("searchMenu")?.addEventListener("keyup", function () {

    var search = this.value.toLowerCase();
    var cards = document.querySelectorAll(".menu-card");

    cards.forEach(function (card) {

        var text = card.innerText.toLowerCase();

        if (text.includes(search)) {
            card.style.display = "";
        } else {
            card.style.display = "none";
        }

    });

});
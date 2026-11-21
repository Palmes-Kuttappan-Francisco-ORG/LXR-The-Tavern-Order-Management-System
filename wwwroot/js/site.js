document.getElementById("searchMenu")?.addEventListener("keyup", function () {

    var search = this.value.toLowerCase();

    document.querySelectorAll(".menu-card").forEach(function (card) {
        var text = card.innerText.toLowerCase();

        card.style.display = text.includes(search) ? "" : "none";
    });

});
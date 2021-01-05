$('#theme').on("change", function () {
    var item = $("#theme option:selected").text();

    $.post("/Home/SetTheme",
        {
            data: item
        });
});
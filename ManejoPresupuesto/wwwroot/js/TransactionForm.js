function initTransactionForm(urlGetCategories) {
    $("#OperationTypeId").change(async function () {
        const selectedValue = $(this).val();

        const answer = await fetch(urlGetCategories, {
            method: 'POST',
            body: selectedValue,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await answer.json();
        const options =
            json.map(categorie => `<option value=${categorie.value}>${categorie.text}</option>`);
        $("#CategorieId").html(options);

    })
}
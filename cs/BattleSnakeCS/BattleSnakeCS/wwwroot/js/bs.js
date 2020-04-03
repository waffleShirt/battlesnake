$("#set-snake-params").on("click", function () 
{
    console.log("clicky click");

    var data = {
        "color" : $("#snake-color").val(),
        "head-type": $("#head-type").val(), 
        "tail-type" : $("#tail-type").val()
    };

    console.log(data); 

    var jqxhr = $.ajax({
        url: "https://localhost:44346/BattleSnake/setsnakeparams",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        //success: formSubmitSuccess,
        //error: formSubmitFailure,
    });
}); 


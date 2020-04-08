$("#set-snake-params").on("click", function () 
{
    console.log("clicky click");

    var data = {
        "color" : $("#snake-color").val(),
        "headType": $("#head-type").val(), 
        "tailType" : $("#tail-type").val()
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

$(document).ready(function ()
{
    var jqxhr = $.ajax({
        url: "https://localhost:44346/BattleSnake/getsnakeparams",
        method: "GET",
        contentType: "application/json",
        success: GetSnakeParamsSuccess,
        //error: formSubmitFailure,
    });
});

function GetSnakeParamsSuccess(data, textStatus, jqXHR)
{ 
    $("#head-type").val(data.headType); 
    $("#tail-type").val(data.tailType); 
    $("#snake-color").val(data.color); 

}
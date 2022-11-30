function aJaxPost(action, myData, target) {
    $.ajax({
        method: "POST",
        url: action,
        data: myData,
        success: function (response) {
            $(target).html(response);
            // alert(response);
            // alert(target + " Details saved successfully!!!");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $(target).html(xhr.responseText);
            //alert(xhr.status);
            console.log(xhr);
            console.log(xhr.status);
            
            //alert(thrownError);
        }
    });
}
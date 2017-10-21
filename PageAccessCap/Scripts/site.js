/// <reference path="jquery-1.10.2.js" />

(function () {

    $("form").submit(function (e) {
        e.preventDefault();

        $.ajax({
            url: this.action,
            method: this.method,
            dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                var err = jqXHR.responseJSON;

                if (!err || err.sc !== 303) {
                    alert(jqXHR.responseText);
                }
                else {
                    window.location.replace(err.desc);
                }
            }
        });
    });

}());
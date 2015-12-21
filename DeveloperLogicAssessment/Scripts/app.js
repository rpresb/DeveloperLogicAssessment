var login = function () {
    $("#login-msg")
        .addClass("hidden")
        .removeClass("alert-danger")
        .removeClass("alert-warning")
        .removeClass("alert-info")
        .removeClass("alert-success");

    $.post("/api/user/login", {
        UserID: $("#login").val(),
        PasswordRaw: $("#password").val()
    }).done(function (data) {
        $("#login-msg").addClass("alert-success");
        $("#login-msg").html("User verified");

        loadRegisteredUsers();
    }).fail(function (event, jqXHR, ajaxSettings) {
        if (event.status == 409) {
            $("#login-msg").addClass("alert-warning");
            $("#login-msg").html(JSON.parse(event.responseText).message);
        } else if (event.status == 412) {
            $("#login-msg").addClass("alert-info");
            $("#login-msg").html(JSON.parse(event.responseText).message);
            loadRegisteredUsers();
        } else {
            $("#login-msg").addClass("alert-danger");
            $("#login-msg").html(event.responseText);
        }
    }).always(function () {
        $("#login-msg").removeClass("hidden");
    });
};

var register = function () {
    $("#register-msg")
        .addClass("hidden")
        .removeClass("alert-danger")
        .removeClass("alert-success");

    $.post("/api/user", {
        UserID: $("#username").val()
    }).done(function (data) {
        $("#register-msg").addClass("alert-success");
        $("#register-msg").html("Success");

        loadRegisteredUsers();
    }).fail(function (event, jqXHR, ajaxSettings) {
        $("#register-msg").addClass("alert-danger");

        if (event.status == 409) {
            $("#register-msg").html(JSON.parse(event.responseText).message);
        } else {
            $("#register-msg").html(event.responseText);
        }
    }).always(function () {
        $("#register-msg").removeClass("hidden");
    });
};

var loadRegisteredUsers = function () {
    $("#registered-user").html("Loading...");
    $.get("/api/user")
        .done(function (data) {
            $("#registered-user").html("");

            if (data.length == 0) {
                $("#registered-user").html("There is no registered user");
                return;
            }

            var template = $("#registered-user-template");
            data.forEach(function (item) {
                var itemHtml = template.clone();
                itemHtml.removeClass("hidden");
                itemHtml.removeAttr("id");
                $(".list-group-item-heading", itemHtml).html(item.UserID);

                if (!item.Verified) {
                    $(".list-group-item-text", itemHtml).html(item.PasswordRaw + " - Expires " + item.ExpirationDate.split('T')[1]);
                } else {
                    $(".list-group-item-text", itemHtml).html("Verified");
                }

                $("#registered-user").append(itemHtml);
            });

        });
};

$(document).ready(function () {
    $("#register-btn").on("click", register);
    $("#login-btn").on("click", login);
    loadRegisteredUsers();
});
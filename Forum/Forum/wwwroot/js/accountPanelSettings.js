﻿$("#changeUsernameButton")
    .click(function () {
        $.ajax(
            {
                type: "GET",
                url: "/Account/ChangeUsername",
                success: function (test) {
                    $('#account-panel').html(test);
                }
            });

        $("#changeUsernameButton").removeClass("text-white");
        $("#changeUsernameButton").removeClass("menu-bg-forum");

        $("#changeUsernameButton").addClass("active");

        if ($("#changePasswordButton").hasClass("active")) {
            $("#changePasswordButton").removeClass("active");

            $("#changePasswordButton").addClass("menu-bg-forum");
            $("#changePasswordButton").addClass("text-white");
        }
        if ($("#deleteAccButton").hasClass("active")) {
            $("#deleteAccButton").removeClass("active");

            $("#deleteAccButton").addClass("menu-bg-forum");
            $("#deleteAccButton").addClass("text-white");
        }
    });

$("#changePasswordButton")
    .click(function () {
        $.ajax(
            {
                type: "GET",
                url: "/Account/ChangePassword",
                success: function (test) {
                    $('#account-panel').html(test);
                }
            });

        $("#changePasswordButton").removeClass("text-white");
        $("#changePasswordButton").removeClass("menu-bg-forum");

        $("#changePasswordButton").addClass("active");

        if ($("#changeUsernameButton").hasClass("active")) {
            $("#changeUsernameButton").removeClass("active");

            $("#changeUsernameButton").addClass("menu-bg-forum");
            $("#changeUsernameButton").addClass("text-white");
        }
        if ($("#deleteAccButton").hasClass("active")) {
            $("#deleteAccButton").removeClass("active");

            $("#deleteAccButton").addClass("menu-bg-forum");
            $("#deleteAccButton").addClass("text-white");
        }   
    });

$("#deleteAccButton")
    .click(function () {
        $.ajax(
            {
                type: "GET",
                url: "/Account/DeleteAccount",
                success: function (test) {
                    $('#account-panel').html(test);
                }
            });

        $("#deleteAccButton").removeClass("text-white");
        $("#deleteAccButton").removeClass("menu-bg-forum");

        $("#deleteAccButton").addClass("active");

        if ($("#changeUsernameButton").hasClass("active")) {
            $("#changeUsernameButton").removeClass("active");

            $("#changeUsernameButton").addClass("menu-bg-forum");
            $("#changeUsernameButton").addClass("text-white");
        }
        if ($("#changePasswordButton").hasClass("active")) {
            $("#changePasswordButton").removeClass("active");

            $("#changePasswordButton").addClass("menu-bg-forum");
            $("#changePasswordButton").addClass("text-white");
        }
    });
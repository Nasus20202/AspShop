﻿function hideCookies() {
    var info = document.getElementById("cookieInfo");
    document.cookie = "acceptedCookies=true; path=/";
    info.style.display = "none";
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

function checkIfCookiesAccepted() {
    var cookie = getCookie("acceptedCookies");
    if (cookie == 'true')
        return true;
    return false;
}


function onStart() {
    //alert(checkIfCookiesAccepted())
    //alert(getCookie("acceptedCookies"));
    if (checkIfCookiesAccepted()) {
        var info = document.getElementById("cookieInfo");
        info.style.display = "none";
    }

}
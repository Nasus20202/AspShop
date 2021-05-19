function hideCookies() {
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
    if (checkIfCookiesAccepted()) {
        var info = document.getElementById("cookieInfo");
        info.style.display = "none";
    }
}

function showPasswordChange() {
    var button = document.getElementById("passwordShow");
    var seciton = document.getElementById("changePassword");
    button.style.display = "none";
    seciton.style.display = "inline";
}

function clickById(id) {
    var element = document.getElementById(id);
    element.click();
}
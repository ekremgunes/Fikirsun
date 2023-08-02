
const userName = document.querySelector("#UserName")
const password = document.querySelector("#Password")
const email = document.querySelector("#Email")
const repassword = document.querySelector("#RePassword")
var userNameStatus = false
var passwordStatus = false
var emailStatus = false
var repasswordStatus = false



function comparePasswords() {
    if (repassword.value != password.value || password.value.trim() == "") {
        repassword.style.borderColor = "#f22d19ed"
        return false
    }
    else {
        repassword.style.borderColor = "#46ca0a"
        return true
    }
}
window.onscroll = function () { scrollFunction() }

window.onload = function () { loadAnimate() }
function loadAnimate() {
    var loader = document.getElementById("loader")
    var wrapper = document.querySelector(".wrapper")
    loader.style.display = "none"
    wrapper.style.display = "none"    
}
//function filterCountry(countryId) {
//    //code
//    console.log(countryId)
//    var icon = document.querySelector("#worldIcon")
//    icon.style.color = "#0392d8eb"
//    setTimeout(() => {
//        icon.style.color = ""
//    }, 2000);
//}
//function filterTop(params) {
//    //code
//    console.log("tops")
//}
//function filterHot(params) { //new
//    //code
//    console.log("hots")

//}


function LoadMore(element) {
    var hourglassIcon = document.querySelector(".fa-hourglass")
    //code
    element.style.display = "none"
    hourglassIcon.style.display = "inline-block"
    //action is done
    //fake action timing
    setTimeout(() => {
        element.style.display = "inline-block"
        hourglassIcon.style.display = "none"
    }, 2000);

}


function scrollFunction() {
    if (document.body.scrollTop > (document.body.scrollHeight * 0.15) ||
        document.documentElement.scrollTop > (document.documentElement.scrollHeight * 0.15)) {
        document.querySelector('.back-to-top').style.display = "block"
    } else {
        document.querySelector('.back-to-top').style.display = "none"
    }
}

document.querySelector('.back-to-top').addEventListener('click', function () {
    document.body.scrollTop = 0
    document.documentElement.scrollTop = 0
});

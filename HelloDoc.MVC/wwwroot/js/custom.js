// function sidebar_wrap(){
//     var x = document.getElementById("side-id");

//     if(x.style.display == "block"){
//         // x.style.display = "none";
//         x.style.setProperty("display", "none", "important")
//     }

//     else{
//         x.style.display = "block";
//     }
// }

// function lastid_wrap(){
//     var x = document.getElementById("last");

//     if(x.style.display == "block"){
//         x.style.display = "none";
//     }

//     else{
//         x.style.display = "block";
//     }
// }

// Initialization for ES Users
// import { Input, initMDB } from "mdb-ui


//  function hellofunction() {
//     var element = document.getElementById("xyz");
//     var element1 = document.getElementById("abc");

//     element.classList.toggle("dark-mode");
//     element1.classList.toggle("dark-mode");


//  }

// *********************************************************** for Submit For Me and Submit For Some one Button ***********************************************************
const toggleButton = (curr_btn, redirect_page) => {
    const buttons = document.getElementsByClassName("common-btn")
    for (let i = 0; i < buttons.length; ++i) {
        if (buttons[i] != curr_btn) {
            buttons[i].classList.remove('create-request-active')
        }
        else {
            buttons[i].classList.add('create-request-active')
        }
    }
    document.getElementById("redirect-value").value = redirect_page;
}

const redirect = () => {
    window.location.assign(`./${document.getElementById("redirect-value").value}`);
}






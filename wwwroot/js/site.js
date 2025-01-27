// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

            

document.addEventListener('DOMContentLoaded', () => {
    const buttons = document.querySelectorAll('.Btn');
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const controller = this.getAttribute('data-controller');
            const action = this.getAttribute('data-action');

            console.log(`Button clicked: Controller=${controller}, Action=${action}`);

            if (controller && action) {
                window.location.href = `/${controller}/${action}`;
            } else {
                console.error("Controller or action attribute is missing!");
            }
        });
    });
});

console.log("site.js is loaded and running");

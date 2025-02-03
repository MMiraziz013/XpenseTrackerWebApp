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

document.addEventListener('DOMContentLoaded', () => {
    const sideBar = document.getElementById('sideBar');

    // Function to update the sidebar height
    const updateSidebarHeight = () => {
        const navbarHeight = document.querySelector('nav').offsetHeight; // Get navbar height
        const viewportHeight = window.innerHeight; // Get viewport height
        sideBar.style.height = `${viewportHeight - navbarHeight}px`; // Calculate sidebar height
    };

    // Run on page load
    updateSidebarHeight();

    // Update on window resize
    window.addEventListener('resize', updateSidebarHeight);
});
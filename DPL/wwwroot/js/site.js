// Wait until the DOM is fully loaded
document.addEventListener("DOMContentLoaded", function () {
    // Find the alert message element
    var alertMessage = document.getElementById("alertMessage");

    // If the alert message element exists
    if (alertMessage) {
        // Set a timeout to remove the alert message after 3 seconds (3000 milliseconds)
        setTimeout(function () {
            alertMessage.style.display = 'none';
        }, 3000);
    }
});

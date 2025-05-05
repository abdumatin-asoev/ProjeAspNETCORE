document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("login-form").addEventListener("submit", function (e) {
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value.trim();

        let valid = true;

        const email_error = document.getElementById("email_error");
        if (email_error) {
            email_error.innerText = "";
        }
        const password_error = document.getElementById("password_error");
        if (password_error) {
            password_error.innerText = "";
        }

        if (email === "") {
            if (email_error) {
                email_error.innerText = "Email is required.";
            }
            valid = false;
        } else if (!email.includes("@")) {
            if (email_error) {
                email_error.innerText = "Invalid email format.";
            }
            valid = false;
        }

        if (password === "") {
            if (password_error) {
                password_error.innerText = "Password is required.";
            }
            valid = false;
        }

        if (!valid) {
            e.preventDefault();
        }
    });
});

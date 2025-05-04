document.getElementById("login-form").addEventListener("submit", function (e) {
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();

    let valid = true;

    // Очистить старые ошибки
    document.getElementById("email-error").innerText = "";
    document.getElementById("password-error").innerText = "";

    if (email === "") {
        document.getElementById("email_error").innerText = "Email is required.";
        valid = false;
    } else if (!email.includes("@")) {
        document.getElementById("email_error").innerText = "Invalid email format.";
        valid = false;
    }

    if (password === "") {
        document.getElementById("password_error").innerText = "Password is required.";
        valid = false;
    }

    if (!valid) {
        e.preventDefault(); // Остановить отправку формы
    }
});

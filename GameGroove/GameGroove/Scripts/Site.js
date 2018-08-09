function hidePassword() {
    var password = document.getElementById("passwordInput");
    if (password.type === "password") {
        password.type = "text";
    } else {
        password.type = "password";
    }
}

function hideConfirmPassword() {
    var confirmPassword = document.getElementById("confirmPasswordInput");
    if (confirmPassword.type === "password") {
        confirmPassword.type = "text";
    } else {
        confirmPassword.type = "password";
    }
}

function hideNewPassword() {
    var newPassword = document.getElementById("newPasswordInput");
    if (newPassword.type === "password") {
        newPassword.type = "text";
    } else {
        newPassword.type = "password";
    }
}


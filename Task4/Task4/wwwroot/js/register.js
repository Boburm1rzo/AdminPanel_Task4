function checkPasswordStrength() {
    const password = document.getElementById('floatingPassword').value;
    const strengthBar = document.getElementById('strengthBar');

    let strength = 0;
    if (password.length >= 8) strength += 25;
    if (password.match(/[a-z]/)) strength += 25;
    if (password.match(/[A-Z]/)) strength += 25;
    if (password.match(/[0-9]/)) strength += 25;

    strengthBar.style.width = strength + '%';

    if (strength <= 25) {
        strengthBar.style.backgroundColor = '#dc3545';
    } else if (strength <= 50) {
        strengthBar.style.backgroundColor = '#ffc107';
    } else if (strength <= 75) {
        strengthBar.style.backgroundColor = '#17a2b8';
    } else {
        strengthBar.style.backgroundColor = '#28a745';
    }
}
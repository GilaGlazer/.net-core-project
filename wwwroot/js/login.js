document.getElementById("loginForm").addEventListener("submit", async (event) => {
    event.preventDefault(); // Prevent form submission

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    // Basic validation
    if (!email || !password) {
        alert("User not found");
        return;
    }

    try {
        const response = await fetch("/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ Email: email, Password: password }), // שים לב לשמות השדות
        });

        if (!response.ok) {
            throw new Error("Login failed. Please check your credentials.");
        }

        const token = await response.text(); // Read the token from the response
        console.log("Token:", token);

        // Save the token to localStorage or sessionStorage
        localStorage.setItem("authToken", token);

        // Redirect to another page if needed
        window.location.href = "/html/item.html";
    } catch (error) {
        console.log(error.message);
    }
});
// Add toggle password visibility functionality
document.addEventListener('DOMContentLoaded', () => {
    const passwordInput = document.getElementById('password');
    const togglePassword = document.getElementById('togglePassword');

    togglePassword.addEventListener('click', () => {
        const isPasswordVisible = passwordInput.type === 'text';
        passwordInput.type = isPasswordVisible ? 'password' : 'text';
        togglePassword.src = isPasswordVisible ? '../images/visible.png' : '../images/hide.png';
    });
});
document.getElementById("loginForm").addEventListener("submit", async (event) => {
    event.preventDefault(); // Prevent form submission

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    // Basic validation
    if (!email || !password) {
        alert("Please fill out all fields.");
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

        alert("Sign In successful!");
        // Redirect to another page if needed
        window.location.href = "/html/item.html";
    } catch (error) {
        alert(error.message);
    }
});



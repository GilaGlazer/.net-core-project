// פונקציה לניווט לעמוד לוגין
const redirectToLogin = () => {
    window.location.href = "/html/login.html"; // עדכן את הנתיב לפי דף הלוגין שלך
};

// פונקציה להתנתקות המשתמש
const logoutUser = () => {
    localStorage.removeItem("authToken"); // ניקוי הטוקן
    redirectToLogin(); // ניתוב לדף הלוגין
};
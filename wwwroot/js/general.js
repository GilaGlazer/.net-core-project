// פונקציה לניווט לעמוד לוגין
const redirectToLogin = () => {
    window.location.href = "/html/login.html"; // עדכן את הנתיב לפי דף הלוגין שלך
};

// פונקציה להתנתקות המשתמש
const logoutUser = () => {
    localStorage.removeItem("authToken"); // ניקוי הטוקן
    redirectToLogin(); // ניתוב לדף הלוגין
};


// פונקציה להבאת כל המשתמשים (admin בלבד)
const getAllUsers = async () => {
    try {
       const token= getToken(); 
        if (!token) {
            alert("You are not logged in. Please log in.");
            return;
        }
        const response = await fetch(userUri, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            if (response.status === 403) {
                console.log("Access denied. Admin permissions required.");
            } else {
                throw new Error(`Unable to fetch users: ${response.statusText}`);
            }
            return;
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching users:', error);
    }
};

// פונקציה לקבלת פריטים
const getAllItems = async () => {
    const token = getToken();
    if (!token)
      return;
    try {
      const response = await fetch('/shoes', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      const data = await response.json();      
      return data;
    } catch (error) {
      console.error('Unable to get items.', error);
    }
  };
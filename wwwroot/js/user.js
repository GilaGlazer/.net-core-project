const userUri = '/users'; // URI למשתמשים
let users = []; // מערך למשתמשים

// פונקציה להבאת כל המשתמשים (admin בלבד)
const getUsers = async () => {
    try {
        const token = localStorage.getItem("authToken"); // קבלת הטוקן
        if (!token) {
            alert("No token found. Please log in.");
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
            if (response.status === 401) {
                console.log("Access denied. Admin permissions required.");
            } else {
                throw new Error(`Unable to fetch users: ${response.statusText}`);
            }
            return;
        }

        const data = await response.json();
        _displayUsers(data);
    } catch (error) {
        console.error('Error fetching users:', error);
    }
};

const addUser = async () => {
    const addUsernameTextbox = document.getElementById('add-username');
    const addPasswordTextbox = document.getElementById('add-password');
    const addEmailTextbox = document.getElementById('add-email');
    const addTypeTextbox = document.getElementById('add-type');

    const user = {
        userName: addUsernameTextbox.value.trim(),
        password: addPasswordTextbox.value.trim(),
        email: addEmailTextbox.value.trim(),
        type: addTypeTextbox.value.trim()
    };

    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            alert("You are not logged in. Please log in.");
            return;
        }

        const response = await fetch(userUri, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            throw new Error(`Unable to add user: ${response.statusText}`);
        }

        await getUsers(); // עדכון הרשימה
        addUsernameTextbox.value = '';
        addPasswordTextbox.value = '';
        addEmailTextbox.value = '';
        addTypeTextbox.value = '';
        closeAddUserModal(); // סגירת החלון לאחר ההוספה
    } catch (error) {
        console.error('Error adding user:', error);
    }
};

// פונקציה למחיקת משתמש (admin בלבד)
const deleteUser = async (id) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            alert("You are not logged in. Please log in.");
            return;
        }

        const response = await fetch(`${userUri}/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error(`Unable to delete user: ${response.statusText}`);
        }

        await getUsers(); // עדכון הרשימה
    } catch (error) {
        console.error('Error deleting user:', error);
    }
};

// // פונקציה להצגת טופס עריכה
const displayEditUserForm = (id) => {
    const user = users.find(user => user.id === id);

    if (!user) {
        console.log("User not found.");
        return;
    }

    // טוען את פרטי המשתמש לטופס
    document.getElementById('edit-username').value = user.userName || '';
    document.getElementById('edit-email').value = user.email || '';
    document.getElementById('edit-password').value = user.password || '';
    document.getElementById('edit-type').value = user.type || '';
    document.getElementById('edit-id').value = user.id || '';

    // פותח את חלון העריכה
    openEditUserModal(id);
};

// פונקציה לסגירת טופס עריכה
const closeUserInput = () => {
    document.getElementById('editUserForm').style.display = 'none';
};

// פונקציה להצגת כמות המשתמשים
const _displayUserCount = (userCount) => {
    const name = (userCount === 1) ? 'user' : 'users';
    document.getElementById('user-counter').innerText = `${userCount} ${name}`;
};

// פונקציה להצגת המשתמשים בטבלה
const _displayUsers = (data) => {
    const tBody = document.getElementById('users');
    tBody.innerHTML = '';

    _displayUserCount(data.length);

    const button = document.createElement('button');

    data.forEach(user => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditUserForm(${user.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteUser(${user.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNodeUsername = document.createTextNode(user.userName);
        td1.appendChild(textNodeUsername);

        let td2 = tr.insertCell(1);
        let textNodeEmail = document.createTextNode(user.email);
        td2.appendChild(textNodeEmail);

        let td3 = tr.insertCell(2);
        let textNodePassword = document.createTextNode(user.password);
        td3.appendChild(textNodePassword);

        let td4 = tr.insertCell(3);
        let textNodeType = document.createTextNode(user.type);
        td4.appendChild(textNodeType);

        let td5 = tr.insertCell(4);
        td5.appendChild(editButton);

        let td6 = tr.insertCell(5);
        td6.appendChild(deleteButton);
    });

    users = data; // עדכון המערך ל-users
};



const openAddUserModal = () => {
    document.getElementById('addUserModal').style.display = 'block';
};

const closeAddUserModal = () => {
    document.getElementById('addUserModal').style.display = 'none';
};



// פונקציה להצגת חלון עריכה
const openEditUserModal = (id) => {
    const user = users.find(user => user.id === id);

    if (!user) {
        console.log("User not found.");
        return;
    }

    // טוען את פרטי המשתמש לטופס
    document.getElementById('edit-username').value = user.userName || '';
    document.getElementById('edit-email').value = user.email || '';
    document.getElementById('edit-password').value = user.password || '';
    document.getElementById('edit-type').value = user.type || '';
    document.getElementById('edit-id').value = user.id || '';

    // מציג את החלון
    document.getElementById('editUserModal').style.display = 'block';
};

// פונקציה לסגירת חלון עריכה
const closeEditUserModal = () => {
    document.getElementById('editUserModal').style.display = 'none';
};

// פונקציה לעדכון משתמש (admin בלבד)
const updateUser = async () => {
    const userId = document.getElementById('edit-id').value;
    const user = {
        id: parseInt(userId, 10),
        userName: document.getElementById('edit-username').value.trim(),
        password: document.getElementById('edit-password').value.trim(),
        email: document.getElementById('edit-email').value.trim(),
        type: document.getElementById('edit-type').value.trim()
    };

    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            alert("You are not logged in. Please log in.");
            return;
        }

        const response = await fetch(`${userUri}/${userId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            throw new Error(`Unable to update user: ${response.statusText}`);
        }

        await getUsers(); // עדכון הרשימה
        closeEditUserModal(); // סגירת חלון העריכה
    } catch (error) {
        console.error('Error updating user:', error);
    }
};



const redirectToItemsPage = () => {
    window.location.href = "/html/item.html";
};



const apiRequest = async(url, method, body = null) => {
    const token = getToken();
    if (!token) {
        console.error('No authentication token found.');
        throw new Error('Authentication required.');
    }
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
    };
    try {
        const response = await fetch(url, {
            method: method,
            headers: headers,
            body: body ? JSON.stringify(body) : null,
        });
        if (!response.ok) {
            throw new Error(`Unable to perform ${method} request: ${response.statusText}`);
        }
        return await response.json();
    } catch (error) {
        console.error(error);
        throw error;
    }
};

const shoeUri = '/shoes'; // URI לנעליים
let shoes = []; // מערך הנעליים
const userUri = '/users';
let currentUserDetails = {};

const getToken = () => {
    const token = localStorage.getItem("authToken");
    if (!token) return null;

    try {
        const payloadBase64 = token.split('.')[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);

        const currentTime = Math.floor(Date.now() / 1000); // שניות מאז 1970
        if (payload.exp && payload.exp > currentTime) {
            return token;
        } else {
            // התוקף פג – מחק מהאחסון
            localStorage.removeItem("authToken");
            return null;
        }
    } catch (e) {
        // טוקן לא תקין – מחק אותו
        localStorage.removeItem("authToken");
        return null;
    }
};

// פונקציה לבדוק אם יש טוקן ולנווט
const checkTokenAndRedirect = () => {
    const token = getToken();
    const currentPath = window.location.pathname;
    if (token && currentPath !== "/html/item.html") {
        window.location.href = "/html/item.html";
    } else if (!token && currentPath !== "/html/login.html") {
        window.location.href = "/html/login.html";
    }
};

const currentUserDetailsFromToken = () => {
    let token = getToken();
    if (token) {
        let payload = token.split('.')[1];
        let decoded = JSON.parse(atob(payload));
        let userType = decoded.type;
        let userId = decoded.userId;
        return { userType, userId };
    }
    return null;
}

window.onload = () => {
    checkTokenAndRedirect()
    let { userType } = currentUserDetailsFromToken();
    if (userType === "admin") {
        document.getElementById('show-users-button').style.display = 'inline-block';
    }
    getItems();
    getUserDetails();

    if (userType === "admin") {
        const tableHeadRow = document.querySelector("table tr");
        const th = document.createElement("th");
        th.textContent = "User Details";
        tableHeadRow.appendChild(th);
    }
};

// פונקציה לקבלת פריטים
const getItems = async() => {
    try {
        const data = await apiRequest(shoeUri, 'GET');
        displayItems(data);
    } catch (error) {
        console.error('Unable to get items.', error);
    }
};

// פונקציה להוספת פריט
const addItem = async() => {
    const addNameTextbox = document.getElementById('add-name');
    const addSizeTextbox = document.getElementById('add-size');
    const addColorTextbox = document.getElementById('add-color');
    let addUserIdTextbox;
    let item = {
        name: addNameTextbox.value.trim(),
        size: parseInt(addSizeTextbox.value, 10),
        color: addColorTextbox.value.trim(),
        userId: currentUserDetails.id,
    };
    let { userType } = currentUserDetailsFromToken();
    if (userType == null)
        return;
    if (userType === "admin") {
        addUserIdTextbox = document.getElementById('add-userId');
        item.userId = parseInt(addUserIdTextbox.value, 10);
    }
    try {
        await apiRequest(shoeUri, 'POST', item);
        await getItems();
        addNameTextbox.value = '';
        addSizeTextbox.value = '';
        addColorTextbox.value = '';
        if (userType === "admin") {
            addUserIdTextbox.value = '';
        }
        closeAddItemModal();
    } catch (error) {
        console.error('Unable to add item.', error);
    }
};

const deleteItem = async(id) => {
    try {
        await apiRequest(`${shoeUri}/${id}`, 'DELETE');
        await getItems();
    } catch (error) {
        console.error('Unable to delete item.', error);
    }
};

// פונקציה לעדכון פריט
const updateItem = async() => {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        name: document.getElementById('edit-name').value.trim(),
        size: parseInt(document.getElementById('edit-size').value, 10),
        color: document.getElementById('edit-color').value.trim(),
        userId: currentUserDetails.id,
    };
    try {
        await apiRequest(`${shoeUri}/${itemId}`, 'PUT', item);
        await getItems();
        closeEditItemModal();
    } catch (error) {
        console.error('Unable to update item.', error);
    }
};

// פונקציה להצגת פריטים
const displayItems = (data) => {
    const tBody = document.getElementById('shoes');
    tBody.innerHTML = '';
    const button = document.createElement('button');

    let { userType } = currentUserDetailsFromToken();

    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `openEditItemModal(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.className = 'delete-button'; // הוספת מחלקת עיצוב
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let userDetailsBtn = button.cloneNode(false);
        userDetailsBtn.innerHTML = 'User Details';
        userDetailsBtn.setAttribute('onclick', `displayUserDetailsToItem(${item.userId})`);

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        let textNodeName = document.createTextNode(item.name);
        td1.appendChild(textNodeName);
        let td2 = tr.insertCell(1);
        let textNodeSize = document.createTextNode(item.size);
        td2.appendChild(textNodeSize);
        let td3 = tr.insertCell(2);
        let textNodeColor = document.createTextNode(item.color);
        td3.appendChild(textNodeColor);
        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);
        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);

        if (userType === "admin") {
            let td6 = tr.insertCell(5);
            td6.appendChild(userDetailsBtn);
        }
    });
    shoes = data;
};

// פונקציה לקבלת פרטי משתמש
const getUserDetails = async() => {
    let { userId } = currentUserDetailsFromToken();
    if (userId == null)
        return;

    try {
        const userDetails = await apiRequest(`${userUri}/${userId}`, 'GET');
        currentUserDetails = userDetails;
        displayUserDetails(currentUserDetails);
    } catch (error) {
        console.error('Error fetching user details:', error);
    }
};

// פונקציה להצגת פרטי משתמש בראש העמוד
const displayUserDetails = (user) => {
    document.getElementById('user-name').innerText = user.userName || '';
};

const checkAuthState = () => {
    const token = localStorage.getItem("authToken");
    const logoutButton = document.getElementById("logout-button");

    if (token) {
        logoutButton.style.display = "inline-block";
    } else {
        logoutButton.style.display = "none";
    }
};

const redirectToUsersPage = () => {
    window.location.href = "/html/user.html";
}

const openAddItemModal = async() => {
    let { userType } = currentUserDetailsFromToken();
    if (userType === "admin") {
        try {
            const data = await getAllUsers();
            const select = document.getElementById('add-userId');
            await populateUserSelect(select, data);
            select.style.display = 'inline-block';
        } catch (err) {
            console.error('שגיאה בקבלת רשימת משתמשים:', err);
        }
    }
    toggleModal('addItemModal', true);
};


const closeAddItemModal = () => {
    toggleModal('addItemModal', false);
};


// פונקציה להצגת חלון עריכה
const openEditItemModal = async(id) => {
    const shoe = shoes.find(s => s.id === id);
    if (!shoe) {
        return;
    }
    document.getElementById('edit-name').value = shoe.name || '';
    document.getElementById('edit-size').value = shoe.size || '';
    document.getElementById('edit-color').value = shoe.color || '';
    document.getElementById('edit-id').value = shoe.id || '';
    document.getElementById('editItemModal').style.display = 'block';
    let { userType } = currentUserDetailsFromToken();
    if (userType == null)
        return;
    if (userType === "admin") {
        try {
            const data = await getAllUsers();
            const select = document.getElementById('edit-item-id');
            select.innerHTML = '';
            data.forEach(user => {
                const option = document.createElement('option');
                option.value = user.id;
                option.textContent = user.userName;
                select.appendChild(option);
            });

            select.value = shoe.userId || '';
            select.setAttribute('add-userId', 'true');
            select.style.display = 'inline-block';
        } catch (err) {
            console.error('שגיאה בקבלת רשימת משתמשים:', err);
        }
    }
};

// פונקציה לסגירת חלון עריכה
const closeEditItemModal = () => {
    document.getElementById('editItemModal').style.display = 'none';
};

// פונקציה לעדכון משתמש (admin בלבד)
const updateUser = async() => {
    let { userId } = currentUserDetailsFromToken();
    if (userId == null)
        return;
    const user = {
        id: userId,
        userName: document.getElementById('edit-username').value.trim(),
        password: document.getElementById('edit-password').value.trim(),
        email: document.getElementById('edit-email').value.trim(),
    };
    try {
        await apiRequest(`${userUri}/${userId}`, 'PUT', user);
        await displayUserDetails(currentUserDetails); // עדכון הרשימה
        await getUserDetails(); // עדכון פרטי המשתמש
        closeEditUserModal(); // סגירת חלון העריכה
    } catch (error) {
        console.error('Error updating user:', error);
    }
};

// פונקציה להצגת פרטי משתמש בפריט
const displayUserDetailsToItem = async(userId) => {
    const dataUsers = await getAllUsers();
    const user = dataUsers.find(u => u.id === userId);

    if (!user) {
        console.log("User not found.");
        return;
    }

    const userDetailsDiv = document.getElementById('user-details');
    userDetailsDiv.innerHTML = `
    <p><strong>Username:</strong> ${user.userName}</p>
    <p><strong>Password:</strong> ${user.password}</p>
    <p><strong>Email:</strong> ${user.email}</p>
    <p><strong>Type:</strong> ${user.type}</p>
  `;

    document.getElementById('displayUserDetailsModal').style.display = 'block';
};

const closeDisplayUserDetailsModal = () => {
    document.getElementById('displayUserDetailsModal').style.display = 'none';
}

const createButton = (text, onClick) => {
    const button = document.createElement('button');
    button.innerText = text;
    button.onclick = onClick;
    return button;
};

const populateUserSelect = async(selectElement, data) => {
    selectElement.innerHTML = '';
    data.forEach(user => {
        const option = document.createElement('option');
        option.value = user.id;
        option.textContent = user.userName;
        selectElement.appendChild(option);
    });
};

const toggleModal = (modalId, display) => {
    document.getElementById(modalId).style.display = display ? 'block' : 'none';
};
// const apiRequest = async(url, method, body = null) => {
//     const token = getToken();
//     if (!token) throw new Error('Authentication required.');
//     const headers = {
//         'Accept': 'application/json',
//         'Content-Type': 'application/json',
//         'Authorization': `Bearer ${token}`,
//     };
//     const response = await fetch(url, { method, headers, body: body ? JSON.stringify(body) : null });
//     if (!response.ok) throw new Error(`Unable to perform ${method} request: ${response.statusText}`);
//     return await response.json();
// };
//
// const shoeUri = '/shoes';
// const userUri = '/users';
// let shoes = [];
// let currentUserDetails = {};

// const getToken = () => {
//     const token = localStorage.getItem("authToken");
//     if (!token) return null;
//     try {
//         const payload = JSON.parse(atob(token.split('.')[1]));
//         if (payload.exp > Math.floor(Date.now() / 1000)) return token;
//         localStorage.removeItem("authToken");
//     } catch {
//         localStorage.removeItem("authToken");
//     }
//     return null;
// };

// const checkTokenAndRedirect = () => {
//     const token = getToken();
//     const currentPath = window.location.pathname;
//     if (token && currentPath !== "/html/item.html") window.location.href = "/html/item.html";
//     else if (!token && currentPath !== "/html/login.html") window.location.href = "/html/login.html";
// };

// const currentUserDetailsFromToken = () => {
//     const token = getToken();
//     if (token) {
//         const payload = JSON.parse(atob(token.split('.')[1]));
//         return { userType: payload.type, userId: payload.userId };
//     }
//     return null;
// };

// window.onload = () => {
//     checkTokenAndRedirect();
//     const { userType } = currentUserDetailsFromToken();
//     if (userType === "admin") {
//         document.getElementById('show-users-button').style.display = 'inline-block';
//         document.querySelector("table tr").appendChild(createElement('th', 'User Details'));
//     }
//     getItems();
//     getUserDetails();
// };

// const getItems = async() => {
//     try {
//         const data = await apiRequest(shoeUri, 'GET');
//         displayItems(data);
//     } catch (error) {
//         console.error('Unable to get items.', error);
//     }
// };

// const addItem = async() => {
//     const item = {
//         name: document.getElementById('add-name').value.trim(),
//         size: parseInt(document.getElementById('add-size').value, 10),
//         color: document.getElementById('add-color').value.trim(),
//         userId: currentUserDetails.id,
//     };
//     const { userType } = currentUserDetailsFromToken();
//     if (userType === "admin") item.userId = parseInt(document.getElementById('add-userId').value, 10);
//     try {
//         await apiRequest(shoeUri, 'POST', item);
//         await getItems();
//         resetAddItemForm(userType);
//         closeAddItemModal();
//     } catch (error) {
//         console.error('Unable to add item.', error);
//     }
// };

// const resetAddItemForm = (userType) => {
//     document.getElementById('add-name').value = '';
//     document.getElementById('add-size').value = '';
//     document.getElementById('add-color').value = '';
//     if (userType === "admin") document.getElementById('add-userId').value = '';
// };
//
// const deleteItem = async(id) => {
//     await apiRequest(`${shoeUri}/${id}`, 'DELETE');
//     await getItems();
// };

// const updateItem = async() => {
//     const itemId = document.getElementById('edit-id').value;
//     const item = {
//         id: parseInt(itemId, 10),
//         name: document.getElementById('edit-name').value.trim(),
//         size: parseInt(document.getElementById('edit-size').value, 10),
//         color: document.getElementById('edit-color').value.trim(),
//         userId: currentUserDetails.id,
//     };
//     await apiRequest(`${shoeUri}/${itemId}`, 'PUT', item);
//     await getItems();
//     closeEditItemModal();
// };

// const displayItems = (data) => {
//     const tBody = document.getElementById('shoes');
//     tBody.innerHTML = '';
//     data.forEach(item => {
//         const tr = tBody.insertRow();
//         tr.insertCell(0).appendChild(document.createTextNode(item.name));
//         tr.insertCell(1).appendChild(document.createTextNode(item.size));
//         tr.insertCell(2).appendChild(document.createTextNode(item.color));
//         tr.insertCell(3).appendChild(createButton('Edit', () => openEditItemModal(item.id)));
//         tr.insertCell(4).appendChild(createButton('Delete', () => deleteItem(item.id)));
//         if (currentUserDetailsFromToken().userType === "admin") {
//             tr.insertCell(5).appendChild(createButton('User Details', () => displayUserDetailsToItem(item.userId)));
//         }
//     });
//     shoes = data;
// };

// const getUserDetails = async() => {
//     const { userId } = currentUserDetailsFromToken();
//     if (!userId) return;
//     currentUserDetails = await apiRequest(`${userUri}/${userId}`, 'GET');
//     displayUserDetails(currentUserDetails);
// };

// const displayUserDetails = (user) => {
//     document.getElementById('user-name').innerText = user.userName || '';
// };

// const openAddItemModal = async() => {
//     if (currentUserDetailsFromToken().userType === "admin") {
//         const data = await getAllUsers();
//         populateUserSelect(document.getElementById('add-userId'), data);
//     }
//     toggleModal('addItemModal', true);
// };

// const closeAddItemModal = () => toggleModal('addItemModal', false);

// const openEditItemModal = async(id) => {
//     const shoe = shoes.find(s => s.id === id);
//     if (!shoe) return;
//     document.getElementById('edit-name').value = shoe.name || '';
//     document.getElementById('edit-size').value = shoe.size || '';
//     document.getElementById('edit-color').value = shoe.color || '';
//     document.getElementById('edit-id').value = shoe.id || '';
//     const { userType } = currentUserDetailsFromToken();
//     if (userType === "admin") {
//         const data = await getAllUsers();
//         populateUserSelect(document.getElementById('edit-item-id'), data);
//         document.getElementById('edit-item-id').value = shoe.userId || '';
//     }
//     toggleModal('editItemModal', true);
// };

// const closeEditItemModal = () => toggleModal('editItemModal', false);

// const displayUserDetailsToItem = async(userId) => {
//     const user = (await getAllUsers()).find(u => u.id === userId);
//     if (!user) return console.log("User not found.");
//     const userDetailsDiv = document.getElementById('user-details');
//     userDetailsDiv.innerHTML = `
//         <p><strong>Username:</strong> ${user.userName}</p>
//         <p><strong>Password:</strong> ${user.password}</p>
//         <p><strong>Email:</strong> ${user.email}</p>
//         <p><strong>Type:</strong> ${user.type}</p>
//     `;
//     toggleModal('displayUserDetailsModal', true);
// };

// const closeDisplayUserDetailsModal = () => toggleModal('displayUserDetailsModal', false);

// const createButton = (text, onClick) => {
//     const button = document.createElement('button');
//     button.innerText = text;
//     button.onclick = onClick;
//     return button;
// };

// const populateUserSelect = (selectElement, data) => {
//     selectElement.innerHTML = '';
//     data.forEach(user => {
//         const option = document.createElement('option');
//         option.value = user.id;
//         option.textContent = user.userName;
//         selectElement.appendChild(option);
//     });
// };

// const toggleModal = (modalId, display) => {
//     document.getElementById(modalId).style.display = display ? 'block' : 'none';
// };
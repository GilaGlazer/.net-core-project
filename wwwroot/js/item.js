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
};


// פונקציה לקבלת פריטים
const getItems = async () => {
  const token = getToken();
  if (!token)
    return;
  try {
    const response = await fetch(shoeUri, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });
    const data = await response.json();
    displayItems(data);
  } catch (error) {
    console.error('Unable to get items.', error);
  }
};

// פונקציה להוספת פריט
const addItem = async () => {
  const token = getToken();
  if (!token)
    return;
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
    addUserIdTextbox.setAttribute('add-userId', 'true');
    item.userId = parseInt(addUserIdTextbox.value, 10);
  }
  try {
    await fetch(shoeUri, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(item),
    });
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

const deleteItem = async (id) => {
  const token = getToken();
  if (!token) return;
  try {
    await fetch(`${shoeUri}/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });
    getItems();
  } catch (error) {
    console.error('Unable to delete item.', error);
  }
};



// פונקציה לעדכון פריט
const updateItem = async () => {
  const token = getToken();
  if (!token) return;
  const itemId = document.getElementById('edit-id').value;
  const item = {
    id: parseInt(itemId, 10),
    name: document.getElementById('edit-name').value.trim(),
    size: parseInt(document.getElementById('edit-size').value, 10),
    color: document.getElementById('edit-color').value.trim(),
    userId: currentUserDetails.id,
  };

  try {
    await fetch(`${shoeUri}/${itemId}`, {
      method: 'PUT',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(item),
    })
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

  data.forEach(item => {
    let editButton = button.cloneNode(false);
    editButton.innerText = 'Edit';
    editButton.setAttribute('onclick', `openEditUserModal(${item.id})`);
    let deleteButton = button.cloneNode(false);
    deleteButton.innerText = 'Delete';
    deleteButton.className = 'delete-button'; // הוספת מחלקת עיצוב
    deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);
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
  });
  shoes = data;
};

// פונקציה לקבלת פרטי משתמש
const getUserDetails = async () => {
  const token = getToken();
  if (!token)
    return;
  try {
    const response = await fetch(`${userUri}/2`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });
    if (response.ok) {
      currentUserDetails = await response.json();
      displayUserDetails(currentUserDetails);
    } else {
      console.error('Unable to fetch user details.');
    }
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
  //const loginButton = document.getElementById("login-button");
  const logoutButton = document.getElementById("logout-button");

  if (token) {
    // משתמש מחובר
    logoutButton.style.display = "inline-block";
    // loginButton.style.display = "none";
  } else {
    // משתמש לא מחובר
    //loginButton.style.display = "inline-block";
    logoutButton.style.display = "none";
  }
};


const redirectToUsersPage = () => {
  window.location.href = "/html/user.html";
}


const openAddItemModal = () => {
  let { userType } = currentUserDetailsFromToken();
  if (userType === "admin") {
    addUserIdTextbox = document.getElementById('add-userId');
    addUserIdTextbox.setAttribute('add-userId', 'true');
    addUserIdTextbox.style.display = 'inline-block';
  }
  document.getElementById('addItemModal').style.display = 'block'
};

const closeAddItemModal = () => {
  document.getElementById('addItemModal').style.display = 'none';
};
// פונקציה להצגת חלון עריכה
const openEditUserModal = (id) => {
  const shoe = shoes.find(s => s.id === id);
  if (!shoe) {
    return;
  }
  document.getElementById('edit-name').value = shoe.name || '';
  document.getElementById('edit-size').value = shoe.size || '';
  document.getElementById('edit-color').value = shoe.color || '';
  document.getElementById('edit-id').value = shoe.id || '';
  document.getElementById('editItemModal').style.display = 'block';
};

// פונקציה לסגירת חלון עריכה
const closeEditItemModal = () => {
  document.getElementById('editItemModal').style.display = 'none';
};

// פונקציה להצגת חלון עריכה
const openEditUserModalByHimSelf = () => {
  const user = currentUserDetails;
  if (!user) {
    console.log("User not found.");
    return;
  }
  document.getElementById('edit-username').value = user.userName || '';
  document.getElementById('edit-email').value = user.email || '';
  document.getElementById('edit-password').value = user.password || '';
  document.getElementById('editUserModal').style.display = 'block';
};

// פונקציה לסגירת חלון עריכה
const closeEditUserModal = () => {
  document.getElementById('editUserModal').style.display = 'none';
};

// פונקציה לעדכון משתמש (admin בלבד)
const updateUser = async () => {
  let { userId } = currentUserDetailsFromToken();
  if(userId == null)
    return;
  const user = {
    id: userId,
    userName: document.getElementById('edit-username').value.trim(),
    password: document.getElementById('edit-password').value.trim(),
    email: document.getElementById('edit-email').value.trim(),
  };
  try {
    const token = getToken();
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

    await displayUserDetails(currentUserDetails); // עדכון הרשימה
    await getUserDetails(); // עדכון פרטי המשתמש
    closeEditUserModal(); // סגירת חלון העריכה
  } catch (error) {
    console.error('Error updating user:', error);
  }
};
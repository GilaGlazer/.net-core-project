const userUri = '/users'; // עדכון ה-URI למשתמשים
let users = []; // מערך למשתמשים

function getUsers() {
    fetch(userUri)
        .then(response => response.json())
        .then(data => _displayUsers(data))
        .catch(error => console.error('Unable to get users.', error));
}

function addUser() {
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

    fetch(userUri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        })
        .then(response => response.json())
        .then(() => {
            getUsers();
            addUsernameTextbox.value = '';
            addPasswordTextbox.value = '';
            addEmailTextbox.value = '';
            addTypeTextbox.value = '';
        })
        .catch(error => console.error('Unable to add user.', error));
}

function deleteUser(id) {
    fetch(`${userUri}/${id}`, {
            method: 'DELETE'
        })
        .then(() => getUsers())
        .catch(error => console.error('Unable to delete user.', error));
}

function displayEditUserForm(id) {
    const user = users.find(user => user.id === id);

    document.getElementById('edit-username').value = user.userName;
    document.getElementById('edit-email').value = user.email;
    document.getElementById('edit-type').value = user.type;
    document.getElementById('edit-id').value = user.id;
    document.getElementById('editUserForm').style.display = 'block';
}

function updateUser() {
    const userId = document.getElementById('edit-id').value;
    const user = {
        id: parseInt(userId, 10),
        userName: document.getElementById('edit-username').value.trim(),
        password: document.getElementById('edit-password').value.trim(),
        email: document.getElementById('edit-email').value.trim(),
        type: document.getElementById('edit-type').value.trim()
    };

    fetch(`${userUri}/${userId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        })
        .then(() => getUsers())
        .catch(error => console.error('Unable to update user.', error));

    closeUserInput();
    return false;
}

function closeUserInput() {
    document.getElementById('editUserForm').style.display = 'none';
}

function _displayUserCount(userCount) {
    const name = (userCount === 1) ? 'user' : 'users';
    document.getElementById('user-counter').innerText = `${userCount} ${name}`;
}

function _displayUsers(data) {
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
        let textNodeType = document.createTextNode(user.type);
        td3.appendChild(textNodeType);

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });

    users = data; // עדכון המערך ל-users
}

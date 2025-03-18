const uri = '/shoes'; // עדכון ה-URI לנעליים
let shoes = []; // שינוי השם מ-pizzas ל-shoes

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const addSizeTextbox = document.getElementById('add-size');
    const addColorTextbox = document.getElementById('add-color');

    const item = {
        name: addNameTextbox.value.trim(),
        size: parseInt(addSizeTextbox.value, 10),
        color: addColorTextbox.value.trim()
    };

    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addSizeTextbox.value = '';
            addColorTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE'
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = shoes.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-size').value = item.size;
    document.getElementById('edit-color').value = item.color;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        name: document.getElementById('edit-name').value.trim(),
        size: parseInt(document.getElementById('edit-size').value, 10),
        color: document.getElementById('edit-color').value.trim()
    };

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();
    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'shoe' : 'shoes'; // עדכון שם היחידה
    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('shoes'); // שינוי ה-ID ל-shoes
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
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

    shoes = data; // עדכון המערך ל-shoes
}

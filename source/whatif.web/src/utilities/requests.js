const baseUrl = `http://localhost:3602/v1.0/invoke/whatifapi/method`;
export async function postData(url = '', data = {}) {
    const response = await fetch(`${baseUrl}/${url}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        body: JSON.stringify(data)
    });
    return response.json();
}

export async function getData(url = '') {
    const response = await fetch(`${baseUrl}/${url}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
    });
    return response.json();
}

export async function login(username, password) {
    const response = await fetch('http://localhost:3602/v1.0/invoke/whatifapi/method/users/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: username, password: password })
    });
    return response.json();
}

export async function logout() {
    
}
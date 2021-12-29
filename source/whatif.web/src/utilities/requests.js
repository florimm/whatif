export async function postData(url = '', data = {}) {
    const response = await fetch(url, {
        method: 'POST',
        // mode: 'cors', // no-cors, *cors, same-origin
        // cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        // credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        body: JSON.stringify(data)
    });
    return response.json();
}

export async function login(username, password) {
    const response = await fetch('http://localhost:3602/v1.0/invoke/whatifapi/method/user/login', {
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
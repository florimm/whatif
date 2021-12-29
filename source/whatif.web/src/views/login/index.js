import {
    useNavigate,
    useLocation,
} from "react-router-dom";
import { useAuth } from "./RequireAuth";

function Login() {
    let navigate = useNavigate();
    let location = useLocation();
    let auth = useAuth();

    let from = location.state?.from?.pathname || "/";
    const tryToLogin = (event) => {
        event.preventDefault();
        const data = new FormData(event.target)
        const formObject = Object.fromEntries(data.entries());
        console.log(formObject);
        console.log('auth', auth);
        auth.signIn(formObject.email, formObject.password)
            .then(response => {
                console.log('response', response);
                localStorage.setItem('token', response.token);
                localStorage.setItem('userName', response.userName);
                navigate(from, { replace: true });
            }).catch(error => {
                console.log('error', error);
            });
    }
    if (auth.user && auth.token) {
        return (
        <div>
            <h1>You are logged in as {auth.user}</h1>
        </div>
        )
    }
    return (
        <div>
            <h1>Login</h1>
            <form onSubmit={tryToLogin}>
                <div className="form-group">
                    <label htmlFor="email">Email address</label>
                    <input type="email" className="form-control" name="email" id="email" aria-describedby="emailHelp" placeholder="Enter email" />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Password</label>
                    <input type="password" className="form-control" name="password" id="password" placeholder="Password" />
                </div>
                <button type="submit" className="btn btn-primary">Submit</button>
            </form>
        </div>
    );
}

export default Login;

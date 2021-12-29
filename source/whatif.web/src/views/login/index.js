import {
    useNavigate,
    useLocation,
} from "react-router-dom";
import { useAuth } from "../../components/useAuth";

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
        <main className="form-signin">
        <form onSubmit={tryToLogin}>
            {/* <img className="mb-4" src="../assets/brand/bootstrap-logo.svg" alt="" width="72" height="57" /> */}
            <h1 className="h3 mb-3 fw-normal">Please sign in</h1>
            <div className="form-floating">
            <input type="email" className="form-control" id="floatingInput" name="email" placeholder="name@example.com" />
            <label htmlFor="floatingInput">Email address</label>
            </div>
            <div className="form-floating">
            <input type="password" className="form-control" id="floatingPassword" name="password" placeholder="Password" />
            <label htmlFor="floatingPassword">Password</label>
            </div>
            <button className="w-100 btn btn-lg btn-primary" type="submit">Sign in</button>
            <p className="mt-5 mb-3 text-muted">&copy; 2017–2021</p>
        </form>
        </main>
    );
}

export default Login;

import { Routes, Route, Link, useNavigate } from "react-router-dom";
import {
  QueryClient,
  QueryClientProvider,
} from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools'
import Home from './views/home';
import Wallets from './views/wallets';
import About from './views/about';
import Login from './views/login';
import Profile from './views/profile';
import { RequireAuth } from './components/RequireAuth';
import { useAuth } from './components/useAuth';
import NotFound from './views/not-found';
import './App.css';

function App() {
  const queryClient = new QueryClient();
  const auth = useAuth();
  const navigate = useNavigate();

  const isUserAuth = auth.isTokenValid();

  const signOut = () => {
    return auth.signOut(() => {
      navigate('/login');
    });
  };

  return (
    <QueryClientProvider client={queryClient}>
      <nav className="navbar navbar-expand-md navbar-dark fixed-top bg-dark">
        <div className="container-fluid">
          <Link className="navbar-brand" to="/">what-if</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarCollapse" aria-controls="navbarCollapse" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarCollapse">
            <ul className="navbar-nav me-auto mb-2 mb-md-0">
              <li className="nav-item">
                <Link className="nav-link" to="/">Home</Link>
              </li>
              {
                isUserAuth &&
                <li className="nav-item">
                  <Link className="nav-link" to="/profile">Profile</Link>
                </li>
              }
              {
                isUserAuth &&
                <li className="nav-item">
                  <Link className="nav-link" to="/wallets">Wallets</Link>
                </li>
              }
              <li className="nav-item">
                <Link className="nav-link" to="/about">About</Link>
              </li>
            </ul>
            {
              isUserAuth ? (
                <form className="d-flex">
                  <button className="btn btn-outline-danger" onClick={signOut} >Logout</button>
                </form>
              ) : (
                <form className="d-flex">
                  <button className="btn btn-outline-primary" onClick={() => navigate('/login')} >Login</button>
                </form>
              )
            }
          </div>
        </div>
      </nav>
        <main className="container">
          <div className="bg-light p-5 rounded">            
              <Routes>
                <Route path="/">
                  <Route index element={<Home />} />
                  <Route path="about" element={<About />} />
                  <Route path="profile" element={<RequireAuth><Profile /></RequireAuth>} />
                  <Route path="wallets/*" element={<RequireAuth><Wallets /></RequireAuth>} />
                  <Route path="login*" element={<Login />} />
                  <Route path="*" element={<NotFound />} />
                </Route>
              </Routes>
          </div>
        </main>
        <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App;

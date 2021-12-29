import * as React from "react";
import {
  useLocation,
  Navigate
} from "react-router-dom";
import { login, logout } from "../../utilities/requests";

export function RequireAuth({ children }) {
    let auth = useAuth();
    let location = useLocation();
  
    if (!auth.user) {
      // Redirect them to the /login page, but save the current location they were
      // trying to go to when they were redirected. This allows us to send them
      // along to that page after they login, which is a nicer user experience
      // than dropping them off on the home page.
      return <Navigate to="/login" state={{ from: location }} replace />;
    }
  
    return children;
  }

  let AuthContext = React.createContext();

  export function AuthProvider({ children }) {
    let [user, setUser] = React.useState(localStorage.getItem('userName'));
    let [token, setToken] = React.useState(localStorage.getItem('token'));
  
    let signIn = (email, password) => {
      return login(email, password).then(response => {
        setUser(response.userName);
        setToken(response.token);
        return response;
      });
    };
  
    let signOut = (callback) => {
      setUser(null);
      setToken(null);
      return callback;
    };
  
    const value = { user, token, signIn, signOut };
  
    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
  }

  export function useAuth() {
    return React.useContext(AuthContext);
  }
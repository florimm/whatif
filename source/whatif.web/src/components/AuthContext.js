import * as React from "react";
import { login } from "../utilities/requests";
import jwtDecode from "jwt-decode";

export const AuthContext = React.createContext();

export function AuthProvider({ children }) {
    const [user, setUser] = React.useState(localStorage.getItem('userName'));
    const [token, setToken] = React.useState(localStorage.getItem('token'));
  
    const signIn = (email, password) => {
      return login(email, password).then(response => {
        setUser(response.userName);
        setToken(response.token);
        return response;
      });
    };
  
    const signOut = (callback) => {
      setUser(null);
      setToken(null);
      localStorage.removeItem('userName');
      localStorage.removeItem('token');
      return callback;
    };

    const isTokenValid = () => {
      if (!token) {
        return false;
      }
      const decodedJwt = jwtDecode(token);
      const tokenExpireDate = decodedJwt.exp * 1000;
      const result = tokenExpireDate > Date.now();
      return result;
    }
  
    const value = { user, token, signIn, signOut, isTokenValid };
  
    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
import {
  useLocation,
  Navigate
} from "react-router-dom";
import { useAuth } from "./useAuth";

export function RequireAuth({ children }) {
    const auth = useAuth();
    const location = useLocation();
  
    if (!auth.user) {
      return <Navigate to="/login" state={{ from: location }} replace />;
    }
  
    return children;
}
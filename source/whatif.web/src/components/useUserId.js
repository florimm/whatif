import jwt from 'jwt-decode';
import { useAuth } from './useAuth';

export function useUserId() {
    const auth = useAuth();
    const jwtData = jwt(auth.token);
    const userId = jwtData['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    return userId;
}
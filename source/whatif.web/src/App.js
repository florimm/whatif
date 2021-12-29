import { Routes, Route } from "react-router-dom";
import {
  QueryClient,
  QueryClientProvider,
} from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools'
import Home from './views/home';
import Wallets from './views/wallets';
import About from './views/about';
import Login from './views/login';
import { RequireAuth, AuthProvider } from './views/login/RequireAuth';
import NotFound from './views/not-found';
import './App.css';

function App() {
  const queryClient = new QueryClient();
  return (
    <QueryClientProvider client={queryClient}>
        <div className="App">
          <div className="App-intro">
            <AuthProvider>
              <Routes>
                <Route path="/">
                  <Route index element={<Home />} />
                  <Route path="login" element={<Login />} />
                  <Route path="about" element={<About />} />
                  <Route path="wallets/*" element={<RequireAuth><Wallets /></RequireAuth>} />
                  <Route path="*" element={<NotFound />} />
                </Route>
              </Routes>
            </AuthProvider>
          </div>
        </div>
        <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App;

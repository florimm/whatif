import { Routes, Route } from "react-router-dom";
import Home from './views/home';
import Wallets from './views/wallets';
import About from './views/about';
import NotFound from './views/not-found';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1 className="App-title">Welcome to What-if</h1>
      </header>
      <div className="App-intro">
        <Routes>
        <Route path="/">
          <Route index element={<Home />} />
          <Route path="about" element={<About />} />
          <Route path="wallets/*" element={<Wallets />} />
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
      </div>
    </div>
  );
}

export default App;

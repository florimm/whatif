import { Routes, Route } from "react-router-dom";
import Home from './views/home';
import Wallets from './views/wallets';
import About from './views/about';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1 className="App-title">Welcome to React</h1>
      </header>
      <div className="App-intro">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/wallets" element={<Wallets />} />
          <Route path="/about" element={<About />} />
        </Routes>
      </div>
    </div>
  );
}

export default App;

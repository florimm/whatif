import { Routes, Route } from "react-router-dom";
import Home from './Home';
import LoginForm from './LoginForm';

function View() {
    return (
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="login" element={<LoginForm />} />
      </Routes>
    );
}

export default View;

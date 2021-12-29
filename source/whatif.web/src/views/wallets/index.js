import { Routes, Route } from "react-router-dom";
import List from './List';
import Details from './Details';
import Create from './Create';

function View() {
    return (
      <Routes>
        <Route path="/" element={<List />} />
        <Route path="/new" element={<Create />} />
        <Route path="/:walletId" element={<Details />} />
      </Routes>
    );
}

export default View;

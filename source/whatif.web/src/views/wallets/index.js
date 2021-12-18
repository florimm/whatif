import { Routes, Route } from "react-router-dom";
import List from './List';
import Details from './Details';

function View() {
    return (
      <Routes>
        <Route path="/" element={<List />} />
        <Route path="/:id" element={<Details />} />
      </Routes>
    );
}

export default View;

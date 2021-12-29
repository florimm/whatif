import { Routes, Route } from "react-router-dom";
import List from './List';
import Details from './Details';
import Create from './Create';
import CreateInvestment from './CreateInvestment';

function View() {
    return (
      <Routes>
        <Route path="/" element={<List />} />
        <Route path="/new" element={<Create />} />
        <Route path="/:walletId" element={<Details />} />
        <Route path="/:walletId/new" element={<CreateInvestment />} />
      </Routes>
    );
}

export default View;

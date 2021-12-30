import { Routes, Route } from "react-router-dom";
import List from './List';
import Details from './Details';

export default function WalletRouts() {
    return (
      <Routes>
        <Route path="/" element={<List />} />
        <Route path="/:walletId" element={<Details />} />
      </Routes>
    );
}

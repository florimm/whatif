import { useQuery } from "react-query";
import { Link } from "react-router-dom";

export default function List() {
    const { isLoading, data } = useQuery('walletList', async () => {
        const response = await Promise.resolve([{ name: 'Main', id: 1 }, { name: 'Secondary', id: 2 }]);
        return response;
    })

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h1>List</h1>
            <ul className="list-group">
                {data.map(wallet => (<li key={wallet.id}> <Link to={`/wallets/${wallet.id}`}>{wallet.name}</Link></li>))}
            </ul>
        </div>
    );
}
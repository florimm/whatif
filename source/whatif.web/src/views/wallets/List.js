import { useQuery, useQueryClient } from "react-query";
import { Link } from "react-router-dom";
import PullToRefresh from 'react-simple-pull-to-refresh';

export default function List() {
    const queryClient = useQueryClient();
    const { isLoading, data } = useQuery('walletList', async () => {
        const response = await Promise.resolve([{ name: 'Main', id: 1 }, { name: 'Secondary', id: 2 }]);
        return response;
    });

    const handleRefresh = () => {
        return queryClient.invalidateQueries('walletList');
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <PullToRefresh onRefresh={handleRefresh}>
            <h1>List</h1>
            <ul className="list-group">
                {data.map(wallet => (<li key={wallet.id}> <Link to={`/wallets/${wallet.id}`}>{wallet.name}</Link></li>))}
            </ul>
        </PullToRefresh>
    );
}
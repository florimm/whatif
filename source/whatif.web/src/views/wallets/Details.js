import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient } from "react-query";
import PullToRefresh from 'react-simple-pull-to-refresh';

export default function Details() {
    const { walletId } = useParams();
    const queryClient = useQueryClient();

    const { isLoading, data } = useQuery(['walletDetail', walletId], async (data) => {
        console.log('data =>', data);
        const response = await Promise.resolve({
            id: walletId,
            name: walletId === '1' ? 'Main wallet' : 'Secondary wallet',
            pairs: [{ pair: 'USDTETH', value: 4000 }, { pair: 'USDTBTC', value: 77000 }]
        });
        return response;
    });

    const handleRefresh = () => {
        return queryClient.invalidateQueries(['walletDetail', walletId]);
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <PullToRefresh onRefresh={handleRefresh}>
            <h1>Wallet: {data.name}</h1>                
            <ul className="list-group">
                {data.pairs.map(pair => (<li key={pair.pair}> {pair.pair} {pair.value}</li>))}
            </ul>
            <Link to="..">Back</Link>
        </PullToRefresh>
    );
}
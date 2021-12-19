import { useParams, Link } from "react-router-dom";
import { useQuery } from "react-query";

export default function Details() {
    const { walletId } = useParams();
    const { isLoading, data } = useQuery(['walletDetail', walletId], async () => {
        const response = await Promise.resolve({
            id: walletId,
            name: walletId === '1' ? 'Main wallet' : 'Secondary wallet',
            pairs: [{ pair: 'USDTETH', value: 4000 }, { pair: 'USDTBTC', value: 77000 }]
        });
        return response;
    });

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h1>Wallet: {data.name}</h1>
            <ul className="list-group">
                {data.pairs.map(pair => (<li key={pair.pair}> {pair.pair} {pair.value}</li>))}
            </ul>
            <Link to="..">Back</Link>
        </div>
    );
}
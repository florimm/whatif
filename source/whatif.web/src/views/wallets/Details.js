import React from 'react';
import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient } from "react-query";
import PullToRefresh from 'react-simple-pull-to-refresh';
import { Icon } from '@iconify/react';

export default function Details() {
    const { walletId } = useParams();
    const queryClient = useQueryClient();

    const { isLoading, data } = useQuery(['walletDetail', walletId], async (data) => {
        console.log('data =>', data);
        const response = await Promise.resolve({
            id: walletId,
            name: walletId === '1' ? 'Main wallet' : 'Secondary wallet',
            pairs: [{ pair: 'USDTETH', value: 4000, from:"usdt", to: "eth" }, { pair: 'USDTBTC', value: 77000, from:"usdt", to: "btc" }]
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
            {
                data.pairs.map(pair => (<Link key={pair.pair} to={`/investment/${pair.id}`} className="list-group-item list-group-item-action d-flex gap-3 py-3" aria-current="true">
                <Icon icon={`cryptocurrency:${pair.from}`} /><Icon icon={`cryptocurrency:${pair.to}`} />
                <div className="d-flex gap-2 w-100 justify-content-between">
                <div>
                    <h6 className="mb-0">{pair.pair}</h6>
                    <p className="mb-0 opacity-75">{pair.description || ''}</p>
                </div>
                <small className="opacity-50 text-nowrap">{pair.value} $</small>
                </div>
            </Link>
                ))}
            <Link to="..">Back</Link>
        </PullToRefresh>
    );
}
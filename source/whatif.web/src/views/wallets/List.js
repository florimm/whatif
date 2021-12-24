import { useQuery, useQueryClient } from "react-query";
import { Link } from "react-router-dom";
import { Icon } from '@iconify/react';
import PullToRefresh from "react-simple-pull-to-refresh";

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
            <div className="list-group">
            <h1>Wallet list</h1>
            {
                data.map(wallet => (
                <Link key={wallet.id} to={`/wallets/${wallet.id}`} className="list-group-item list-group-item-action d-flex gap-3 py-3" aria-current="true">
                    <Icon icon="ant-design:wallet-filled" />
                    <div className="d-flex gap-2 w-100 justify-content-between">
                    <div>
                        <h6 className="mb-0">{wallet.name}</h6>
                        <p className="mb-0 opacity-75">{wallet.description || ''}</p>
                    </div>
                    <small className="opacity-50 text-nowrap">2000$</small>
                    </div>
                </Link>
            ))}
            </div>
        </PullToRefresh>
    );
}
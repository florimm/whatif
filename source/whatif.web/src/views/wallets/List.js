import { useQuery, useQueryClient } from "react-query";
import { Link, useNavigate } from "react-router-dom";
import { Icon } from '@iconify/react';
import PullToRefresh from "react-simple-pull-to-refresh";

export default function List() {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
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

    const addNewWallet = () => {
        navigate('/wallets/new');
    };

    return (
        <PullToRefresh onRefresh={handleRefresh}>
            <div className="list-group">
                <div className="d-flex bd-highlight">
                    <div className="p-2 w-100 bd-highlight"><h3>Wallet list</h3></div>
                    <div className="p-2 flex-shrink-1 bd-highlight">
                        <button className="btn btn-light" onClick={addNewWallet}>
                            <Icon style={{ fontSize: '24px' }} icon="ant-design:folder-add-filled" />
                        </button>
                    </div>
                </div>
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
import { useState } from 'react';
import { useQuery, useQueryClient, useMutation } from "react-query";
import { Link, useParams } from "react-router-dom";
import { Icon } from '@iconify/react';
import { Modal, Button } from 'react-bootstrap';
import PullToRefresh from "react-simple-pull-to-refresh";
import WalletForm from "./components/WalletForm";
import { getData, postData } from "../../utilities/requests";
import { useUserId } from '../../components/useUserId';

export default function List() {
    const userId = useUserId();
    const [show, setShow] = useState(false);
    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const queryClient = useQueryClient();

    const { isLoading, data } = useQuery(['walletList', userId], () => {
        return getData(`users/${userId}/wallets`);
    });

    const mutation = useMutation(
        wallet => postData(`users/${userId}/wallets`, wallet), {
        onSuccess: () => {
            queryClient.invalidateQueries('walletList');
            handleClose();
        },
    });

    const saveWallet = wallet => {
        mutation.mutate({ ...wallet });
        handleClose();
    }

    const handleRefresh = () => {
        return queryClient.invalidateQueries('walletList');
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <>
        <PullToRefresh onRefresh={handleRefresh}>
            <div className="list-group">
                <div className="d-flex bd-highlight">
                    <div className="p-2 w-100 bd-highlight"><h3>Wallet list</h3></div>
                    <div className="p-2 flex-shrink-1 bd-highlight">
                        <button className="btn btn-light" onClick={handleShow}>
                            <Icon style={{ fontSize: '24px' }} icon="ant-design:folder-add-filled" />
                        </button>
                    </div>
                </div>
            {
                data?.map(wallet => (
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
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
            <Modal.Title>Create new wallet</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <WalletForm onSave={saveWallet} onCancel={handleClose} />
            </Modal.Body>
        </Modal>
        </>
    );
}
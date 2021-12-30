import React, { useState } from 'react';
import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient, useMutation } from "react-query";
import PullToRefresh from 'react-simple-pull-to-refresh';
import { Modal } from 'react-bootstrap';
import { getData, postData } from "../../utilities/requests";
import { useUserId } from '../../components/useUserId';
import InvestmentRow from './components/InvestmentRow';

export default function Details() {
    const userId = useUserId();
    const [show, setShow] = useState(false);

    const handleClose = () => setShow(false);
    const onEditWallet = () => {
        setShow(true);
    };
    const { walletId } = useParams();
    const queryClient = useQueryClient();

    const { isLoading, data } = useQuery(['walletDetail', walletId], () => {
        return getData(`users/${userId}/wallets/${walletId}`);
    });

    const mutation = useMutation(
        wallet => postData(`users/${userId}/wallets/${walletId}`, wallet), {
        onSuccess: () => {
            queryClient.invalidateQueries('walletList');
            queryClient.invalidateQueries(['walletDetail', walletId]);
            handleClose();
        },
    });

    const saveWallet = wallet => {
        mutation.mutate({ ...wallet });
    }

    const handleRefresh = () => {
        return queryClient.invalidateQueries(['walletDetail', walletId]);
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <>
            <PullToRefresh onRefresh={handleRefresh}>
                <h1>Wallet: {data.name}</h1><button onClick={onEditWallet}>Edit</button>
                <div className="list-group-item list-group-item-action d-flex gap-4 py-3" aria-current="true">
                    <div className="d-flex gap-2 w-100 justify-content-between">
                        <div>
                            <h6 className="mb-0">Pairs</h6>
                        </div>
                        <small className="opacity-50 text-nowrap">Investment</small>
                        <small className="opacity-50 text-nowrap">Holdings</small>
                    </div>
                </div>
                {
                    data.pairs.map(pair => <InvestmentRow key={pair.pair} pair={pair} />)
                }
                <Link to="..">Back</Link>
            </PullToRefresh>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Edit wallet</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <WalletForm currentWallet={{ walletId: data.walletId, name: data.name }} onSave={saveWallet} onCancel={handleClose} />
                </Modal.Body>
            </Modal>
        </>
    );
}


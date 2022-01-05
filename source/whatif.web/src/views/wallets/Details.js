import React, { useState } from 'react';
import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient, useMutation } from "react-query";
import PullToRefresh from 'react-simple-pull-to-refresh';
import { Modal } from 'react-bootstrap';
import { Icon } from '@iconify/react';
import WalletForm from "./components/WalletForm";
import InvestForm from './components/InvestForm';
import { getData, postData } from "../../utilities/requests";
import InvestmentRow from './components/InvestmentRow';
import { queryKeys } from '../../constants';

export default function Details() {
    const [modalForm, setModalForm] = useState(null);
    const [ selectedInvestment, setSelectedInvestment ] = useState(null);
    const { walletId } = useParams();
    const queryClient = useQueryClient();

    const handleClose = () => setModalForm(null);
    const onEditWallet = () => {
        setModalForm('wallet');
    };

    const onEditInvestment = () => {
        setModalForm('investment');
    };

    const onAddInvestment = () => {
        setModalForm('investment');
    };

    const onSelectInvestment = (investment) => {
        setSelectedInvestment(investment);
    }

    // const onAddInvestment = () => {
    //     addInvestmentMutation.mutate({
    //         walletId: walletId, 
    //         From: 'eth', 
    //         To: 'usdt', 
    //         Value: 300,
    //         Amount: 1
    //     });
    // }

    const { isLoading, data } = useQuery(
        [queryKeys.wallet.detail, walletId],
        () => getData(`wallets/${walletId}/investments`)
    );

    const mutation = useMutation(
        wallet => postData(`wallets/${walletId}`, wallet), {
        onSuccess: () => {
            queryClient.invalidateQueries(queryKeys.wallet.list);
            queryClient.invalidateQueries([queryKeys.wallet.detail, walletId]);
            handleClose();
        },
    });

    const addInvestmentMutation = useMutation(
        investment => postData(`wallets/${walletId}/investments`, investment), {
        onSuccess: () => {
            queryClient.invalidateQueries(queryKeys.wallet.list);
            queryClient.invalidateQueries([queryKeys.wallet.detail, walletId]);
            handleClose();
        },
    });

    const saveWallet = wallet => {
        mutation.mutate({ ...wallet, id: walletId });
    }

    const saveInvestment = investment => {
        addInvestmentMutation.mutate({ ...investment });
    }

    const handleRefresh = () => {
        postData(`wallets/refresh`).then(() => {
            queryClient.invalidateQueries([queryKeys.wallet.detail, walletId]);
        });
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    console.log('data', data);

    return (
        <>
            <PullToRefresh onRefresh={handleRefresh}>
            <div className="list-group">
                <div className="d-flex bd-highlight">
                    <div className="p-2 w-100 bd-highlight"><h3>Wallet list</h3></div>
                    <div className="d-flex p-2 flex-shrink-1 bd-highlight">
                        <button className="btn btn-light" onClick={onEditWallet}>
                            <Icon style={{ fontSize: '24px' }} icon="ant-design:edit-filled" />
                        </button>
                        <button className="btn btn-light" onClick={onAddInvestment}>
                            <Icon style={{ fontSize: '24px' }} icon="ant-design:file-add-filled" />
                        </button>
                    </div>
                </div>
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
                    data?.investments.map(investment => <InvestmentRow key={investment.pair} investment={investment} onSelect={onSelectInvestment} />)
                }
                </div>
                <Link to="..">Back</Link>
            </PullToRefresh>
            <Modal show={modalForm !== null} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>{modalForm}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {
                        modalForm === 'wallet' && <WalletForm currentWallet={{ walletId: data.walletId, name: data.name }} onSave={saveWallet} onCancel={handleClose} />
                    }
                    {
                        modalForm === 'investment' && <InvestForm currentInvestment={{ walletId: data.walletId, ...selectedInvestment }} onSave={saveInvestment} onCancel={handleClose} />
                    }
                </Modal.Body>
            </Modal>
        </>
    );
}


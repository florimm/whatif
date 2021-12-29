import React, { useState } from 'react';
import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient } from "react-query";
import PullToRefresh from 'react-simple-pull-to-refresh';
import { Modal, Button } from 'react-bootstrap';
import { Icon } from '@iconify/react';

export default function Details() {
    const [show, setShow] = useState(false);

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const { walletId } = useParams();
    const queryClient = useQueryClient();

    const { isLoading, data } = useQuery(['walletDetail', walletId], async () => {
        const response = await Promise.resolve({
            id: walletId,
            name: walletId === '1' ? 'Main wallet' : 'Secondary wallet',
            pairs: [{
                pair: 'USDTETH',
                value: 4000,
                amount: 1,
                from: "usdt",
                to: "eth"
            },
            {
                pair: 'USDTBTC',
                value: 77000,
                amount: 0.2,
                from: "usdt",
                to: "btc"
            }]
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
        <>
        <PullToRefresh onRefresh={handleRefresh}>
            <h1>Wallet: {data.name}</h1><button onClick={handleShow}>Edit</button>
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
            <Modal.Title>Modal heading</Modal.Title>
            </Modal.Header>
            <Modal.Body>Woohoo, you're reading this text in a modal!</Modal.Body>
            <Modal.Footer>
            <Button variant="secondary" onClick={handleClose}>
                Close
            </Button>
            <Button variant="primary" onClick={handleClose}>
                Save Changes
            </Button>
            </Modal.Footer>
        </Modal>
        </>
    );
}

function InvestmentRow({ pair }) {
    const { data: price } = useQuery(['price', pair.pair], async (data) => {
        console.log('geting data', data.queryKey[1]);
        const value = Math.random() * 20;
        return Promise.resolve(value.toFixed(2));
    }, { refetchInterval: (1000 * 60) });
    return (
        <Link key={pair.pair} to={`/investment/${pair.id}`} className="list-group-item list-group-item-action d-flex gap-3 py-3" aria-current="true">
            <div className="d-flex gap-2 w-100 justify-content-between">
                <div>
                    <div className='d-flex gap-2' style={{ alignItems: 'center' }}>
                        <Icon icon={`cryptocurrency:${pair.from}`} /><h6 className="mb-0 p-1">{pair.from}</h6>
                    </div>
                    <div className='d-flex gap-2' style={{ alignItems: 'center' }}>
                        <Icon icon={`cryptocurrency:${pair.to}`} /><h6 className="mb-0 p-1">{pair.to}</h6>
                    </div>
                </div>
                <div>
                    <small className="opacity-50 text-nowrap">{pair.value}$</small>
                    <br />
                    <small className="opacity-50 text-nowrap">{price} $</small>
                </div>
                <div>
                    <small className="opacity-50 text-nowrap">{(pair.amount * price).toFixed(2)} $</small>
                    <br />
                    <small className="opacity-50 text-nowrap">{pair.amount}</small>
                </div>
            </div>
        </Link>
    )
}
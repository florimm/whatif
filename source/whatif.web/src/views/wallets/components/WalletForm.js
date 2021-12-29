import { useParams, Link } from "react-router-dom";
import { useQuery, useQueryClient, useMutation } from "react-query";
import { postData } from "../../../utilities/requests";
import { useState } from 'react';

const initWalletForm = {
    name: ''
}

export default function WalletForm({ currentWallet }) {
    const { walletId, userId } = useParams();
    const [wallet, setWallet] = useState(currentWallet || initWalletForm);

    const mutation = useMutation(wallet => {
        const url = walletId === 'new' ? `/api/users/${userId}/wallets` : `/api/users/${userId}/wallets/${walletId}`;
        return postData(url, wallet);
    });

    const submitForm = e => {
        e.preventDefault();
        mutation.mutate({ name: wallet.name });
    }

    return (
        <form onSubmit={submitForm}>
            <div className="form-group">
                <label htmlFor="walletName">Wallet Name</label>
                <input type="text" value={wallet.name} onChange={e => setWallet(t => ({...t, name: e.target.value }))} name="walletName" className="form-control" id="walletName" placeholder="Enter wallet name" />
            </div>
            <button type="submit" className="btn btn-primary">Save</button>
        </form>
    )
}
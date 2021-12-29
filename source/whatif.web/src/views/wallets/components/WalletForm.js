import { useState } from 'react';

const initWalletForm = {
    name: ''
}

export default function WalletForm({ currentWallet, onSave }) {
    const [wallet, setWallet] = useState(currentWallet || initWalletForm);

    const submitForm = e => {
        e.preventDefault();
        onSave(wallet);
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
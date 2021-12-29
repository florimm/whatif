import { useParams } from "react-router-dom";
import { useMutation, useQueries } from "react-query";
import { postData, getData } from "../../utilities/requests";
import WalletForm from "./components/WalletForm";

export default function UpdateWallet() {
    const { walletId, userId } = useParams();

    const { data } = useQueries(["wallet", walletId], async () => {
        return getData(`/api/users/${userId}/wallets/${walletId}`);
    });

    const mutation = useMutation(wallet => {
        return postData(`/api/users/${userId}/wallets/${walletId}`, wallet);
    });

    const saveWallet = wallet => {
        mutation.mutate({ ...wallet });
    }
    return (
        <>
            <div>Update</div>
            <WalletForm currentWallet={data} onSave={saveWallet} />
        </>
    )
}
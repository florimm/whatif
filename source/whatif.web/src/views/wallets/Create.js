import { useParams } from "react-router-dom";
import { useMutation } from "react-query";
import { postData } from "../../utilities/requests";
import WalletForm from "./components/WalletForm";

export default function CreateWallet() {
    const { userId } = useParams();

    const mutation = useMutation(wallet => {
        return postData(`/api/users/${userId}/wallets`, wallet);
    });

    const saveWallet = wallet => {
        mutation.mutate({ ...wallet });
    }
    return (
        <>
            <div>Create</div>
            <WalletForm onSave={saveWallet} />
        </>
    )
}
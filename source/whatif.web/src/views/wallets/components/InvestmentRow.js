import { useQuery } from "react-query";
import { Icon } from '@iconify/react';
import { getData } from "../../../utilities/requests";
import { queryKeys } from '../../../constants';

export default function InvestmentRow({ investment, onSelect }) {
    const { from, to } = investment;
    const { data: currentValue, isFetching } = useQuery(
        [queryKeys.price, from, to],
        (data) => getData(`pricechange/pair/${data.queryKey[1].toUpperCase()}-${data.queryKey[2].toUpperCase()}`),
        { refetchInterval: (1000 * 60) }
    );

    const handleSelection = (e) => {
        e.preventDefault();
        onSelect(investment);
    }

    const rowStyle = isFetching ? { borderBottom: '1px solid red' } : {};
    return (
        <button onClick={handleSelection} style={rowStyle} title={currentValue?.date} className="list-group-item list-group-item-action d-flex gap-3 py-3">
            <div className="d-flex gap-2 w-100 justify-content-between">
                <div>
                    <div className='d-flex gap-2' style={{ alignItems: 'center' }}>
                        <Icon icon={`cryptocurrency:${investment.from.toLowerCase()}`} /><h6 className="mb-0 p-1">{investment.from}</h6>
                    </div>
                    <div className='d-flex gap-2' style={{ alignItems: 'center' }}>
                        <Icon icon={`cryptocurrency:${investment.to.toLowerCase()}`} /><h6 className="mb-0 p-1">{investment.to}</h6>
                    </div>
                </div>
                <div>
                    <small className="opacity-50 text-nowrap">{investment.amount}</small>
                    <br />
                    <small className="opacity-50 text-nowrap">{investment.value} $</small>
                </div>
                <div>
                    <small className="opacity-50 text-nowrap">{(investment.amount * currentValue?.price).toFixed(2)} $</small>
                    <br />
                    <small className="opacity-50 text-nowrap">{currentValue?.price.toFixed(2)} $</small>
                </div>
            </div>
        </button>
    )
}
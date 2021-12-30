import { Link } from "react-router-dom";
import { useQuery } from "react-query";
import { Icon } from '@iconify/react';
import { getData } from "../../utilities/requests";
import { queryKeys } from '../../../constants';

export default function InvestmentRow({ pair }) {
    const { data: price } = useQuery(
        [queryKeys.price, pair.pair],
        (data) => getData(`pricechange/pair-price/${data.queryKey[1]}`),
        { refetchInterval: (1000 * 60) }
    );

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
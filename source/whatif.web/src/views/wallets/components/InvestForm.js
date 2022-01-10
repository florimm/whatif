import React, { useState } from 'react';
import { useQuery, useQueryClient } from "react-query";
import { getData } from "../../../utilities/requests";
import { queryKeys } from '../../../constants';

const initInvestForm = {
    from: 'ETH',
    to: 'USDT',
    value: 0,
    amount: 0,
    total: '0 $',
};

export default function InvestForm({ currentInvestment, onSave, onCancel }) {
    const [investment, setInvestmentment] = useState(currentInvestment || initInvestForm);
    const queryClient = useQueryClient();
    const { from } = investment;
    const { data: currentValue } = useQuery(
        [queryKeys.priceLookup, from],
        (data) => getData(`pricechange/pair/${data.queryKey[1].toLowerCase()}-USDT`),
        {
            enabled: (!!from),
            onSuccess: (data) => { setInvestmentment({ ...investment, value: data.price }); } }
    );
    
    const submitForm = e => {
        e.preventDefault();
        onSave({...investment, to: 'USDT'});
    }

    const handleOnBlur = e => {
        queryClient.invalidateQueries(queryKeys.priceLookup);
    }

    const onAmountChange = e => {
        setInvestmentment({ ...investment, amount: e.target.value, total: `${(e.target.value * currentValue?.price)}` });
    }


    return (
        <div>
            <form onSubmit={submitForm}>
                <div className="form-group">
                    <label htmlFor="from">Crupto</label>
                    <input type="text" className="form-control" id="from" placeholder="From" value={investment.from} onBlur={handleOnBlur} onChange={e => setInvestmentment({ ...investment, from: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="to">Stable coin</label>
                    <input type="text" readOnly={true} className="form-control" value="USDT" name="to" />
                </div>
                <div className="form-group">
                    <label htmlFor="value">Value</label>
                    <input type="number" className="form-control" id="value" placeholder="Value" value={investment.value} onChange={e => setInvestmentment({ ...investment, value: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="amount">Amount</label>
                    <input type="number" step="0.01" className="form-control" id="amount" placeholder="Amount" value={investment.amount} onChange={onAmountChange} />
                </div>
                <div className="form-group">
                    <label htmlFor="total">Total</label>
                    <p className="form-control" name="total">{Number(investment.total).toFixed(2)} $</p>
                </div>
                <hr />
                <div className='d-flex'>
                    <button type="submit" className="btn btn-primary m-2">Save</button>
                    <button onClick={onCancel} className="btn btn-primary m-2">Cancel</button>
                </div>
            </form>
        </div>
    )
}
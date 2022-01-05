import React, { useState } from 'react';
import { useQuery, useQueryClient, useMutation } from "react-query";
import { useParams } from "react-router-dom";
import { getData, postData } from "../../../utilities/requests";
import { queryKeys } from '../../../constants';

const initInvestForm = {
    from: 'ETH',
    to: 'USDT',
    value: 0,
    amount: 0,
};

export default function InvestForm({ currentInvestment, onSave, onCancel }) {
    const [investment, setinvestmentment] = useState(currentInvestment || initInvestForm);

    const { from, to } = investment;
    const { data: currentValue, isFetching } = useQuery(
        [queryKeys.price, from, to],
        (data) => {
            if (data.queryKey[1] && data.queryKey[2]) {
                return getData(`pair-price/${data.queryKey[1].toLowerCase()}${data.queryKey[2].toLowerCase()}`);
            }
        },
        { enabled: (!!from && !!to) }
    );
    
    const submitForm = e => {
        e.preventDefault();
        onSave(investment);
    }


    return (
        <div>
            <h1>Investment</h1>
            <form onSubmit={submitForm}>
                <div className="form-group">
                    <label htmlFor="from">From</label>
                    <input type="text" className="form-control" id="from" placeholder="From" value={investment.from} onChange={e => setinvestmentment({ ...investment, from: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="to">To</label>
                    <input type="text" className="form-control" id="to" placeholder="To" value={investment.to} onChange={e => setinvestmentment({ ...investment, to: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="value">Value</label>
                    <input type="number" className="form-control" id="value" placeholder="Value" value={investment.value} onChange={e => setinvestmentment({ ...investment, value: e.target.value })} />
                </div>
                <div className="form-group">
                    <label htmlFor="amount">Amount</label>
                    <input type="number" className="form-control" id="amount" placeholder="Amount" value={investment.amount} onChange={e => setinvestmentment({ ...investment, amount: e.target.value })} />
                </div>
                <button type="submit" className="btn btn-primary">Submit</button>
                <button onClick={onCancel} className="btn btn-primary">Cancel</button>
            </form>
        </div>
    )
}
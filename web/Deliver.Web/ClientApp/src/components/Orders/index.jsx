import React, { useState, useEffect } from 'react';
import API from '../../api';
import Constants from '../../Constants';
import Select from 'react-select';
import OrderCard from './orderCard';
import Pagination from '../Pagination';

function Orders({ history, id }) {
    const api = API.getStore;
    const getOrders = API.getOrders;
    const [state, setState] = useState({ 
        shop:null, 
        orders:[], 
        paging:null, 
        islands:[], 
        paymentMethods:[], 
        selectedIsland: null, 
        selectedPaymentMethod:null,
        selectedDate: null,
        selectedStatus: null,
    });
    
    useEffect(() => {
            api(id).then(response => {
                let newState = { ...state };
                newState.shop = response.result;
                newState.paymentMethods = response.result.paymentMethods.map(m => ({ value:m, label:Constants.PaymentMethods.getText(m) }))
                const allowedIslandIds = newState.shop.islands.map(x => x.id);
                getOrders()
                .then(response => {
                    newState.orders = response.result
                    newState.paging = response.paging;
                    API.getIslands()
                    .then(response => {
                        newState.islands = response.result
                        .filter(x => allowedIslandIds.includes(x.id))
                        .map(x => ({ label: x.name, value: x.id }));
                        setState(newState);
                    });
                });
            });
    }, []);

    const handleAction = function (key, value = null) {
        let newState = { ...state };
        switch (key) {
            case 'search_input':
                newState.search = value;
                setState(newState);
                break;
            case 'search':
                {
                    const island = newState.selectedIsland ? newState.selectedIsland.value : null;
                    const paymentMethod = newState.selectedPaymentMethod ? newState.selectedPaymentMethod.value : null;
                    const status = newState.selectedStatus ? newState.selectedStatus.value : null;
                    const date = newState.selectedDate ? newState.selectedDate.value : null;
                    const page = value ? value : 1;
                    
                    getOrders(newState.search, paymentMethod, date, status, island, page)
                        .then(response => {
                            newState.orders = response.result;
                            newState.paging = response.paging;
                            setState(newState);
                        });
                }
                break;
            case 'island_select':
                newState.selectedIsland = value;
                setState(newState);
                break;
            case 'payment_method_select':
                newState.selectedPaymentMethod = value;
                setState(newState);
                break;
            case 'status_select':
                newState.selectedStatus = value;
                setState(newState);
                break;
            case 'date_change':
                newState.selectedDate = '' + value;
                setState(newState);
                break;
            case 'order_click':
                history.push(`/shop/${id}/orders/${value}`);
                break;
            case 'pagination':
                handleAction('search', value);
                break;
        }
    };

    const data = state.shop;

    if(!data)
        return null;

    return (
        <div className="container">
            <div className="row">
                <div className="d-none col-md-3 d-md-block">
                    <div className="card shop-card" style={{ cursor: 'pointer' }}>
                        <div className="card-body">
                            <h5 className="card-title text-primary">{data.name}</h5>
                            <h6 className="card-subtitle mb-2 text-muted">Phone: {data.phoneNumber}</h6>
                            <p className="card-subtitle mb-2 text-muted">{
                                data.islands.map(x => (
                                <small className="px-1">{x.name}</small>
                                ))
                            }</p>
                        </div>
                        <div className="card-footer text-muted flex">
                            {data.paymentMethods.map(x => (
                                <span
                                    title={`This store accepts ${Constants.PaymentMethods.getText(x)} payments.`}
                                    className={`${Constants.PaymentMethods.getClasses(x)} p-1 mx-2`}>
                                    {Constants.PaymentMethods.getText(x)}
                                </span>
                                ))}
                        </div>
                    </div>
                </div>
                <div className="col-12 col-md-9">
                    <div className="form-row align-items-center">
                        <div className="pt-2 pt-md-0 col-12 col-md-auto mb-2">
                            <label className="sr-only" for="inlineFormInput">Search</label>
                            <input
                                onChange={e => handleAction('search_input', e.target.value)}
                                value={state.search}
                                style={{ minWidth:'270px' }}
                                onKeyDown={e => { if (e.keyCode === 13) handleAction('search'); }  }
                                type="text"
                                className="form-control"
                                placeholder="Address, Island, Phone or Notes..." />
                        </div>
                        <div className="pt-2 pt-md-0 col-12 col-md-auto mb-2">
                            <label className="sr-only" for="inlineFormInput">Search</label>
                            <input
                                onChange={e => handleAction('date_change', e.target.value)}
                                value={state.selectedDate}
                                type="date"
                                className="form-control"
                                placeholder="Filter by date" />
                        </div>
                        <div className="pt-2 pt-md-0 col-12 col-md-auto mb-2" style={{minWidth: '200px'}}>
                            <label className="sr-only" htmlFor="inlineFormInput">Payment Method</label>
                            <Select
                                options={state.paymentMethods}
                                isClearable={true}
                                value={state.selectedPaymentMethod}
                                placeholder="Payment Method"
                                onChange={v => handleAction('payment_method_select', v)}
                            />
                        </div>
                        <div className="pt-2 pt-md-0 col-12 col-md-auto mb-2" style={{minWidth: '200px'}}>
                                <label className="sr-only" htmlFor="inlineFormInput">Status</label>
                                <Select
                                    options={Constants.OrderStatuses.getDefinition()}
                                    isClearable={true}
                                    value={state.selectedStatus}
                                    placeholder="Filter by Status"
                                    onChange={v => handleAction('status_select', v)}
                                />
                            </div>
                        {
                            state.islands.length > 1 &&
                            <div className="pt-2 pt-md-0 col-12 col-md-auto mb-2" style={{minWidth: '200px'}}>
                                <label className="sr-only" htmlFor="inlineFormInput">Island</label>
                                <Select
                                    options={state.islands}
                                    isClearable={true}
                                    value={state.selectedIsland}
                                    placeholder="Filter by Island"
                                    onChange={v => handleAction('island_select', v)}
                                />
                            </div>
                        }
                        <div className="pt-2 pt-md-0 col-12 col-md-auto text-center mb-2">
                            <button
                                onClick={() => handleAction('search')}
                                type="button"
                                className="btn btn-primary"
                            >Search</button>
                        </div>
                    </div>
                </div>
            </div>
            <div className="row">
                    {
                        state.orders.length > 0 &&
                        state.orders.map(o => (
                            <OrderCard data={o} onClick={oid => handleAction('order_click', oid)} />
                        ))
                    }
            </div>
            <Pagination paging={state.paging} onPage={page => handleAction('pagination', page)} allowGranularControl={true} />
        </div>

    );
}


export default function paramsToProps(props) {
    return Orders({ history: props.history, id:props.match.params.id });
}
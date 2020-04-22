import React, { useState, useEffect } from 'react';
import API from '../../api';
import Constants from '../../Constants';
import Select from 'react-select';
import OrderCard from './orderCard';
import Pagination from '../Pagination';
import { notify } from 'react-notify-toast';

function Order({ history, shopId, id }) {
    const getStore = API.getStore;
    const getOrder = API.getOrder;
    const updateOrder = API.updateOrder;

    const [state, setState] = useState({ 
        shop:null, 
        order:null
    });

    const [fillables, setFillables] = useState({
        status: {
            value: null
        },
        notes: {
            value: null
        },
        price: {
            value: null
        },
    });
    
    useEffect(() => {
            getStore(shopId).then(response => {
                let newState = { ...state };
                newState.shop = response.result;
                getOrder(id)
                .then(response => {
                    newState.order = response.result;
                    const newFillables = {...fillables};
                    const order = newState.order;
                    newFillables.status.value = Constants.OrderStatuses.getDefinition(order.status);
                    newFillables.notes.value = order.notes;
                    newFillables.price.value = order.price;
                    setFillables(newFillables);
                    setState(newState);
                });
            });
    }, []);

    // useEffect(() => {
    //     const newFillables = {...fillables};
    //     const order = state.order;
    //     console.log('order', order);
    //     newFillables.status = Constants.OrderStatuses.getDefinition(order.status);
    //     newFillables.notes = order.notes;
    //     newFillables.price = order.price;
    //     setFillables(newFillables);
    // }, [state.order])

    const handleAction = function (key, value = null) {
        let newState = { ...state };
        switch (key) {
            case 'save':
                    updateOrder(id, { price: fillables.price.value, notes:fillables.notes.value, status:fillables.status.value.value }).then(() => {
                        ///TODO: Display Success and navgate back
                        notify.show('Saved Successfully', 'success', 3000);
                    }).catch(e => notify.show('Sorry, there was an error', 'danger', 3000));
                break;
            default:
                {
                    const fillable = {...fillables[key]};
                    fillable.value = value;

                    let newFillables = {...fillables};
                    newFillables[key] = fillable;
                    setFillables(newFillables);
                }
                break;
        }
    };


    if(!state.order || !state.shop)
        return null;

    const { order, shop } = state;

    return (
        <div className="container">
            <div className="row">
                <div className="d-none col-md-3 d-md-block">
                    <div className="card shop-card" style={{ cursor: 'pointer' }}>
                        <div className="card-body">
                            <h5 className="card-title text-primary">{shop.name}</h5>
                            <h6 className="card-subtitle mb-2 text-muted">Phone: {shop.phoneNumber}</h6>
                            <p className="card-subtitle mb-2 text-muted">{
                                shop.islands.map(x => (
                                <small className="px-1">{x.name}</small>
                                ))
                            }</p>
                        </div>
                        <div className="card-footer text-muted flex">
                            {shop.paymentMethods.map(x => (
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
                    <div className="row align-items-center">
                        <div className="col-12 pb-4">
                            <p>Order Items</p>
                            {
                                order.orderItems && order.orderItems.length > 0 &&
                                order.orderItems.map((i,index) => (
                                    <div class="card">
                                        <div class="card-body px-3 py-2">
                                            {(index+1)}. {i.orderDetails}
                                        </div>
                                    </div>
                                ))
                            }
                            {
                                (!order.orderItems || order.orderItems.length < 1) &&
                                <h6 className="text-danger">Sorry we found no items in this order.</h6>
                            }
                        </div>

                        <div className="form-group col-12 col-md-6">
                            <label>Price (MVR)</label>
                            <input className="form-control"
                                type="number"
                                min="1"
                                step="any"
                                value={fillables.price.value}
                                placeholder="Price"
                                onChange={v => handleAction('price', v.target.value)}
                            />
                        </div>
                        <div className="form-group  col-12 col-md-6">
                            <label>Status</label>
                            <Select 
                                options={Constants.OrderStatuses.getDefinition()} 
                                value={fillables.status.value}
                                placeholder="Select Order Status"
                                onChange={v => handleAction('status', v)}
                            />
                        </div>
                        <div className="form-group col-12">
                            <textarea className="form-control"
                                style={{minHeight: '200px'}}
                                value={fillables.notes.value}
                                placeholder={"Enter any notes for the customer to see."}
                                onChange={v => handleAction('notes', v.target.value)}
                            />
                        </div>

                            <div className="pt-2 pt-md-0 col-12 col-md-auto text-center mb-2">
                                <button
                                    onClick={() => handleAction('save')}
                                    type="button"
                                    className="btn btn-primary"
                                >Save</button>
                            </div>
                        </div>
                </div>
            </div>
        </div>

    );
}


export default function paramsToProps(props) {
    return Order({ history: props.history, ...props.match.params });
}
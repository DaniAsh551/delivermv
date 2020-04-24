import React, { useState, useEffect } from 'react';
import API from '../../api';
import Constants from '../../Constants';
import Select from 'react-select';
import OrderCard from './orderCard';
import Pagination from '../Pagination';
import { notify } from 'react-notify-toast';

function TrackOrder({ orderId }) {
    const trackOrder = API.trackOrder;
    const getShop = API.getStore;

    const [state, setState] = useState({
        order:null,
        shop: null
    });

    const [fillables, setFillables] = useState({
        orderId: {
            value: orderId,
            validationText: '',
            validation: function(value){
                return value && value.length == 32
                    ? ''
                    : 'Please Enter a valid Order no.';
            }
        },
    });

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
            case 'track':
                    {
                        let newFillables = {...fillables};
                        newFillables.orderId.validationText 
                            = newFillables.orderId.validation(newFillables.orderId.value);

                        if(newFillables.orderId.validationText.length > 0){
                            setFillables(newFillables);
                            return;
                        }

                        let orderId = '';
                        let noAdded = 0;
                        fillables.orderId.value.split('').forEach((e,i) => {
                            if([8,13,18,23].includes(noAdded)){
                                noAdded++;
                                orderId += '-';
                            }

                            orderId += ('' + e);
                            noAdded++;
                        });

                        trackOrder(orderId).then(response => {
                            newState.order = response.result;
                            getShop(newState.order.shop).then(response => {
                                newState.shop = response.result;
                                setState(newState);
                            });
                        }).catch(e => {
                            newFillables.orderId.validationText = newFillables.orderId.validation('');
                            setFillables(newFillables);
                            notify.show("Sorry, we couldn't find that order.", 'warning', 5000);
                        });
                    }
                break;
            default:
                {
                    const fillable = {...fillables[key]};
                    fillable.value = value;
                    fillable.validationText = fillable.validation(fillable.value);

                    let newFillables = {...fillables};
                    newFillables[key] = fillable;
                    setFillables(newFillables);
                }
                break;
        }
    };

    
    const { order, shop } = state;
    if(orderId && (!order || !shop))
        handleAction('track');

    return (
        <div className="container">
            
            <div className="row">
                {
                    !(order && shop) &&
                    <div className="col-12 col-md-3">
                        <div className="form-group">
                            <label>Order No</label>
                            <input type="text"
                                placeholder="Enter your order number"
                                onKeyDown={e => { if(e.which == 13) handleAction('track') }}
                                onChange={e => handleAction('orderId', e.target.value)}
                            className="form-control" />
                            <span className="text-danger">{fillables.orderId.validationText}</span>
                        </div>
                        <div className="form-group">
                            <button className="btn btn-primary"
                                onClick={() => handleAction('track')}
                                >Track</button>
                        </div>
                    </div>
                }
                
                {
                    (order && shop) &&
                    <>
                        <div className="col-12 col-md-3">
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
                    </>
                }

                {
                    (order && shop) &&
                    <div className="col-12 col-md-9">
                        <ul class="timeline">
                            <li className={getTimelineClass(0, order)}>
                                <p>Received</p>
                                <p>
                                    <small className="text-muted">
                                        This means that the shop has received your order, but haven't yet processed or confirmed it.
                                    </small>
                                </p>
                            </li>
                            <li className={getTimelineClass(1, order)}>
                                <p>Confirmed</p>
                                <p>
                                    <small className="text-muted">
                                        This means that the shop has received and confirmed your order.
                                    </small>
                                </p>
                            </li>
                            {
                                order.status != 2 &&
                                <li className={getTimelineClass(3, order)}>
                                    <p>Completed</p>
                                    <p>
                                        <small className="text-muted">
                                            This means that the shop has successfully delivered your order and have received payment from you.
                                        </small>
                                    </p>
                                </li>
                            }
                            {
                                order.status == 2 &&
                                <li className="rejected text-danger">
                                    <p>Rejected</p>
                                    <p>
                                        <small className="text-muted">
                                            This means that the shop was unable to process your order. 
                                            The order notes may contain some helpful information from the seller.
                                        </small>
                                    </p>
                                </li>
                            }
                        </ul>
                    </div>
                }
                
                {
                    (order && shop) &&
                    <div className="col-6 col-md-4">
                        <h6>Address</h6>
                        {
                            order.address && order.address.length > 0 &&
                            <p className="text-muted">
                               {order.address}, {order.island.name}
                            </p>
                        }
                        {
                            !(order.address && order.address.length > 0) &&
                            <p className="text-muted font-italic">
                            Sorry, we couldn't find that information.
                            </p>
                        }
                    </div>
                }

                {
                    (order && shop) &&
                    <div className="col-6 col-md-4">
                        <h6>Price</h6>
                        {
                            order.price && order.price > 0 &&
                            <p className="text-muted">
                                MVR {order.price}
                            </p>
                        }
                        {
                             !(order.price && order.price > 0) &&
                             <p className="text-muted font-italic">
                                Not Updated by Shop
                             </p>
                        }
                    </div>
                }

                {
                    (order && shop) &&
                    <div className="col-6 col-md-4">
                        <h6>Payment Method</h6>
                        {
                            order.paymentMethods != null && order.paymentMethods.length > 0 &&
                            <p className="text-muted">
                                {order.paymentMethods.map(x => Constants.PaymentMethods.getText(x)).join(', ')}
                            </p>
                        }
                        {
                             !(order.paymentMethods != null && order.paymentMethods.length > 0) &&
                             <p className="text-muted font-italic">
                                 Sorry, we couldn't find that information.
                             </p>
                        }
                    </div>
                }

                {
                    (order && shop) &&
                    <div className="col-12 col-md-12">
                        <h6>Notes from Shop</h6>
                        {
                            order.notes != null && order.notes.length > 0 &&
                            <p className="text-muted">
                                <p dangerouslySetInnerHTML={{__html:order.notes.split('\n').map(x => `<p>${x}</p>`)}}></p>
                            </p>
                        }
                        {
                             !(order.notes != null && order.notes.length > 0) &&
                             <p className="text-muted font-italic">
                                The shop has not written any notes for this order.
                             </p>
                        }
                    </div>
                }

                {
                    (order && shop) &&
                    <>
                        <div className="col-12 col-md-9">
                                <div className="row align-items-center">
                                    <div className="col-12 pb-4">
                                        <h6>Order Items</h6>
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
                                            <h6 className="text-danger font-italic">Sorry we found no items in this order.</h6>
                                        }
                                    </div>
                                </div>
                            </div>
                    </>
                }
                
            </div>
        </div>

    );
}

function getTimelineClass(status, order){
    return (order && status == order.status) ? 'active text-info' : '';
}

export default function paramsToProps(props) {
    return TrackOrder({ orderId: props.match.params.orderId });
}
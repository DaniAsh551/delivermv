import React, { useState, useEffect } from 'react';
import API from '../../api';
import Constants from '../../Constants';
import Select from 'react-select';
import { notify } from 'react-notify-toast';

function Shop({ history, id }) {
    const api = API.getStore;
    const createOrder = API.createOrder;
    const [state, setState] = useState({ shop:null, islands:[], orderId:null });

    const [fillables, setFillables] = useState({
        island: {
            value: null,
            validationText: '',
            validation: (value) => {
                return typeof(value) === 'object'
                ? ''
                : 'Please select your Island.';
            }
        },
        phoneNumber: {
            value: null,
            validationText: '',
            validation: (value) => {
                value = value || '';
                return value.length === 7 
                && parseInt(value) != NaN
                && ['3','7','9'].includes(value[0])
                ? ''
                : 'Please enter your phone number.'
                ;
            }
        },
        address: {
            value: null,
            validationText: '',
            validation: (value) => {
                return value && value.length > 0
                ? ''
                : 'Please enter your address.'
                ;
            }
        },
        items: {
            value: null,
            validationText: '',
            validation: (value) => {
                return value && value.length > 0
                ? ''
                : 'Please enter the items you need.'
                ;
            }
        },
        paymentMethods: {
            value: null,
            validationText: '',
            validation: (value) => {
                return value && value.length > 0
                ? ''
                : 'Please select atleast one Payment Method.'
                ;
            }
        }
    });

    useEffect(() => {
            api(id).then(response => {
                let newState = { ...state };
                newState.shop = response.result;
                const allowedIslandIds = newState.shop.islands.map(x => x.id);
                API.getIslands()
                .then(response => {
                    newState.islands = response.result
                    .filter(x => allowedIslandIds.includes(x.id))
                    .map(x => ({ label: x.name, value: x.id }));
                    setState(newState);
                });
            });
    }, []);

    useEffect(() => {
        if(state.islands.length === 1){
            const newFillables = {...fillables};
            newFillables.island.value = state.islands[0];
            setFillables(newFillables);
        }
    }, [state.islands])

    const handleEvent= function(key,value = null){
        switch(key){
            case 'order':
                    {
                        const newFillables = {...fillables};
                        const validItems = Object.keys(newFillables)
                        .map(k => ({ id:k, fillable:newFillables[k] }))
                        .filter(({id, fillable}) => {
                            fillable.validationText = fillable.validation(fillable.value);
                            newFillables[id].validationText = fillable.validationText;
                            return fillable.validationText.length < 1;
                        });
                        const areAllValid = validItems.length == Object.keys(fillables).length;

                        if(!areAllValid)
                        {
                            setFillables(newFillables);
                            return;
                        }

                        const orderItems = fillables.items.value.split('\n');
                        const order = {
                            address: fillables.address.value,
                            store: id,
                            island: fillables.island.value.value,
                            phone: fillables.phoneNumber.value,
                            paymentMethods: fillables.paymentMethods.value.map(x => x.value),
                            orderItems
                        };

                        createOrder(order).then(response => {
                            const orderId = response.result[0];
                            const newState = { ...state };
                            newState.orderId = orderId;
                            notify.show('Order placed successfully', 'success', 3000);
                            setState(newState);
                        })
                        .catch(e => {
                            notify.show('We could not place that order.', 'danger', 3000);
                        });
                    }
                break;
            default:
                {
                    const fillable = {...fillables[key]};
                    fillable.value = value;
                    fillable.validationText = fillable.validation(value);

                    let newFillables = {...fillables};
                    newFillables[key] = fillable;
                    setFillables(newFillables);
                }
                break;
        }
    };

    const {orderId} = state;
    const data = state.shop;

    if(!data && !orderId)
        return null;

    return (
        <div className="row">
            {
                orderId && 
                <div className="col-12 text-center">
                    <h6>Congratulations, your order has been placed.</h6>
                    <p>Your order no is: <b>{state.orderId}</b></p>
                    <p>Please keep this order no saved someplace as you won't be able to find the order later without the order no.</p>
                    <p>You can also head over to <a className="text-success" style={{cursor:'pointer'}} onClick={e => history.push('/track')}>'{window.location.origin}/track'</a> to track your order.</p>
                    <button className="btn btn-success" onClick={() => history.goBack()}>Go Back</button>
                </div>
            }
            {
                !orderId &&
                <>
                    <div className="col-12 col-md-3">
                        <div className="card shop-card" style={{ cursor: 'pointer' }}>
                        <div className="card-body">
                            <h5 className="card-title text-primary">{data.name}</h5>
                            <h6 className="card-subtitle mb-2 text-muted">Phone: {data.phoneNumber}</h6>

                            {
                                data.bmlAccount && 
                                <>
                                    <p><b>BML Account No:</b></p>
                                    <p>{data.bmlAccount}</p>
                                </>
                            }
                            {
                                data.mibAccount && 
                                <>
                                    <p><b>MIB Account No:</b></p>
                                    <p>{data.mibAccount}</p>
                                </>
                            }

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
                    <div className="col-12 col-md-9 mt-4 mt-md-0">
                        <h4>Place your order</h4>
                        <div className="container">
                            <div className="row">
                                <div className="col-12 col-md-4">
                                    <div className="form-group">
                                        <Select
                                        style={getInputStyle(fillables.island.validationText)}
                                        options={state.islands} 
                                        value={fillables.island.value} 
                                        onChange={v => handleEvent('island', v)} 
                                        placeholder="Select your Island" />
                                        <span className="text-danger">{fillables.island.validationText}</span>
                                    </div>
                                </div>
                                <div className="col-12 col-md-4">
                                    <div className="form-group">
                                        <input className="form-control"
                                        style={getInputStyle(fillables.phoneNumber.validationText)}
                                        placeholder="Your Phone Number"
                                        value={fillables.phoneNumber.value}
                                        onChange={e => handleEvent('phoneNumber', e.target.value)} />
                                        <span className="text-danger">{fillables.phoneNumber.validationText}</span>
                                    </div>
                                </div>
                                <div className="col-12 col-md-4">
                                    <div className="form-group">
                                        <input className="form-control"
                                        style={getInputStyle(fillables.address.validationText)}
                                        placeholder="Your Address"
                                        value={fillables.address.value}
                                        onChange={e => handleEvent('address', e.target.value)} />
                                        <span className="text-danger">{fillables.address.validationText}</span>
                                    </div>
                                </div>
                                <div className="col-12 col-md-6">
                                    <div className="form-group">
                                        <Select
                                        isMulti={true}
                                        style={getInputStyle(fillables.paymentMethods.validationText)}
                                        options={data.paymentMethods.map(x => ({ label:Constants.PaymentMethods.getText(x), value:x }))} 
                                        value={fillables.paymentMethods.value} 
                                        onChange={v => handleEvent('paymentMethods', v)}
                                        placeholder="Select Payment methods" />
                                        <span className="text-danger">{fillables.paymentMethods.validationText}</span>
                                    </div>
                                </div>
                                <div className="col-12">
                                    <div className="form-group">
                                        <textarea className="form-control"
                                        style={{...getInputStyle(fillables.items.validationText), minHeight: '200px'}}
                                        placeholder={"Enter your order items.\r\neg: \r\n5 x 1.5L Water Bottles (Taza)\r\n2 x Noodle Packets (Kottu Mee)"}
                                        value={fillables.items.value}
                                        onChange={e => handleEvent('items', e.target.value)} />
                                        <span className="text-danger">{fillables.items.validationText}</span>
                                    </div>
                                </div>

                                <div className="col-12 text-center">
                                    <button type="button" className="btn btn-primary px-4" onClick={e => handleEvent('order')}>Order</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            }
        </div>

    );
}

function getInputStyle(validationText){
    if(validationText && validationText.length > 1)
        return { borderColor:'red' };

    return {};
}

export default function paramsToProps(props) {
    return Shop({ history: props.history, id:props.match.params.id });
}
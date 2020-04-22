import React, { useState, useEffect } from 'react';
import Constants from '../../Constants';

export default function ShopCard({data, onClick}){
    return (
        <div className="col-12 col-md-4 py-2">
            <div className="card shop-card" style={{ cursor: 'pointer' }} onClick={onClick}>
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
    );
};
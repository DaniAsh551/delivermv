import React, { useState, useEffect } from 'react';
import Constants from '../../Constants';

export default function OrderCard({data, onClick}){
    return (
        <div className="col-12 col-md-6 py-2">
            <div className="card shop-card" style={{ cursor: 'pointer' }} onClick={() => onClick(data.id)}>
                <div className="card-body">
                    <h5 className="card-title text-primary">{data.Address}</h5>
                    <h6 className="card-subtitle mb-2 text-muted">Phone: {data.phoneNumber}</h6>
                    <p className="card-subtitle mb-2 text-muted">{data.address}, {data.island.name}</p>
                    <p className="card-subtitle mb-2 text-muted"><small>{data.orderItems.length} Item{ data.orderItems.length == 1 ? '' : 's'}</small></p>
                    <p className="card-subtitle mb-2 text-muted"><small>Status: {Constants.OrderStatuses.getDefinition(data.status).label}</small></p>
                </div>
                <div className={"card-footer text-muted flex " + Constants.OrderStatuses.getClasses(data.status)}>
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
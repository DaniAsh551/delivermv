import React, { useState, useEffect } from 'react';
import API from '../../api';

export default function Pagination({ paging, onPage, allowGranularControl = false }) {

    const hasPrev = paging ? paging.hasPageBefore : false;
    const hasAfter = paging? paging.hasPageAfter : false;
    const page = paging? paging.page : 1;
    const totalPages = paging? paging.totalPages : 1;
    const pageLabel = `Page ${page} of ${totalPages}`;

    if(!allowGranularControl)
        return (
            <div className="p-2 rounded row text-white mx-1 mt-2" style={{background: '#47c4ff'}}>
                <div className="col-2 text-left">
                    {
                        hasPrev && (
                        <a 
                            className="text-white font-weight-bold"
                            style={{cursor:'pointer', fontSize: 'large'}} 
                            onClick={() => onPage(page - 1)}>
                            ◄
                        </a>
                        )
                    }
                    {
                        !hasPrev && (
                        <a className="text-white font-weight-bold" style={{ fontSize: 'large' }}>
                            ◄
                        </a>
                        )
                    }
                </div>
                <div className="col-8 text-center">
                    { 
                        <span>{pageLabel}</span>
                    }
                </div>
                <div className="col-2 text-right">
                    {
                        hasAfter && (
                        <a className="text-white font-weight-bold" style={{cursor:'pointer', fontSize: 'large'}} onClick={() => onPage(page + 1)}>
                            ►
                        </a>
                        )
                    }
                    {
                        !hasAfter && (
                        <a className="text-white font-weight-bold" style={{fontSize: 'large'}}>
                            ►
                        </a>
                        )
                    }
                </div>
            </div>
        );

    

    return (
        <div className="text-center" style={{width:'100%'}}>
            <nav aria-label="Page navigation example">
                <ul className="pagination">
                    
                    {
                        hasPrev && 
                        <>
                            <li className="page-item" style={{cursor:'pointer'}}>
                                <a className="page-link" aria-label="Previous" onClick={() => onPage(1)}>
                                    <span aria-hidden="true">&laquo;</span>
                                    <span className="sr-only">Previous</span>
                                </a>
                            </li>
                            {
                                Array(getLower(page - 1, 2)).fill(null).map((_,i) => (
                                    <li className="page-item" style={{cursor:'pointer'}}><a className="page-link" onClick={() => onPage(page - (i + 1))}>{page - (i + 1)}</a></li>
                                ))
                            }
                        </>
                    }
                    <li className="page-item"><span className="page-link">{page}</span></li>
                    {
                        hasAfter &&
                        <>
                            {
                                Array(getLower(totalPages - 1, 2)).fill(null).map((_,i) => (
                                    <li className="page-item" style={{cursor:'pointer'}}><a className="page-link" onClick={() => onPage(page + (i + 1))}>{page + (i + 1)}</a></li>
                                ))
                            }
                            <li className="page-item" style={{cursor:'pointer'}}>
                                <a className="page-link" aria-label="Next" onClick={() => onPage(totalPages)}>
                                    <span aria-hidden="true">&raquo;</span>
                                    <span className="sr-only">Next</span>
                                </a>
                            </li>
                        </>
                    }
                    
                </ul>
            </nav>
        </div>
    );
}

function getLower(a,b){
    if(a < b)
        return a;
    return b;
}
import React, { useState, useEffect } from 'react';
import API from '../../api';
import ReactPaginate from 'react-paginate';

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
            <nav>
                <ReactPaginate
                onPageChange={p => onPage(p.selected + 1)}
                containerClassName="pagination"
                pageCount={totalPages}
                pageRangeDisplayed={2}
                marginPagesDisplayed={2}
                pageClassName="page-item"
                pageLinkClassName="page-link"
                activeClassName="page-item active"
                previousClassName="page-item"
                previousLinkClassName="page-link"
                nextClassName="page-item"
                nextLinkClassName="page-link"
                previousLabel={'<'}
                initialPage={page - 1}
                nextLabel={'>'} />
            </nav>
        </div>
    );
}

function getLower(a,b){
    if(a < b)
        return a;
    return b;
}
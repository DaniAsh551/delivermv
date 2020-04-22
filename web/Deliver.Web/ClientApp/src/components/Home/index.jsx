import React, { useState, useEffect } from 'react';
import API from '../../api';
import Select from 'react-select';
import ShopCard from './shopCard';
import Pagination from '../Pagination';
import { notify } from 'react-notify-toast';

export function Home({ history }) {

    const api = API.searchStores;
    const displayName = Home.name;

    const [state, setState] = useState({ islands: [], selectedIsland:null, shops: [], search:null, paging:null });

    useEffect(() => {
        API.getIslands()
            .then(response => {
                let newState = { ...state, islands: response.result.map(x => ({ label: x.name, value: x.id })) };
                api()
                    .then(response => {
                        newState.shops = response.result;
                        newState.paging = response.paging;
                        setState(newState)
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
                api(newState.search, state.selectedIsland ? state.selectedIsland.value : null)
                    .then(response => {
                        newState.shops = response.result;
                        newState.paging = response.paging;
                        setState(newState);
                    }).catch(e => notify.show('Sorry, an error has occured', 'danger'));
                break;
            case 'island_select':
                newState.selectedIsland = value;
                setState(newState);
                break;
            case 'shop_click':
                history.push(`/shop/${value}`);
                break;
            case 'pagination':
                api(newState.search, state.selectedIsland ? state.selectedIsland.value : null , value)
                    .then(response => {
                        newState.shops = response.result;
                        newState.paging = response.paging;
                        setState(newState);
                    }).catch(e => notify.show('Sorry, an error has occured', 'danger'));
                break;
        }
    };

    return (
        <div className="container">
            <div className="form-row align-items-center">
                <div className="pt-2 pt-md-0 col-12 col-md-auto">
                    <label className="sr-only" for="inlineFormInput">Search</label>
                    <input
                        onChange={e => handleAction('search_input', e.target.value)}
                        value={state.search}
                        onKeyDown={e => { if (e.keyCode === 13) handleAction('search'); }  }
                        type="text"
                        className="form-control"
                        placeholder="Shop name" />
                </div>
                <div className="pt-2 pt-md-0 col-12 col-md-auto" style={{minWidth: '200px'}}>
                    <label className="sr-only" htmlFor="inlineFormInput">Island</label>
                    <Select
                        options={state.islands}
                        isClearable={true}
                        value={state.selectedIsland}
                        placeholder="Select your Island"
                        onChange={v => handleAction('island_select', v)}
                    />
                </div>
                <div className="pt-2 pt-md-0 col-12 col-md-auto text-center">
                    <button
                        onClick={() => handleAction('search')}
                        type="button"
                        className="btn btn-primary"
                    >Search</button>
                </div>
            </div>
            <div className="row pt-5">
                {state.shops.map(s => (
                    <ShopCard
                        onClick={() => handleAction('shop_click', s.id)}
                        data={s}
                        key={s.id} />
                ))}
            </div>
            <Pagination paging={state.paging} onPage={page => handleAction('pagination', page)} />
        </div>
    );
}

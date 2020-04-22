import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import Shop from './components/Shop';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import Orders from './components/Orders/index';
import Order from './components/Orders/Order';
import TrackOrder from './components/Orders/TrackOrder';
import Notifications from 'react-notify-toast';

import './custom.css'

export default function App() {
  const displayName = App.name;
    return (
        <>
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
                <Route exact path='/shop/:id' component={Shop}  />
                <Route exact path='/track' component={TrackOrder}  />
                <AuthorizeRoute exact path='/shop/:id/orders' component={Orders} />
                <AuthorizeRoute exact path='/shop/:shopId/orders/:id' component={Order} />
            </Layout>
            <Notifications />
        </>
    );
}

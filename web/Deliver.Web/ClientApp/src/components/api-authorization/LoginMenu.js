import React, { Component, Fragment } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import authService from './AuthorizeService';
import { ApplicationPaths } from './ApiAuthorizationConstants';

export class LoginMenu extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isAuthenticated: false,
            userName: null,
            shopId: null
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    async populateState() {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()]);
        this.setState({
            isAuthenticated,
            userName: user && user.name,
            userId: user && user.sub
        });
    }

    render() {
        const { isAuthenticated, userName, userId } = this.state;
        if (!isAuthenticated) {
            const registerPath = `${ApplicationPaths.Register}`;
            const loginPath = `${ApplicationPaths.Login}`;
            return this.anonymousView(registerPath, loginPath);
        } else {
            const profilePath = `${ApplicationPaths.Profile}`;
            const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
            return this.authenticatedView(userId, userName, profilePath, logoutPath);
        }
    }

    authenticatedView(userId, userName, profilePath, logoutPath) {
        return (<Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-light" to={profilePath}>Manage Account</NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} className="text-light" to={`/shop/${userId}/orders`}>Orders</NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} className="text-light" to={logoutPath}>Logout</NavLink>
            </NavItem>
        </Fragment>);

    }

    anonymousView(registerPath, loginPath) {
        return (<Fragment>
            <NavItem>
                <NavLink tag={Link} className="text-light" to="/track">Track Order</NavLink>
            </NavItem>
            <NavItem>
                <NavLink tag={Link} className="text-light" to={loginPath}>Shopkeepers Area</NavLink>
            </NavItem>
        </Fragment>);
    }
}

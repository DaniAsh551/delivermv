import React, { Component, useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';

export default function NavMenu(props) {
    const displayName = NavMenu.name;
    const [state, setState] = useState({
        collapsed: true
    });


    const toggleNavbar = function() {
        setState({
          collapsed: !state.collapsed
        });
    }

    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3 bg-primary" dark>
                <Container>
                    <NavbarBrand tag={Link} to="/"><span>Deliver<i>Mv</i></span></NavbarBrand>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <LoginMenu>
                            </LoginMenu>
                        </ul>
                    </Collapse>
                </Container>
            </Navbar>
        </header>
    );
}

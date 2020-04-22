import React, { Component } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export function Layout({ children }) {
  const displayName = Layout.name;

    return (
        <div>
            <NavMenu />
            <Container>
                { children }
            </Container>
        </div>
    );
}

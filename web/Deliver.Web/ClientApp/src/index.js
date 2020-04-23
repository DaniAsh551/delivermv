import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker, { unregister } from './registerServiceWorker';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
  <div className="pb-5">
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>
  </div>,
  rootElement);

registerServiceWorker();
import React from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';
import App from './App';

const root = document.getElementById('root');
if (root !== null) {
    const reactRoot = createRoot(root);
    reactRoot.render(<App />);
}
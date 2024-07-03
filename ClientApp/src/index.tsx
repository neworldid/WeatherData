import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import Cities from "./Cities";

let root = document.getElementById('root');
if (root !== null) {
    debugger;
    const reactRoot = createRoot(root);
    reactRoot.render(<App />);
} else {
    root = document.getElementById('cityRoot');
    if (root !== null) {
        const reactRoot = createRoot(root);
        reactRoot.render(<Cities />);
    }
}
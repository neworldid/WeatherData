import React from 'react';
import {createRoot} from 'react-dom/client';
import App from './App';
import Cities from "./Cities";
import './index.css'

let root = document.getElementById('root');
if (root !== null) {
	const reactRoot = createRoot(root);
	reactRoot.render(<App/>);
} else {
	root = document.getElementById('cityRoot');
	if (root !== null) {
		const reactRoot = createRoot(root);
		reactRoot.render(<Cities/>);
	}
}
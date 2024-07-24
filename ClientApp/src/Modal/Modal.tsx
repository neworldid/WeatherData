import React, {useEffect, useRef} from 'react';
import './Modal.css'
import ReactDOM from 'react-dom';

interface ModalProps {
	open: boolean;
	children: React.ReactNode;
}

const Modal: React.FC<ModalProps> = ({children, open}) => {
	const dialog = useRef<HTMLDialogElement>(null);

	useEffect(() => {
		if (open) {
			dialog.current?.showModal();
		} else {
			dialog.current?.close();
		}
	}, [open]);

	return ReactDOM.createPortal(
		<dialog ref={dialog}>{children}</dialog>,
		document.getElementById('modal-city-add')
	);
};

export default Modal;
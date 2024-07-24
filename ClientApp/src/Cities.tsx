import React, {useState} from 'react';
import Modal from "./Modal/Modal";
import Button from "./Button/Button";
import FieldSection from "./FieldSection";
import CityList from "./CityList";

export default function Cities() {
	const [modal, setModal] = useState(false);
	const closeModal = () => setModal(false);

	return (
		<section>
			<CityList/>
			<div>
				<Button onClick={() => setModal(true)} isActive>Add City</Button>
				<Modal open={modal}>
					<FieldSection closeModal={closeModal}/>
					<Button style={{display: 'block', marginLeft: 'auto'}}
							onClick={() => setModal(false)}>Закрыть</Button>
				</Modal>
			</div>
		</section>
	)
};
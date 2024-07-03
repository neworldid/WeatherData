import React, {useState} from 'react';
import Modal from "./Modal";

export default function Cities(){
    const [isOpen, setIsOpen] = useState(false);
   
    return (
        <section>
            <div>
                <button onClick={() => setIsOpen(true)}>Open Modal</button>
                <Modal isOpen={isOpen} onClose={() => setIsOpen(false)}>
                    <p>Modal Content Here</p>
                </Modal>
            </div>
        </section>
    )
};
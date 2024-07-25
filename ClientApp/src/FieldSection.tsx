import React, {useState} from 'react';
import Button from "./Button/Button";

interface FieldSectionProps {
	closeModal: () => void;
}

function isNullOrWhiteSpace(value: string | null | undefined): boolean {
	return value === null || value === undefined || value.trim() === '';
}

export default function FieldSection({closeModal}: FieldSectionProps) {
	const [cityName, setCityName] = useState('');
	const [error, setError] = useState(false);
	const [isLoading, setIsLoading] = useState(false);

	const addCity = async () => {
		setIsLoading(true);
		if (isNullOrWhiteSpace(cityName)) {
			setError(true);
			setIsLoading(false);
			return;
		}

		try {
			const response = await fetch('/Cities/AddCity', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify({cityName}),
			});

			const data = await response.json();
			if (data.success) {
				setCityName('');
				setError(false);
				closeModal();
				location.reload();
			} else {
				setError(true);
			}
		} catch (error) {
			setError(true);
		} finally {
			setIsLoading(false);
		}
	};

	return (
		<section>
			<h3>City adding</h3>

			<form style={{marginBottom: "1rem"}} onSubmit={(e) => e.preventDefault()}>
				<label htmlFor="cityInput">Input city</label>
				<input
					type="text"
					className="control"
					id="cityInput"
					value={cityName}
					style={{
						border: error ? '1px solid red' : null
					}}
					onChange={(e) => setCityName(e.target.value)}
				/>

				<Button onClick={addCity} isActive={!isLoading}>{isLoading ? 'Loading...' : 'Add'}</Button>

			</form>
		</section>
	)
}
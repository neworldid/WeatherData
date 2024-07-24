import React, {useState, useEffect} from 'react';

const CityList = () => {
	const [cities, setCities] = useState([]);
	const [isLoading, setIsLoading] = useState(false);

	useEffect(() => {
		const fetchCities = async () => {
			setIsLoading(true);
			try {
				const response = await fetch('/Cities/GetActualCities');
				if (!response.ok) {
					throw new Error('Network response was not ok');
				}
				const data = await response.json();
				setCities(data.data || []);
			} catch (error) {
				console.error('Failed to fetch cities:', error);
			} finally {
				setIsLoading(false);
			}
		};

		fetchCities();
	}, []);

	return (
		<div>
			<h2>Actual Cities</h2>
			{isLoading ? (
				<p>Loading...</p>
			) : (
				cities.length > 0 ? (
					<ul>
						{cities.map((city, index) => {
							return (
								<li className={"city-element"} key={index}>{city.cityName}, {city.country}</li>
							)
						})}
					</ul>
				) : (
					<p>No cities found.</p>
				)
			)}
		</div>
	);
};

export default CityList;
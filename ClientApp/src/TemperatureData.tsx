import React, {useState, useEffect} from 'react';
import {Bar} from 'react-chartjs-2';
import {
	Chart as ChartJS,
	CategoryScale,
	LinearScale,
	PointElement,
	LineElement,
	Title,
	Tooltip,
	Legend,
	BarElement
} from 'chart.js';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, BarElement);

interface Temperature {
	country: string;
	cityName: string;
	temperature: number;
	modified: Date;
}

const TemperatureData = () => {
	const [temperatureData, setTemperatureData] = useState<Temperature[]>([]);
	const [lastRefreshTime, setLastRefreshTime] = useState<string>('');

	const fetchTemperatureRecords = async () => {
		try {
			const response = await fetch('/Home/GetTemperatureRecords');
			if (!response.ok) {
				throw new Error('Network response was not ok');
			}
			const jsonResponse = await response.json();
			const data: Temperature[] = jsonResponse.data;
			setTemperatureData(data);
			setLastRefreshTime(new Date().toLocaleTimeString());
		} catch (error) {
			console.error('Failed to fetch temperature records:', error);
		}
	};

	useEffect(() => {
		const fetchConfigAndData = async () => {
			try {
				const intervalResponse = await fetch('/Cities/GetInterval');
				const updateIntervalInSeconds = await intervalResponse.json();

				fetchTemperatureRecords();
				const intervalId = setInterval(fetchTemperatureRecords, updateIntervalInSeconds * 1000);

				return () => clearInterval(intervalId);
			} catch (error) {
				console.error('Failed to fetch configuration:', error);
			}
		};

		fetchConfigAndData();
	}, []);

	// Prepare the data for the chart
	const labels = temperatureData.map(data => data.cityName);
	const temperatures = temperatureData.map(data => data.temperature);

	// Set up the chart data structure
	const data = {
		labels,
		datasets: [
			{
				label: 'Temperature',
				data: temperatures,
				backgroundColor: 'rgba(255, 99, 132, 0.2)',
				borderColor: 'rgba(255, 99, 132, 1)',
				borderWidth: 1,
			},
		],
	};

	return (
		<div>
			<h2>Temperature Overview</h2>
			<Bar data={data} options={{
				scales: {
					y: {
						beginAtZero: true,
						title: {
							display: true,
							text: 'Temperature (°C)'
						}
					}
				},
				plugins: {
					legend: {
						display: true,
						position: 'top',
					},
					title: {
						display: true,
						text: `City Temperatures - Last Refreshed: ${lastRefreshTime}`
					}
				}
			}}/>
		</div>
	);
};

export default TemperatureData;
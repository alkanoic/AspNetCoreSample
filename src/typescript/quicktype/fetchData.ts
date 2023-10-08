import axios from 'axios';
import { WeatherForecast } from './webapi';

async function fetchApiData(): Promise<WeatherForecast> {
  const response = await axios.get('https://127.0.0.1:7035/WeatherForecast');
  return response.data;
}

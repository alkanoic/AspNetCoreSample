import axios from 'axios';
import { components, paths } from './schema';

const BASE_URL = 'https://localhost:7035/api/';

export const getSomeResource = async (
  id: string
): Promise<components['schemas']['WeatherForecast']> => {
  try {
    const response = await axios.get<components['schemas']['WeatherForecast']>(
      `${BASE_URL}/WeatherForecast`
    );
    return response.data;
  } catch (error) {
    throw new Error(`Failed to fetch resource: ${error.message}`);
  }
};

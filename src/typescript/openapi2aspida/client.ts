import axios from 'axios';
import aspida from '@aspida/axios';
import api from './api/$api';

const client = api(aspida(axios, { baseURL: 'https://localhost:7035' }));

export default client;

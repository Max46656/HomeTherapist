import axios from 'axios';

const API_URL = 'https://localhost:5000/api/AvailableAppointments';

class AvailableAppointmentsService {
  async getAvailableDays(latitude, longitude, serviceId, date) {
    const response = await axios.get(API_URL +'getAvailableDays', {
      params: {
        latitude: latitude,
        longitude: longitude,
        serviceId: serviceId,
        date: date
      }
    });
    return response.data.data;
  }
}

export default AvailableAppointmentsService();

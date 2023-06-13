import React, { useState, useEffect } from 'react';
import AuthService from '../services/auth.service';
import { Calendar } from 'antd';
import { LayoutMarTop } from "./style";
import "../../src/css/styleTwo.css";


const UserAppointments = () => {
  const [appointments, setAppointments] = useState([]);
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
 const [selectedAppointment, setSelectedAppointment] = useState(null);

  useEffect(() => {
    const currentUser = AuthService.getCurrentUser();
    if (currentUser) {
      setUser(currentUser);
      setToken(currentUser.token);
    }
    const fetchAppointments = async () => {
      if (token) {
        const response = await AuthService.getAppointmentsByUser(token);
        if (response && response.data) {
          setAppointments(response.data);
        }
      }
    };
    fetchAppointments();
  }, [token]);

    const onDateSelect = (date) => {
      const selectedAppointments = appointments.filter(appointment =>
        appointment.startDt.startsWith(date.toISOString().split('T')[0])
      );
      setSelectedAppointment(selectedAppointments);
      console.log(selectedAppointments);
    };


const appointmentData = appointments.reduce((acc, appointment) => {
    acc[new Date(appointment.startDt).toDateString()] = appointment;
    return acc;
  }, {});




  return (
    <div>
    <LayoutMarTop />
    <br>

    </br>
      <h2>我的預約</h2>
    <Calendar
  dateCellRender={(date) => {
    const dateString = new Date(date).toDateString();
    if (appointmentData[dateString]) {
      const appointmentTime = new Date(appointmentData[dateString].startDt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
      return <div>{appointmentTime}</div>;
    }
  }}
  onSelect={onDateSelect}
/>
{selectedAppointment && (
  <div>
    <h3>預約詳細資料</h3>
    {selectedAppointment.map((appointment) => (
      <div key={appointment.id}>
        <p>顧客身份證字號：{appointment.customerId}</p>
        <p>顧客手機：{appointment.customerPhone}</p>
        <p>顧客地址：
          <a
            href={`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(appointment.customerAddress)}`}
            target="_blank"
            rel="noopener noreferrer"
          >
            {appointment.customerAddress}
          </a>
        </p>
        <p>顧客性別：{appointment.gender}</p>
        <p>顧客年齡：{appointment.ageGroup}</p>
      </div>
    ))}
  </div>
)}
    </div>
  );
};

export default UserAppointments;

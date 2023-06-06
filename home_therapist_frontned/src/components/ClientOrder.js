import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { LayoutMarTop } from './style';

const ClientOrder = () => {
  const [appointments, setAppointments] = useState([]);
  const [idNumber, setIdNumber] = useState('');
  const [phone, setPhone] = useState('');
  const [selectedAppointment, setSelectedAppointment] = useState(null);
  const [form, setForm] = useState({ CustomerId: '', CustomerPhone: '', CustomerAddress: '' });

  useEffect(() => {
    fetchAppointments();
  }, [idNumber, phone]);

  const fetchAppointments = () => {
    axios.get(`https://localhost:5000/Appointment/${idNumber}?Phone=${phone}`)
      .then((res) => {
        const { data } = res;
        if (data.isSuccess) {
          setAppointments(data.data);
        } else {
          alert(data.message);
        }
      })
      .catch((err) => console.log(err));
  };

  const deleteAppointment = (appointment) => {
    axios.delete(`https://localhost:5000/Appointment/${idNumber}?Phone=${phone}&date=${appointment.startDt}`)
      .then((res) => {
        const { data } = res;
        alert(data.message);
        fetchAppointments();
      })
      .catch((err) => console.log(err));
  };

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const response = await axios.patch(`https://localhost:5000/Appointment/${idNumber}`, form);
    console.log(response.data);
    fetchAppointments();
  };

  return (
    <div className='container mt-5'>
      <LayoutMarTop style={{height:"100px"}}/>
      <input
        type="text"
        value={idNumber}
        onChange={(e) => setIdNumber(e.target.value)}
        placeholder="身份證字號"
      />
      <input
        type="text"
        value={phone}
        onChange={(e) => setPhone(e.target.value)}
        placeholder="手機號碼"
      />
      <button className='btn-common' onClick={fetchAppointments}>查詢預約</button>
      {appointments.map((appointment, index) => (
        <div className='mt-3' key={index}>
          <p>本次預約時間:{appointment.appointment.startDt}</p>
          <form className='mb-5' onSubmit={handleSubmit}>
            <label>
              身份證字號:
              <input type="text" name="CustomerId" value={form.CustomerId} onChange={handleChange} />
            </label>
            <label>
              手機:
              <input type="text" name="CustomerPhone" value={form.CustomerPhone} onChange={handleChange} />
            </label>
            <label>
              地址:
              <input type="text" name="CustomerAddress" value={form.CustomerAddress} onChange={handleChange} />
            </label>
            <button className='btn-common' type="submit">更新預約</button>
          </form>
          <button className='btn-common' onClick={() => deleteAppointment(appointment.appointment)}>刪除預約</button>
        </div>
      ))}
      <div style={{height:"500px"}}></div>
    </div>
  );
};

export default ClientOrder;

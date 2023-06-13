import React, { useState } from 'react';
import { Form, Input, Button, Select, message } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import { useLocation } from 'react-router-dom';
import { LayoutMarTop } from "../style";
import "../.././css/styleTwo.css"

const { Item } = Form;
const { Option } = Select;
const ConfirmAppointment = () => {
  const location = useLocation();
  const { serviceId, latitude, longitude, selectedDate } = location.state;

  const [appointment, setAppointment] = useState(null);
  const [details, setDetails] = useState({
    ServiceId: serviceId,
    Latitude: latitude,
    Longitude: longitude,
    SelectedDate: selectedDate,
  });
  const [therapistInfo, setTherapistInfo] = useState(null);

  const handleSubmit = async (values) => {
    values = {
      ...values,
      serviceId: serviceId,
      latitude: latitude,
      longitude: longitude,
      selectedDate: selectedDate,
    };

    console.log(values);
    const response = await axios.post('https://localhost:5000/api/AvailableAppointments/createAppointment', values, {
      headers: {
        'Content-Type': 'application/json-patch+json',
      },
    });
    console.log(response.data);
    if (response.data.isSuccess) {
      setAppointment(response.data.data);
      const res = await axios.get(`https://localhost:5000/Photo/ProfilePhotoUrl?staffId=${response.data.data.userId}`);
      setTherapistInfo({
        photoUrl: "https://localhost:5000" + res.data,
        name: response.data.data.user.userName
      });
      console.log("https://localhost:5000" + res.data,response.data.data.user.userName)
    } else {
      message.error(response.data.message);
    }
  };

  const handleChange = (event) => {
    setDetails({
      ...details,
      [event.target.name]: event.target.value,
    });
  };

  return (
    <div>
      <LayoutMarTop />
      <div className="container py-md-5 ConfirmAppointment_sm vh-100" >

     {therapistInfo !== null && appointment !== null ? (
        <div>
          <img src={therapistInfo.photoUrl} alt="Therapist" />
          <p>治療師姓名: {therapistInfo.name}</p>
          <p>服務名稱: {appointment.serviceId}</p>
          <p>價格: {appointment.appointmentDetails[0]?.price}</p>
          <p>您的ID: {appointment.customerId}</p>
          <p>您的手機: {appointment.customerPhone}</p>
          <p>您的地址: {appointment.customerAddress}</p>
          <p>性別: {appointment.gender}</p>
          <p>年齡層: {appointment.ageGroup}</p>
          <p>備註: {appointment.appointmentDetails[0]?.note || "沒有留下備註"}</p>
          <p>請將此頁面截圖或紀錄下來，物理治療師將在近期與你聯絡!</p>
        </div>
      ) : (
        <Form className='py-md-5 row ' onFinish={handleSubmit} layout="vertical">
          <Item className=''  label="您的ID" name="customerId" rules={[{ required: true }]}>
            <Input placeholder="您的身份證字號" />
          </Item>
          <Item className='' label="您的手機" name="customerPhone" rules={[{ required: true }]}>
            <Input placeholder="您的手機" />
          </Item>
          <Item label="您的地址" name="customerAddress" rules={[{ required: true }]}>
            <Input placeholder="您的地址" />
          </Item>
          <Item label="性別" name="gender" rules={[{ required: true }]}>
            <Select placeholder="請選擇性別">
              <Option value="男">男</Option>
              <Option value="女">女</Option>
              <Option value="其他">其他</Option>
            </Select>
          </Item>
          <Item label="年齡層" name="ageGroup" rules={[{ required: true }]}>
            <Select placeholder="請選擇年齡層">
              <Option value="小於18">小於18</Option>
              <Option value="18-25">18-25</Option>
              <Option value="26-35">26-35</Option>
              <Option value="36-45">36-45</Option>
              <Option value="46-55">46-55</Option>
              <Option value="56-65">56-65</Option>
              <Option value="66-75">66-75</Option>
              <Option value="大於75">大於75</Option>
            </Select>
          </Item>
          <Item label="備註" name="note" initialValue="這位顧客沒有留下備註">
            <Input placeholder="備註" />
          </Item>
          <Item>
            <button className='btn color_2' type="primary" htmlType="submit">
              送出
            </button>
          </Item>
        </Form>
      )}
      </div>
    </div>
  );
};

export default ConfirmAppointment;

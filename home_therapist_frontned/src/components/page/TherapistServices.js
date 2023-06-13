import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import { Table, Button, message } from 'antd';
import AuthService from "../../services/auth.service";
import { LayoutMarTop } from "../style";
import "../../css/styleTwo.css";

const TherapistServices = () => {
  const [services, setServices] = useState([]);
  const [openServices, setOpenServices] = useState([]);

  const user = AuthService.getCurrentUser();
  const token = user.token;

  const fetchAllServices = useCallback(async () => {
    const response = await axios.get('https://localhost:5000/api/Service/GetAllServices');
    if (response.data.isSuccess) {
      setServices(response.data.data);
    } else {
      message.error(response.data.message);
    }
  }, []);

  const fetchOpenServices = useCallback(async () => {
    const response = await axios.get('https://localhost:5000/TherapistOpenService', {
      headers: {
        Authorization: token,
      },
    });
    if (response.data.isSuccess) {
      setOpenServices(response.data.data);
    } else {
      message.error(response.data.message);
    }
  }, [token]);

  useEffect(() => {
    fetchAllServices();
    fetchOpenServices();
  }, [fetchAllServices, fetchOpenServices]);

  const isServiceOpen = (serviceId) => {
    return openServices.some((service) => service.serviceId === serviceId);
  };

  const toggleService = async (serviceId) => {
    const isOpen = isServiceOpen(serviceId);

    if (isOpen) {
      const response = await axios.delete(`https://localhost:5000/TherapistOpenService?serviceId=${serviceId}`, {
        headers: {
          Authorization: token,
        },
      });
      if (response.data.isSuccess) {
        message.success('成功關閉該服務');
        fetchOpenServices();
      } else {
        message.error(response.data.message);
      }
    } else {
      const response = await axios.post(`https://localhost:5000/TherapistOpenService?serviceId=${serviceId}`, {}, {
        headers: {
          Authorization: token,
        },
      });
      if (response.data.isSuccess) {
        message.success('成功開放該服務');
        fetchOpenServices();
      } else {
        message.error(response.data.message);
      }
    }
  };

  const columns = [
    {
      title: '服務名稱',
      dataIndex: 'name',
      key: 'name',
      width: '33%',
    },
    {
      title: '價格',
      dataIndex: 'price',
      key: 'price',
      width: '33%',
    },
    {
      title: '',
      key: 'action',
      width: '33%',
      render: (text, record) => (
        <Button onClick={() => toggleService(record.key)}>
          {isServiceOpen(record.key) ? '已開啟' : '已關閉'}
        </Button>
      ),
    },
  ];

  const dataSource = services.map((service) => ({
    key: service.id,
    name: service.name,
    price: service.price,
  }));

  return (
    <div>
    <LayoutMarTop />
    <div className="container py-md-5 my-md-3 TherapistServices_sm vh-100">

    <Table
      dataSource={dataSource}
      columns={columns}
      pagination={false}
      bordered
      />
      </div>
    </div>
  );
};

export default TherapistServices;

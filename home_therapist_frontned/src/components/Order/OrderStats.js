import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Pie, Bar, Line } from 'react-chartjs-2';
import { Button } from 'antd';
import AuthService from '../../services/auth.service';
import { useNavigate } from 'react-router-dom';
import { ArcElement } from 'chart.js';
import ChartJS from 'chart.js/auto';
import { LayoutMarTop } from '../style';
import "../.././css/styleTwo.css";


ChartJS.register(ArcElement);

const OrderStats = () => {
  const [genderStats, setGenderStats] = useState({});
  const [ageGroupStats, setAgeGroupStats] = useState({});
  const [genderAndAgeStats, setGenderAndAgeStats] = useState({});
  const navigate = useNavigate();
  const user = AuthService.getCurrentUser();
  const token = user.token;

  useEffect(() => {
    const fetchGenderStats = async () => {
      try {
        const response = await axios.get('https://localhost:5000/Order/gender', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (response.data) {
          setGenderStats(response.data);
        }
      } catch (error) {
        console.error(error);
      }
    };

    const fetchAgeGroupStats = async () => {
      try {
        const response = await axios.get('https://localhost:5000/Order/agegroup', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.data) {
          setAgeGroupStats(response.data);
        }
      } catch (error) {
        console.error(error);
      }
    };

    const fetchGenderAndAgeStats = async () => {
      try {
        const response = await axios.get('https://localhost:5000/Order/genderAndAgeGroups', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.data) {
          setGenderAndAgeStats(response.data);
        }
      } catch (error) {
        console.error(error);
      }
    };

    fetchGenderStats();
    fetchAgeGroupStats();
    fetchGenderAndAgeStats();
  }, []);

  const genderData = {
    labels: Object.keys(genderStats).filter((key) => key !== '$id'),
    datasets: [
      {
        data: Object.values(genderStats).filter((value) => typeof value !== 'object'),
        backgroundColor: ['blue', 'red', 'green'],
      },
    ],
  };

  const ageGroupData = {
    labels: Object.keys(ageGroupStats).filter((key) => key !== '$id'),
    datasets: [
      {
        data: Object.values(ageGroupStats).filter((value) => typeof value !== 'object'),
        backgroundColor: ['purple'],
      },
    ],
  };

  const genderAndAgeData = {
  labels: Object.keys(genderAndAgeStats).filter((key) => key !== '$id'),
  datasets: [
    {
      data: Object.values(genderAndAgeStats).filter((value) => typeof value !== 'object'),
      backgroundColor: Object.keys(genderAndAgeStats).map((key) => {
        const gender = key.split('/')[0];
        if (gender === '男') {
          return 'blue';
        } else if (gender === '女') {
          return 'red';
        } else {
          return 'green';
        }
      }),
    },
  ],
};


  return (
    <div>
      <LayoutMarTop />
      <div style={{ display: 'flex', justifyContent: 'space-between' }}>
        <div>
          <Pie data={genderData}/>
        </div>
        <div>
          <Line data={ageGroupData}  height={400}/>
        </div>
        <div>
         <Bar data={genderAndAgeData} options={{ indexAxis: 'y' }} height={500} />
        </div>
      </div>
        <Button onClick={() => navigate('/MyOrder')}>我的訂單</Button>
        <Button onClick={() => navigate('/MyFeedback')}>我的評價</Button>
    </div>
  );
};

export default OrderStats;

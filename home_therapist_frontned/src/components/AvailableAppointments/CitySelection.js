import React, { useState, useEffect } from "react";
import { Form, Select, Button, Table } from "antd";
import { LayoutMarTop } from "../style";
// import AvailableAppointmentsService from '../services/AvailableAppointments.Service';
import axios from "axios";
import { OpenStreetMapProvider } from "leaflet-geosearch";
import { GeoSearchControl } from "react-leaflet-geosearch";
import { useMap } from "react-leaflet";
// import { useHistory } from 'react-router-dom';
import { useNavigate } from "react-router-dom";
import areaData from "../data/areaData";
import "../.././css/styleTwo.css"

const { Option } = Select;

const CitySelection = () => {
  const [selectedCity, setSelectedCity] = useState("");
  const [selectedTownship, setSelectedTownship] = useState("");
  const [townshipOptions, setTownshipOptions] = useState([]);
  const [services, setServices] = useState([]);
  const [selectedService, setSelectedService] = useState("");
  const [selectedDate, setSelectedDate] = useState("");
  const [latitude, setLatitude] = useState(null);
  const [longitude, setLongitude] = useState(null);
  const provider = new OpenStreetMapProvider();

  const navigate = useNavigate();
  const [availableDays, setAvailableDays] = useState([]);

  useEffect(() => {
    const fetchServices = async () => {
      const response = await axios.get(
        "https://localhost:5000/api/Service/GetAllServices"
      );
      const serviceDescriptions = {
        疼痛管理: "提供專業的疼痛管理服務，幫助患者緩解和控制疼痛感。",
        日常活動訓練:
          "透過日常活動的指導和訓練，\n幫助患者恢復或改善日常生活中的功能和活動能力。",
        環境最佳化:
          "評估和優化患者所處環境，包括家庭和社區，\n以提供最適合患者需求的支持和便利。",
        環境安全與適應性評估:
          "評估患者所處環境的安全性和適應性，\n提供必要的建議和改善措施，確保患者的居住和生活環境符合其需求和安全要求。",
        家庭照顧者訓練:
          "為家庭照顧者提供必要的培訓和指導，\n幫助他們更好地照顧和支持患者，同時提供必要的知識和技能。"
      };

      const servicesWithDescriptions = response.data.data.map((service) => ({
        ...service,
        description: serviceDescriptions[service.name]
      }));
      setServices(servicesWithDescriptions);
    };
    fetchServices();
  }, []);

  const handleFormSubmit = async () => {
    try {
      const response = await axios.get(
        "https://localhost:5000/api/AvailableAppointments/getAvailableDays",
        {
          params: {
            latitude: latitude,
            longitude: longitude,
            serviceId: selectedService,
            date: selectedDate
          }
        }
      );

      // console.log(selectedCity,selectedService," Latitude Longitude "+latitude,longitude,selectedDate);
       console.log(response.data);
      if (response.data.isSuccess) {
        setAvailableDays(response.data.data);
        // 跳轉點

        navigate("/available-dates", {
          state: {
            availableDays: response.data.data,
            latitude: latitude,
            longitude: longitude,
            serviceId: selectedService
          }
        });
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleCityChange = (value) => {
    setSelectedCity(value);
    setSelectedTownship("");
    const selectedCityData = areaData.find((item) => item.city === value);
    setTownshipOptions(selectedCityData ? selectedCityData.townships : []);
  };

  const handleTownshipChange = async (value) => {
    setSelectedTownship(value);

    const Location = await provider.search({
      query: `${selectedCity} ${value}`
    });
    if (Location && Location.length > 0) {
      setLatitude(Location[0].y);
      setLongitude(Location[0].x);
      // console.log(selectedService);
      // console.log(selectedCity,value," Latitude Longitude "+results[0].y,results[0].x);
    }
  };

  const handleServiceSelection = (serviceId) => {
    setSelectedService(serviceId);
  };

  const handleDateChange = (value) => {
    const selectedDateTime = new Date(value);
    const year = selectedDateTime.getFullYear();
    const month = (selectedDateTime.getMonth() + 1).toString().padStart(2, "0");
    const day = selectedDateTime.getDate().toString().padStart(2, "0");
    const hour = selectedDateTime.getHours().toString().padStart(2, "0");
    const minute = selectedDateTime.getMinutes().toString().padStart(2, "0");
    const second = selectedDateTime.getSeconds().toString().padStart(2, "0");
    const formattedDate = `${year}-${month}-${day} ${hour}:${minute}:${second}`;

    setSelectedDate(formattedDate);
  };

  const getCurrentMonth = () => {
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth() + 1;
    return currentMonth;
  };

  const getCurrentYear = () => {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    return currentYear;
  };

  const generateYearMonthOptions = () => {
    const currentYear = getCurrentYear();
    const currentMonth = getCurrentMonth();
    const options = [];
    // 嚴格來說有錯，但現在是對的。
    const startDate = new Date(2023, 4); // May 2023 (month is 0-based)
    for (
      let date = startDate;
      date <= new Date(2024, 11);
      date.setMonth(date.getMonth() + 1)
    ) {
      const year = date.getFullYear();
      const month = date.getMonth() + 1;
      const optionValue = `${year}-${month.toString().padStart(2, "0")}`;
      const optionLabel = `${year}年${month}月`;
      options.push(
        <Option key={optionValue} value={optionValue}>
          {optionLabel}
        </Option>
      );
    }

    return options;
  };

  const ServicesColumns = [
    {
      title: "名稱",
      dataIndex: "name",
      key: "name"
    },
    {
      title: "描述",
      dataIndex: "description",
      key: "description",
      render: (text) => {
        const lines = text.split("\n");
        return lines.map((line, index) => (
          <div  key={index.toString()}>
            {line}
          </div>
        ));
      }
    },
    {
      title: "價格",
      dataIndex: "price",
      key: "price"
    },
    {
      title: "選擇服務",
      dataIndex: "id",
      key: "id",
      render: (id) => (
        <Button
          type={selectedService === id ? "primary" : "default"}
          onClick={() => handleServiceSelection(id)}
        >
          選擇
        </Button>
      )
    }
  ];
  return (
    <div className="p-0 m-0 overflow-hidden">
      <LayoutMarTop />
      <div className="row d-flex justify-content-center flex-nowrap">
        <Form className="col-lg-9 col-md-10 ">
          <br />
          <br />
          <div className="mb-5">

          <Form.Item className=" city_sm_col" label="縣市">
            <Select value={selectedCity} onChange={handleCityChange}>
              {areaData.map((item) => (
                <Option key={item.city} value={item.city}>
                  {item.city}
                </Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item className=" city_sm_col" label="鄉鎮區">
            <Select value={selectedTownship} onChange={handleTownshipChange}>
              {selectedCity &&
                areaData
                  .find((item) => item.city === selectedCity)
                  ?.townships.map((township) => (
                    <Option key={township} value={township}>
                      {township}
                    </Option>
                  ))}
            </Select>
          </Form.Item>
          </div>
          <div className="city_table ">
            <Table
              className=""
              style={{}}
              dataSource={services}
              columns={ServicesColumns}
              rowKey="id"
            />
            {/* <Form.Item label="服務">
  <Table
    dataSource={services}
    columns={[
      { title: '名稱', dataIndex: 'name', key: 'name' },
      { title: '描述', dataIndex: 'description', key: 'description' },
      { title: '價格', dataIndex: 'price', key: 'price' },
      {
        title: '選擇的服務',
        dataIndex: 'id',
        key: 'id',
        render: (id) => (
          <Button
            type={selectedService === id ? 'primary' : 'default'}
            onClick={() => handleServiceSelection(id)}
          >
            選擇
          </Button>
        ),
      },
    ]}
  />
</Form.Item> */}

          </div>
          <div className=" city_sm_col mb-5">

            <Form.Item  label="年份與月份">
              <Select onChange={handleDateChange}>
                {generateYearMonthOptions()}
              </Select>
            </Form.Item>
            <Form.Item>
              <Button type="btn color_2 " onClick={handleFormSubmit}>
                送出
              </Button>
            </Form.Item>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default CitySelection;

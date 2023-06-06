import React, { useState,useEffect  } from 'react';
import { Form, Select, Button, Table } from 'antd';
import { LayoutMarTop } from "../style";
// import AvailableAppointmentsService from '../services/AvailableAppointments.Service';
import axios from 'axios';
import { OpenStreetMapProvider } from 'leaflet-geosearch';
import { GeoSearchControl } from 'react-leaflet-geosearch';
import { useMap } from 'react-leaflet';
// import { useHistory } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';



const { Option } = Select;

const CitySelection = () => {
  const [selectedCity, setSelectedCity] = useState('');
  const [selectedTownship, setSelectedTownship] = useState('');
  const [townshipOptions, setTownshipOptions] = useState([]);
  const [services, setServices] = useState([]);
  const [selectedService, setSelectedService] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [latitude, setLatitude] = useState(null);
  const [longitude, setLongitude] = useState(null);
  const provider = new OpenStreetMapProvider();
  const areaData = [
    {
      city: '臺北市',
      townships: [
        '中正區', '大同區', '中山區', '萬華區', '信義區', '松山區', '大安區', '南港區', '北投區', '內湖區', '士林區', '文山區'
      ]
    },
    {
      city: '新北市',
      townships: [
        '板橋區', '新莊區', '泰山區', '林口區', '淡水區', '金山區', '八里區', '萬里區', '石門區', '三芝區', '瑞芳區', '汐止區', '平溪區', '貢寮區', '雙溪區', '深坑區', '石碇區', '新店區', '坪林區', '烏來區', '中和區', '永和區', '土城區', '三峽區', '樹林區', '鶯歌區', '三重區', '蘆洲區', '五股區'
      ]
    },
    {
      city: '基隆市',
      townships: [
        '仁愛區', '中正區', '信義區', '中山區', '安樂區', '暖暖區', '七堵區'
      ]
    },
    {
      city: '桃園市',
      townships: [
        '桃園區', '中壢區', '平鎮區', '八德區', '楊梅區', '蘆竹區', '龜山區', '龍潭區', '大溪區', '大園區', '觀音區', '新屋區', '復興區'
      ]
    },
    {
      city: '新竹縣',
      townships: [
        '竹北市', '竹東鎮', '新埔鎮', '關西鎮', '峨眉鄉', '寶山鄉', '北埔鄉', '橫山鄉', '芎林鄉', '湖口鄉', '新豐鄉', '尖石鄉', '五峰鄉'
      ]
    },
    {
      city: '新竹市',
      townships: [
        '東區', '北區', '香山區'
      ]
    },
    {
      city: '苗栗縣',
      townships: [
        '苗栗市', '通霄鎮', '苑裡鎮', '竹南鎮', '頭份鎮', '後龍鎮', '卓蘭鎮', '西湖鄉', '頭屋鄉', '公館鄉', '銅鑼鄉', '三義鄉', '造橋鄉', '三灣鄉', '南庄鄉', '大湖鄉', '獅潭鄉', '泰安鄉'
      ]
    },
    {
      city: '臺中市',
      townships: [
        '中區', '東區', '南區', '西區', '北區', '北屯區', '西屯區', '南屯區', '太平區', '大里區', '霧峰區', '烏日區', '豐原區', '后里區', '東勢區', '石岡區', '新社區', '和平區', '神岡區', '潭子區', '大雅區', '大肚區', '龍井區', '沙鹿區', '梧棲區', '清水區', '大甲區', '外埔區', '大安區'
      ]
    },
    {
      city: '南投縣',
      townships: [
        '南投市', '埔里鎮', '草屯鎮', '竹山鎮', '集集鎮', '名間鄉', '鹿谷鄉', '中寮鄉', '魚池鄉', '國姓鄉', '水里鄉', '信義鄉', '仁愛鄉'
      ]
    },
    {
      city: '彰化縣',
      townships: [
        '彰化市', '員林鎮', '和美鎮', '鹿港鎮', '溪湖鎮', '二林鎮', '田中鎮', '北斗鎮', '花壇鄉', '芬園鄉', '大村鄉', '永靖鄉', '伸港鄉', '線西鄉', '福興鄉', '秀水鄉', '埔心鄉', '埔鹽鄉', '大城鄉', '芳苑鄉', '竹塘鄉', '社頭鄉', '二水鄉', '田尾鄉', '埤頭鄉', '溪州鄉'
      ]
    },
    {
      city: '雲林縣',
      townships: [
        '斗六市', '斗南鎮', '虎尾鎮', '西螺鎮', '土庫鎮', '北港鎮', '莿桐鄉', '林內鄉', '古坑鄉', '大埤鄉', '崙背鄉', '二崙鄉', '麥寮鄉', '臺西鄉', '東勢鄉', '褒忠鄉', '四湖鄉', '口湖鄉', '水林鄉', '元長鄉'
      ]
    },
    {
      city: '嘉義縣',
      townships: [
        '太保市', '朴子市', '布袋鎮', '大林鎮', '民雄鄉', '溪口鄉', '新港鄉', '六腳鄉', '東石鄉', '義竹鄉', '鹿草鄉', '水上鄉', '中埔鄉', '竹崎鄉', '梅山鄉', '番路鄉', '大埔鄉', '阿里山鄉'
      ]
    },
    {
      city: '嘉義市',
      townships: [
        '東區', '西區'
      ]
    },
    {
      city: '臺南市',
      townships: [
        '中西區', '東區', '南區', '北區', '安平區', '安南區', '永康區', '歸仁區', '新化區', '左鎮區', '玉井區', '楠西區', '南化區', '仁德區', '關廟區', '龍崎區', '官田區', '麻豆區', '佳里區', '西港區', '七股區', '將軍區', '學甲區', '北門區', '新營區', '後壁區', '白河區', '東山區', '六甲區', '下營區', '柳營區', '鹽水區', '善化區', '大內區', '山上區', '新市區', '安定區'
      ]
    },
    {
      city: '高雄市',
      townships: [
        '楠梓區', '左營區', '鼓山區', '三民區', '鹽埕區', '前金區', '新興區', '苓雅區', '前鎮區', '小港區', '旗津區', '鳳山區', '大寮區', '鳥松區', '林園區', '仁武區', '大樹區', '大社區', '岡山區', '路竹區', '橋頭區', '梓官區', '彌陀區', '永安區', '燕巢區', '田寮區', '阿蓮區', '茄萣區', '湖內區', '旗山區', '美濃區', '內門區', '杉林區', '甲仙區', '六龜區', '茂林區', '桃源區', '那瑪夏區'
      ]
    },
    {
      city: '屏東縣',
      townships: [
        '屏東市', '潮州鎮', '東港鎮', '恆春鎮', '萬丹鄉', '長治鄉', '麟洛鄉', '九如鄉', '里港鄉', '鹽埔鄉', '高樹鄉', '萬巒鄉', '內埔鄉', '竹田鄉', '新埤鄉', '枋寮鄉', '新園鄉', '崁頂鄉', '林邊鄉', '南州鄉', '佳冬鄉', '琉球鄉', '車城鄉', '滿州鄉', '枋山鄉', '霧台鄉', '瑪家鄉', '泰武鄉', '來義鄉', '春日鄉', '獅子鄉', '牡丹鄉', '三地門鄉'
      ]
    },
    {
      city: '宜蘭縣',
      townships: [
        '宜蘭市', '羅東鎮', '蘇澳鎮', '頭城鎮', '礁溪鄉', '壯圍鄉', '員山鄉', '冬山鄉', '五結鄉', '三星鄉', '大同鄉', '南澳鄉'
      ]
    },
    {
      city: '花蓮縣',
      townships: [
        '花蓮市', '鳳林鎮', '玉里鎮', '新城鄉', '吉安鄉', '壽豐鄉', '秀林鄉', '光復鄉', '豐濱鄉', '瑞穗鄉', '萬榮鄉', '富里鄉', '卓溪鄉'
      ]
    },
    {
      city: '臺東縣',
      townships: [
        '臺東市', '成功鎮', '關山鎮', '長濱鄉', '海端鄉', '池上鄉', '東河鄉', '鹿野鄉', '延平鄉', '卑南鄉', '金峰鄉', '大武鄉', '達仁鄉', '綠島鄉', '蘭嶼鄉', '太麻里鄉'
      ]
    },
    {
      city: '澎湖縣',
      townships: [
        '馬公市', '湖西鄉', '白沙鄉', '西嶼鄉', '望安鄉', '七美鄉'
      ]
    },
    {
      city: '金門縣',
      townships: [
        '金城鎮', '金湖鎮', '金沙鎮', '金寧鄉', '烈嶼鄉', '烏坵鄉'
      ]
    },
    {
      city: '連江縣',
      townships: [
        '南竿鄉', '北竿鄉', '莒光鄉', '東引鄉'
      ]
    }
  ];
  const navigate = useNavigate();
  const [availableDays, setAvailableDays] = useState([]);

useEffect(() => {
  const fetchServices = async () => {
    const response = await axios.get('https://localhost:5000/api/Service/GetAllServices');
    const serviceDescriptions = {
  '疼痛管理': '提供專業的疼痛管理服務，幫助患者緩解和控制疼痛感。',
  '日常活動訓練': '透過日常活動的指導和訓練，\n幫助患者恢復或改善日常生活中的功能和活動能力。',
  '環境最佳化': '評估和優化患者所處環境，包括家庭和社區，\n以提供最適合患者需求的支持和便利。',
  '環境安全與適應性評估': '評估患者所處環境的安全性和適應性，\n提供必要的建議和改善措施，確保患者的居住和生活環境符合其需求和安全要求。',
  '家庭照顧者訓練': '為家庭照顧者提供必要的培訓和指導，\n幫助他們更好地照顧和支持患者，同時提供必要的知識和技能。',
};

    const servicesWithDescriptions = response.data.data.map((service) => ({
    ...service,
    description: serviceDescriptions[service.name],
  }));
    setServices(servicesWithDescriptions);
  };
  fetchServices();
}, []);

const handleFormSubmit = async () => {
  try {
    const response = await axios.get('https://localhost:5000/api/AvailableAppointments/getAvailableDays', {
      params: {
        latitude: latitude,
        longitude: longitude,
        serviceId: selectedService,
        date: selectedDate
      }
    });
    // console.log(selectedCity,selectedService," Latitude Longitude "+latitude,longitude,selectedDate);
    // console.log(response.data);
    if (response.data.isSuccess) {
      setAvailableDays(response.data.data);
      // 跳轉點

      navigate('/available-dates', { state: {
        availableDays:response.data.data,
        latitude:latitude,
        longitude:longitude,
        serviceId:selectedService} });

    }
  } catch (error) {
      console.error(error);
  }
};

const handleCityChange = (value) => {
    setSelectedCity(value);
    setSelectedTownship('');
    const selectedCityData = areaData.find((item) => item.city === value);
    setTownshipOptions(selectedCityData ? selectedCityData.townships : []);
  };

  const handleTownshipChange = async (value) => {
    console.log(latitude,longitude);
  setSelectedTownship(value);

  const Location = await provider.search({ query: `${selectedCity} ${value}` });
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
  const month = (selectedDateTime.getMonth() + 1).toString().padStart(2, '0');
  const day = selectedDateTime.getDate().toString().padStart(2, '0');
  const hour = selectedDateTime.getHours().toString().padStart(2, '0');
  const minute = selectedDateTime.getMinutes().toString().padStart(2, '0');
  const second = selectedDateTime.getSeconds().toString().padStart(2, '0');
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
   for (let date = startDate; date <= new Date(2024, 11); date.setMonth(date.getMonth() + 1)) {
    const year = date.getFullYear();
    const month = date.getMonth() + 1;
    const optionValue = `${year}-${month.toString().padStart(2, '0')}`;
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
    title: '名稱',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '描述',
    dataIndex: 'description',
    key: 'description',
    render: (text) => {
      const lines = text.split('\n');
      return lines.map((line, index) => <div key={index.toString()}>{line}</div>);
    },
  },
  {
    title: '價格',
    dataIndex: 'price',
    key: 'price',
  },
  {
    title: '選擇服務',
    dataIndex: 'id',
    key: 'id',
           render: (id) => (
          <Button className='btn-common'
            type={selectedService === id ? 'primary' : 'default'}
            onClick={() => handleServiceSelection(id)}
          >
            選擇
          </Button>
        ),
  },
];
  return (
    <div>
      <LayoutMarTop />
      <div className="container">

      <Form>
      <br/><br/>
        <Form.Item label="縣市">
          <Select value={selectedCity} onChange={handleCityChange}>
            {areaData.map((item) => (
              <Option key={item.city} value={item.city}>
                {item.city}
              </Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item label="鄉鎮區">
          <Select value={selectedTownship} onChange={handleTownshipChange}>
            {townshipOptions.map((township) => (
              <Option key={township} value={township}>
                {township}
              </Option>
            ))}
          </Select>
        </Form.Item>
<Table dataSource={services} columns={ServicesColumns} rowKey="id"/>
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

        <Form.Item label="年份與月份">
          <Select onChange={handleDateChange}>
            {generateYearMonthOptions()}
          </Select>
        </Form.Item>
        <Form.Item>
          <Button className='btn-common' type="primary" onClick={handleFormSubmit}>
            送出
          </Button>
        </Form.Item>
      </Form>
      </div>
    </div>
  );
};

export default CitySelection;
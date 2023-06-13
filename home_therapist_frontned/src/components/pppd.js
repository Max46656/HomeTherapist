// import React, { useState, useEffect } from 'react';
// import { TimePicker } from '@material-ui/pickers';
// import { MapContainer, TileLayer, Marker } from 'react-leaflet';
// import { OpenStreetMapProvider } from 'leaflet-geosearch';
// import 'leaflet/dist/leaflet.css';
// import axios from 'axios';

// const PPd = () => {
//     const [position, setPosition] = useState(null);
//     const [services, setServices] = useState([]);
//     const [serviceId, setServiceId] = useState('');
//     const [customerId, setCustomerId] = useState('');
//     const [customerPhone, setCustomerPhone] = useState('');
//     const [customerAddress, setCustomerAddress] = useState('');
//     const [note, setNote] = useState('');
//     const [chosenDate, setChosenDate] = useState(null);
  
//     useEffect(() => {
//       const fetchServices = async () => {
//         const response = await axios.get('api/Service/GetAllServiceNames');
//         setServices(response.data.data);
//       };
//       fetchServices();
//     }, []);
  
//     const fetchAvailableDates = async () => {
//       try {
//         const response = await axios.get('/getAvailableDays', {
//           params: {
//             lat: position.lat,
//             lon: position.lng,
//             serviceId,
//             month: chosenDate.getMonth() + 1, // 加1以獲取正確的月份值 (0-11)
//           },
//         });
//         setAvailableDates(response.data);
//       } catch (error) {
//         console.error(error);
//       }
//     };
  
//     const fetchAvailableTimes = async () => {
//       try {
//         const response = await axios.get('/getAvailableDatetime', {
//           params: {
//             lat: position.lat,
//             lon: position.lng,
//             serviceId,
//             date: chosenDate,
//           },
//         });
//         setAvailableTimes(response.data);
//       } catch (error) {
//         console.error(error);
//       }
//     };
  
//     const createAppointment = async () => {
//       try {
//         await axios.post('/api/Appointment/CreateAppointmentAsync', {
//           serviceId: serviceId,
//           customerId: customerId,
//           customerPhone: customerPhone,
//           customerAddress: customerAddress,
//           note: note,
//           selectedDate: chosenDate,
//           latitude: position ? position.lat : null,
//           longitude: position ? position.lng : null,
//         });
//       } catch (error) {
//         console.error(error);
//       }
//     };
  
//     return (
//       <div>
//         <h1>預約</h1>
//         <MapContainer center={[51.505, -0.09]} zoom={13} style={{ height: "300px", width: "300px" }}>
//           <TileLayer
//             url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
//           />
//           {position && <Marker position={position} />}
//           <Geosearch provider={new OpenStreetMapProvider()} showMarker={false} showPopup={false} popupFormat={({ query }) => query} />
//         </MapContainer>
//         <select value={serviceId} onChange={(e) => setServiceId(e.target.value)}>
//           {services.map((service, index) => (
//             <option key={index} value={service}>{service}</option>
//           ))}
//         </select>
//         <input type="text" placeholder="客戶ID" value={customerId} onChange={(e) => setCustomerId(e.target.value)} />
//         <input type="text" placeholder="客戶電話" value={customerPhone} onChange={(e) => setCustomerPhone(e.target.value)} />
//         <input type="text" placeholder="客戶地址" value={customerAddress} onChange={(e) => setCustomerAddress(e.target.value)} />
//         <input type="text" placeholder="備註" value={note} onChange={(e) => setNote(e.target.value)} />
//         <TimePicker
//           value={chosenDate}
//           onChange={(date) => setChosenDate(date)}
//           minutesStep={5}
//         />
//         <button onClick={fetchAvailableDates}>獲取可用日期</button>
//         <select value={chosenDate} onChange={(e) => setChosenDate(e.target.value)}>
//           {availableDates.map((date) => (
//             <option value={date}>{date}</option>
//           ))}
//         </select>
//         <button onClick={fetchAvailableTimes}>獲取可用時間</button>
//         <TimePicker
//           value={chosenTime}
//           onChange={(time) => setChosenTime(time)}
//           minutesStep={5}
//         />
//         <button onClick={createAppointment}>創建預約</button>
//       </div>
//     );
//   };

// export default PPd;
import React,{useState,useEffect} from 'react'
import {LayoutMarTop} from "./style";
import { Calendar, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';


const localizer = momentLocalizer(moment);
// 使用 Moment.js 作為本地化函數庫


const EventComponent = ({ event }) => {
  return (
    <div>
      <strong>{event.title}</strong>
      <button onClick={() => handleEventClick(event)}
      >管理</button>
    </div>
  );
};
const handleEventClick = (event) => {
  alert(`點擊了事件：${event.title}`);
};

// const events = [
//   {
//     title: 'Appointment 1',
//     start: new Date(),
//     end: new Date(moment().add(1, 'hours')),
//   },
//   {
//     title: 'Appointment 2',
//     start: new Date(moment().add(2, 'days')),
//     end: new Date(moment().add(2, 'days').add(2, 'hours')),
//   },
// ];

const ProfileAppoint = () => {
  const [userInfo, setUserInfo] = useState(null);
  const [events, setEvents] = useState([]);

  useEffect(() => {
    fetchEvents();
    fetchUserInfo();
  }, []);

    const fetchEvents = async () => {
      try {
        const response = await fetch('https://localhost:5000/User/GetAppointmentsByUser');
        const data = await response.json();

        const formattedEvents = data.map(event => ({
          title: event.title,
          start: new Date(event.start),
          end: new Date(event.end)
        }));

        setEvents(formattedEvents);
      } catch (error) {
        console.error('Error fetching events:', error);
      }
    }
  const fetchUserInfo = async () => {
    try {
      const response = await fetch('https://localhost:5000/User');
      const data = await response.json();

      setUserInfo(data);
    } catch (error) {
      console.error('Error fetching user info:', error);
    }
  };

  return (
    <div>
      <div style={{ padding: "3rem" }}>
      <LayoutMarTop/>
      {userInfo && (
          <div>
            <h2>管理者資訊</h2>
            <p>姓名：{userInfo.name}</p>
            <p>Email:{userInfo.email}</p>
          </div>
         )}
      <div>
    <Calendar
      localizer={localizer}
      events={events}
      startAccessor="start"
      endAccessor="end"
      style={{ height: 500 }}
      components={{
          event: EventComponent
        }}
    />
  </div>
        {/* <button
        // onClick={handleSerch}
        className="btn btn-primary">
          <span>查詢預約</span>
        </button> */}
      </div>
    </div>

  )
}

export default ProfileAppoint;
import React, { useState, useEffect } from 'react';
import { Calendar, TimePicker, Input, message, Button } from 'antd';
import { useLocation } from 'react-router-dom';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-tw';
import locale from 'antd/locale/zh_CN';
import axios from 'axios';
import { LayoutMarTop } from "../style";
import weekday from "dayjs/plugin/weekday";
import localeData from "dayjs/plugin/localeData";
import { useNavigate } from 'react-router-dom';

dayjs.extend(weekday)
dayjs.extend(localeData)
dayjs.locale('zh-tw');

const format = 'HH:mm';

const AvailableDates = () => {
  const [availableDays, setAvailableDays] = useState([]);
  const [selectedDate, setSelectedDate] = useState(null);
  const [selectedTime, setSelectedTime] = useState(null);
  const [availableTimes, setAvailableTimes] = useState([]);

  const navigate = useNavigate();
  const location = useLocation();

  if (availableDays.length === 0) {
    setAvailableDays(location.state.availableDays);
  }

  const latitude = location.state.latitude;
  const longitude = location.state.longitude;
  const serviceId = location.state.serviceId;

  const availableMonth = dayjs(availableDays[0]).format('YYYY-MM');

  const fullCellRender = (date) => {
    const style = {};
    if (availableDays.find((day) => dayjs(day).isSame(date, 'day'))) {
      style.border = '1px solid #5dff18';
      style.borderRadius = '50%';
    }
    return (
      <div className="ant-picker-cell-inner" style={style}>
        {date.date()}
      </div>
    );
  };

  const disabledDate = (current) => {
    return current && current < dayjs().endOf('day');
  };

  const handleMonthChange = async (date) => {
    try {
      const selectedDateTime = new Date(date);
      const year = selectedDateTime.getFullYear();
      const month = (selectedDateTime.getMonth() + 1).toString().padStart(2, '0');
      const day = selectedDateTime.getDate().toString().padStart(2, '0');
      const hour = selectedDateTime.getHours().toString().padStart(2, '0');
      const minute = selectedDateTime.getMinutes().toString().padStart(2, '0');
      const second = selectedDateTime.getSeconds().toString().padStart(2, '0');
      const startOfMonth = `${year}-${month}-${day} ${hour}:${minute}:${second}`;

      const response = await axios.get('https://localhost:5000/api/AvailableAppointments/getAvailableDays', {
        params: {
          latitude: latitude,
          longitude: longitude,
          serviceId: serviceId,
          date: startOfMonth,
        }
      });

      if (response.data.isSuccess) {
        setAvailableDays(response.data.data);
      } else {
        message.error('抓取可預約的日期失敗！');
      }
    } catch (error) {
      console.error(error);
      message.error('發生錯誤，請稍後再試！');
    }
  };

 const handleDateSelect = async (date) => {
    setSelectedDate(date);
    // 在選擇日期後，根據所選日期獲取可用時間
     const formattedTime = dayjs(date).format('YYYY-MM-DDTHH:mm:ss');
    //  console.log(formattedTime);
    const url = `https://localhost:5000/api/AvailableAppointments/getAvailableDatetime?latitude=${latitude}&longitude=${longitude}&serviceId=${serviceId}&date=${formattedTime}`;
    const response = await axios.get(url);
    if (response.data.isSuccess) {
      setAvailableTimes(response.data.data);
    } else {
      message.error('抓取可用的時間失敗！');
    }
  };

  const handleTimeSelect = (time) => {
    setSelectedTime(time);
  };

  const handleConfirmAppointment = () => {
    const formattedDateTime = dayjs(selectedDate)
    .set('hour', dayjs(selectedTime).hour())
    .set('minute', dayjs(selectedTime).minute())
    .set('second', dayjs(selectedTime).second())
    .format('YYYY-MM-DD HH:mm:ss');
  // 跳轉點
  // console.log(formattedDateTime);
  navigate('/confirm-appointment', {
    state: {
      latitude: latitude,
      longitude: longitude,
      serviceId: serviceId,
      selectedDate: formattedDateTime,
    }
  });
};

  const AvailableTimes = ({ availableTimes }) => {
    const availableHours = availableTimes.map((time) => dayjs(time).hour());

    const disabledHours = () => {
      const hours = [];
      for (let i = 0; i < 24; i++) {
        if (!availableHours.includes(i)) {
          hours.push(i);
        }
      }
      return hours;
    };

    const disabledMinutes = (selectedHour) => {
      if (!availableHours.includes(selectedHour)) {
        return Array.from({ length: 60 }).map((_, index) => index);
      }

      const availableMinutes = availableTimes
        .filter((time) => dayjs(time).hour() === selectedHour)
        .map((time) => dayjs(time).minute());

      const disabledMinutes = Array.from({ length: 60 })
        .map((_, index) => index)
        .filter((minute) => !availableMinutes.includes(minute));

      return disabledMinutes;
    };

    return (
      <TimePicker
        disabledHours={disabledHours}
        disabledMinutes={disabledMinutes}
        format={format}
        hideDisabledOptions
        allowClear={false}
        onChange={handleTimeSelect}
      />
    );
  };

  return (
    <div>
      <LayoutMarTop />
      <p>綠色為可選擇日期，先選擇日期再選擇時間。</p>
      <Calendar
        locale={'zh-tw'}
        fullscreen={false}
        defaultValue={dayjs(availableMonth)}
        fullCellRender={fullCellRender}
        disabledDate={disabledDate}
        onSelect={handleDateSelect}
        onPanelChange={handleMonthChange}
      />
      {selectedDate && (
        <AvailableTimes availableTimes={availableTimes} />
      )}
      <Button type="primary" onClick={handleConfirmAppointment} disabled={!selectedDate || !selectedTime}>
        確定預約
      </Button>
    </div>
  );
};

export default AvailableDates;

import React, { useState, useEffect } from 'react';
import { Calendar, TimePicker, message, Modal, Button } from 'antd';
import dayjs from 'dayjs';
import axios from 'axios';
import { LayoutMarTop } from "../style";
import AuthService from "../../services/auth.service";

dayjs.locale('zh-tw'); // 設定 Day.js 使用繁體中文

const TherapistOpenTime = () => {
  const [openTimes, setOpenTimes] = useState([]);
  const [open, setOpen] = useState(false);
  const [selectedTime, setSelectedTime] = useState(null);
  const [selectedDate, setSelectedDate] = useState(null);
  const [deleteMode, setDeleteMode] = useState(false);

  const user = AuthService.getCurrentUser();
  const token = user.token;

  // 格式化行事曆上的開放時間
  const formatOpenTimes = (list) => {
    const formatted = {};
    list.forEach(item => {
      const date = dayjs(item.startDt).format('YYYY-MM-DD');
      if (!formatted[date]) {
        formatted[date] = [item.startDt];
      } else {
        formatted[date].push(item.startDt);
      }
    });
    return formatted;
  };

  // 取得治療師的開放時間
  const fetchOpenTimes = async () => {
    const response = await axios.get('https://localhost:5000/TherapistOpenTime', {
      headers: {
        Authorization: token,
      }
    });
    if (response.data.isSuccess) {
      setOpenTimes(formatOpenTimes(response.data.data));
    } else {
      message.error(response.data.message);
    }
  };

  // 新增一個開放時間
  const addOpenTime = async (startDt) => {
    const formattedTime = dayjs(`${selectedDate}T${selectedTime}`).format('YYYY-MM-DD HH:mm:ss');
    console.log(formattedTime);
    const response = await axios.post(`https://localhost:5000/TherapistOpenTime?startDt=${encodeURIComponent(formattedTime)}`, {}, {
      headers: {
        Authorization: token,
      }
    });
    if (response.data.isSuccess) {
      message.success(response.data.message);
      fetchOpenTimes();
    } else {
      message.error(response.data.message);
    }
  };

  // 刪除一個開放時間
  const deleteOpenTime = async (startDt) => {
    const response = await axios.delete(`https://localhost:5000/TherapistOpenTime?startDt=${encodeURIComponent(startDt)}`, {
      headers: {
        Authorization: token,
      }
    });
    if (response.data.isSuccess) {
      message.success(response.data.message);
      fetchOpenTimes();
    } else {
      message.error(response.data.message);
    }
  };

  // 點選行事曆上的日期
  const handleDateSelect = (date) => {
    const formattedDate = date.format('YYYY-MM-DD');
    setSelectedDate(formattedDate);
    if (deleteMode) {
      deleteAllTimesInDate(formattedDate);
    } else {
      setOpen(true);
    }
  };

  // 選擇時間
  const handleTimeSelect = (time) => {
    setSelectedTime(time.format('HH:mm'));
  };

  // 點選對話框的確定按鈕
  const handleOk = () => {
    if (!deleteMode) {
      const formattedTime = dayjs(`${selectedDate}T${selectedTime}`).format('YYYY-MM-DD HH:mm:ss');
      if (openTimes[selectedDate] && openTimes[selectedDate].includes(formattedTime)) {
        deleteOpenTime(formattedTime);
      } else {
        addOpenTime(formattedTime);
      }
    } else {
      openTimes[selectedDate].forEach(time => deleteOpenTime(time));
    }
    setOpen(false);
  };

  const handleDeleteMode = () => {
    setDeleteMode(!deleteMode);
  };

  // 刪除指定日期的所有時間
  const deleteAllTimesInDate = async (date) => {
    const times = openTimes[date];
    if (times) {
      for (const time of times) {
        await deleteOpenTime(time);
      }
    }
  };

  // 點選對話框的取消按鈕
  const handleCancel = () => {
    setOpen(false);
  };

  useEffect(() => {
    fetchOpenTimes();
  }, []);

  return (
    <div>
      <LayoutMarTop />
    <div className="container py-md-5 my-md-3 TherapistOpenTime_sm">
            <Button
          onClick={handleDeleteMode}
          type={deleteMode ? 'danger' : 'default'}
          style={{ background: deleteMode ? 'red' : 'white' }}>
          {deleteMode ? '刪除模式' : '新增模式'}
        </Button>
      <Calendar
        cellRender={(value) => {
          const formattedDate = value.format('YYYY-MM-DD');
          if (openTimes[formattedDate]) {
            return (
              <ul>
                {openTimes[formattedDate].map((time, index) => (
                  <li key={index}>
                    {dayjs(time).format('HH:mm')}
                  </li>
                ))}
              </ul>
            );
          }
        }}
        onSelect={handleDateSelect}
        />
      <Modal open={open} onOk={handleOk} onCancel={handleCancel}>
        {!deleteMode && (
          <TimePicker
          format="HH:mm"
          minuteStep={10}
          hideDisabledOptions
          onChange={handleTimeSelect}
          />
          )}
      </Modal>
          </div>
    </div>
  );
};

export default TherapistOpenTime;

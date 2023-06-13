import React, { useState } from "react";
import axios from "axios";
import { notification, Button, DatePicker, TimePicker, Rate } from "antd";
import dayjs from "dayjs";
import { LayoutMarTop } from "../style";
import { Image } from "../style";
import photo_5 from "../../image/photo-6.jpg";
import "../../css/styleTwo.css";

const OrderFeedback = () => {
  const [customerId, setCustomerId] = useState("");
  const [customerPhone, setCustomerPhone] = useState("");
  const [startDt, setStartDt] = useState(null);
  const [rating, setRating] = useState(0);
  const [comments, setComments] = useState("");

  const handleFeedbackSubmit = async () => {
    try {
      const response = await axios.post("https://localhost:5000/Feedback", {
        customerId: customerId,
        customerPhone: customerPhone,
        startDt: startDt?.format(),
        rating: rating,
        comments: comments
      });

      if (response.data.isSuccess) {
        notification.success({
          message: response.data.message,
          duration: 3
        });

        setCustomerId("");
        setCustomerPhone("");
        setStartDt(null);
        setRating(0);
        setComments("");
      } else {
        notification.error({
          message: response.data.message,
          duration: 3
        });
      }
    } catch (error) {
      // console.error(error);
      notification.error({
        message: "評價提交失敗",
        duration: 3
      });
    }
  };

  const disabledMinutes = () => {
    const disabledMinutes = [];
    for (let minute = 0; minute < 60; minute++) {
      if (minute % 10 !== 0) {
        disabledMinutes.push(minute);
      }
    }
    return disabledMinutes;
  };

  // Function to determine which hours and minutes should be disabled
  const disabledTime = (_, type) => {
    if (type === "minute") {
      return disabledMinutes();
    }
    return [];
  };

  return (
    <div>
      <LayoutMarTop />
      <div className="container pt-5 my-md-5 vh-100">
        <div className="row d-flex justify-content-center  flex-wrap feedback_sm">
          <div className="col-md-6  col-tt-12 feedback_sm_style">
          <h2 className="mb-3">訂單評價</h2>
          <div className="mb-3" >
            <label className=" ">身份證號:</label>
            <input
            className="col-9"
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
            />
          </div>
          <div className="mb-3" >
            <label>手機號碼:</label>
            <input
                  className="col-9"
              value={customerPhone}
              onChange={(e) => setCustomerPhone(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label>訂單日期:</label>
            <DatePicker
                  className="col-9"
              value={startDt ? dayjs(startDt) : null}
              onChange={(date) => setStartDt(date ? date : null)}
            />
          </div>
          <div className="mb-3">
            <label>訂單時間:</label>
            <TimePicker
                  className="col-9"
              value={startDt ? dayjs(startDt) : null}
              onChange={(time) => setStartDt(time ? time : null)}
              minuteStep={10}
              secondStep={60}
              disabledTime={disabledTime}
            />
          </div>
          <div className="mb-3">
            <label>評分:</label>
            <Rate value={rating} onChange={(value) => setRating(value)} />
          </div>
          <div className="d-flex mb-3">
            <label className="">評價內容:</label>
            <textarea
                  className="col-9"
              value={comments}
              onChange={(e) => setComments(e.target.value)}
            />
          </div>
          <button className="btn color_2" type="primary" onClick={handleFeedbackSubmit}>
            提交評價
          </button>
        </div>
          <div className="col-md-6 col-sm-5  ">
            <Image url={photo_5 } height={400}/>
          </div>

          </div>
      </div>
    </div>
  );
};

export default OrderFeedback;

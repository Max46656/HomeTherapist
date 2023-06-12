import React, { useState, useEffect } from "react";
import axios from "axios";
import { notification, Button, Table, InputNumber } from "antd";
import AuthService from "../../../services/auth.service";
import { LayoutMarTop } from "../../style";
import { useNavigate } from "react-router-dom";

const FeedbackSummary = () => {
  const [averageRating, setAverageRating] = useState(null);
  const [feedbacks, setFeedbacks] = useState([]);
  const [loading, setLoading] = useState(false);
  const [minRating, setMinRating] = useState(0);
  const [maxRating, setMaxRating] = useState(5);

  const navigate = useNavigate();
  useEffect(() => {
    fetchFeedbackSummary();
  }, []);

  const fetchFeedbackSummary = async () => {
    try {
      setLoading(true);
      const user = AuthService.getCurrentUser();
      const token = user.token;

      const headers = {
        Authorization: token
      };

      const averageRatingResponse = await axios.get(
        "https://localhost:5000/Feedback/User/AverageRating",
        { headers }
      );
      const feedbacksResponse = await axios.get(
        `https://localhost:5000/Feedback/User/Ratings?minRating=${minRating}&maxRating=${maxRating}`,
        { headers }
      );

      // console.log(feedbacksResponse);
      if (averageRatingResponse.data.isSuccess) {
        setAverageRating(averageRatingResponse.data.data);
      } else {
        notification.error({
          message: averageRatingResponse.data.message,
          duration: 3
        });
      }

      if (feedbacksResponse.data.isSuccess) {
        setFeedbacks(feedbacksResponse.data.data);
      } else {
        notification.error({
          message: feedbacksResponse.data.message,
          duration: 3
        });
      }
    } catch (error) {
      console.error(error);
      notification.error({
        message: "Failed to fetch feedback summary",
        duration: 3
      });
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      title: "Order Date",
      dataIndex: "startDt",
      key: "orderDate",
      sorter: (a, b) => new Date(a.startDt) - new Date(b.startDt),
      render: (_, record) => record.feedback.order.startDt
    },
    {
      title: "Rating",
      dataIndex: "rating",
      key: "rating",
      sorter: (a, b) => a.rating - b.rating,
      render: (_, record) => record.feedback.rating
    },
    {
      title: "Comments",
      dataIndex: "comments",
      key: "comments",
      render: (_, record) => record.feedback.comments
    },
    {
      title: "Customer Phone",
      dataIndex: "customerPhone",
      key: "customerPhone",
      render: (_, record) => record.feedback.order.customerPhone
    }
  ];

  return (
    <div>
      <LayoutMarTop />
      <div className="container py-md-5 feedbackSU_style_sm">
        <h2 className="mb-md-2">我的訂單</h2>
        <div className="row">
          <div className="col-md-12">
            <h3 className="mb-md-2">平均評價: {averageRating}</h3>
            <div className="py-md-3 feedbackSU_sm_mb">
              評價最小值:{" "}
              <InputNumber
                value={minRating}
                min={0}
                max={5}
                onChange={(value) => setMinRating(value)}
              />
            </div>
            <div className="py-md-3 feedbackSU_sm_mb">
              評價最大值:{" "}
              <InputNumber
                value={maxRating}
                min={0}
                max={5}
                onChange={(value) => setMaxRating(value)}
              />
            </div>
            <button
              className="btn color_2 mb-md-5 feedbackSU_sm_mb "
              onClick={fetchFeedbackSummary}
            >
              尋找評價
            </button>
          </div>
          <Table
          className=""
    
            dataSource={feedbacks}
            columns={columns}
            loading={loading}
            rowKey={(record) => record.feedback.id}
            onChange={(pagination, filters, sorter) => console.log(sorter)}
          />
          <div className="row">

          <div className="col-lg-2 col-md-3  feedbackSU_sm_mb">
            <button
              className="btn color_2 "
              onClick={() => navigate("/OrderStats")}
            >
              訂單統計資訊
            </button>
          </div>
          <div className="col-md-3  ">
            <button
              className="btn color_2"
              onClick={() => navigate("/MyOrder")}
            >
              我的訂單
            </button>
          </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default FeedbackSummary;

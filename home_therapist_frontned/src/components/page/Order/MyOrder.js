import React, { useState, useEffect } from "react";
import axios from "axios";
import { Table, message, Button } from "antd";
import AuthService from "../../../services/auth.service";
import { useNavigate } from "react-router-dom";
import { LayoutMarTop } from "../../style";
import "../../../css/styleTwo.css";


const MyOrder = () => {
  const [orders, setOrders] = useState([]);
  const user = AuthService.getCurrentUser();
  const token = user.token;
  const navigate = useNavigate();

  useEffect(() => {
    const fetchOrders = async () => {
      const response = await axios.get("https://localhost:5000/Order", {
        headers: {
          Authorization: token
        }
      });
      console.log(response);
      if (response.data.isSuccess) {
        setOrders(
          response.data.data.map((item) => {
            return {
              ...item.order,
              service: item.order.orderDetails[0].service.name,
              note: item.order.orderDetails[0].note
            };
          })
        );
      } else {
        message.error(response.data.message);
      }
    };

    fetchOrders();
  }, [token]);

  const columns = [
    { title: "訂單時間", dataIndex: "startDt", key: "startDt" },
    { title: "服務項目", dataIndex: "service", key: "service" },
    { title: "顧客電話", dataIndex: "customerPhone", key: "customerPhone" },
    { title: "顧客性別", dataIndex: "gender", key: "gender" },
    { title: "顧客年齡層", dataIndex: "ageGroup", key: "ageGroup" },
    { title: "顧客自述", dataIndex: "note", key: "note" },
    {
      title: "顧客已匯款",
      dataIndex: "isComplete",
      key: "isComplete",
      render: (isComplete) => (isComplete ? "是" : "否")
    }
  ];

  return (
    <div>
      <LayoutMarTop />
      <div className="container pt-md-5 vh-100 ">
        <div className="row py-md-5 order_sm_style ">
          <div className="col-md-12 ">
            <Table
            className=" col-tt-12"
              dataSource={orders}
              columns={columns}
              pagination={{ pageSize: 12 }}
              rowKey="id"
            />
          </div>
          <div className="col-4 col-tt-12 ">
            <button
              className="btn color_2 mx-2 "
              onClick={() => navigate("/OrderStats")}
            >
              訂單統計資訊
            </button>
            <button
              className="btn color_2 mx-2"
              onClick={() => navigate("/MyFeedback")}
            >
              我的評價
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MyOrder;

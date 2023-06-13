import React, { useState, useEffect } from "react";
import axios from "axios";
import { LayoutMarTop } from "../style";
import "../../css/styleTwo.css";

const ClientOrder = () => {
  const [appointments, setAppointments] = useState([]);
  const [idNumber, setIdNumber] = useState("");
  const [phone, setPhone] = useState("");
  const [selectedAppointment, setSelectedAppointment] = useState(null);
  const [form, setForm] = useState({
    CustomerId: "",
    CustomerPhone: "",
    CustomerAddress: ""
  });

  useEffect(() => {
    fetchAppointments();
  }, [idNumber, phone]);

  const fetchAppointments = () => {
    axios
      .get(`https://localhost:5000/Appointment/${idNumber}?Phone=${phone}`)
      .then((res) => {
        const { data } = res;
        if (data.isSuccess) {
          setAppointments(data.data);
        } else {
          alert(data.message);
        }
      })
      .catch((err) => console.log(err));
  };

  const deleteAppointment = (appointment) => {
    axios
      .delete(
        `https://localhost:5000/Appointment/${idNumber}?Phone=${phone}&date=${appointment.startDt}`
      )
      .then((res) => {
        const { data } = res;
        alert(data.message);
        fetchAppointments();
      })
      .catch((err) => console.log(err));
  };

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const response = await axios.patch(
      `https://localhost:5000/Appointment/${idNumber}`,
      form
    );
    console.log(response.data.data);
    fetchAppointments();
  };

  return (
    <div>
      <LayoutMarTop />
      <div className="container pt-5 vh-100">
        <div className="row clientOrde_sm">
          <div className="clientOrde_flex">
            <div className="col-lg-5  mb-md-3 mx-2 ">
              <input
                className="p-1 clientOrde_style"
                type="text"
                value={idNumber}
                onChange={(e) => setIdNumber(e.target.value)}
                placeholder="身份證字號"
              />
            </div>
            <div className="col-lg-5  mb-md-3 mx-2 ">
              <input
                className="p-1 clientOrde_style"
                type="text"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                placeholder="手機號碼"
              />
            </div>
          </div>
          <div className="col-lg-2  col-sm-12 mb-md-3 ">
            <button
              className="btn color_2 ms-md-2  mb-5"
              onClick={fetchAppointments}
            >
              查詢預約
            </button>
          </div>
          {appointments.map((appointment, index) => (
            <div key={index}>
              <p>本次預約時間:{appointment.appointment.startDt}</p>
              <form className="row clientOrde_flex" onSubmit={handleSubmit}>
                <label className="col-12 my-2">
                  身份證字號:
                  <input
                    className="clientOrde_style2"
                    type="text"
                    name="CustomerId"
                    value={form.CustomerId}
                    onChange={handleChange}
                  />
                </label>
                <label className="col-12 my-2 ms-5">
                  手機:
                  <input
                    className="clientOrde_style2"
                    type="text"
                    name="CustomerPhone"
                    value={form.CustomerPhone}
                    onChange={handleChange}
                  />
                </label>
                <label className="col-12 my-2 ms-5">
                  地址:
                  <input
                    className="clientOrde_style2"
                    type="text"
                    name="CustomerAddress"
                    value={form.CustomerAddress}
                    onChange={handleChange}
                  />
                </label>
                <button
                  className="col-md-2 clientOrde_btn_mr btn color_2"
                  type="submit"
                >
                  更新預約
                </button>
                <button
                  className="col-md-2 clientOrde_btn_mr btn color_2"
                  onClick={() => deleteAppointment(appointment.appointment)}
                >
                  刪除預約
                </button>
              </form>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ClientOrder;

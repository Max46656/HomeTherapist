import React, { useState, useEffect } from "react";
import axios from "axios";
import Select from "react-select";
import { LayoutMarTop } from "../style";
import { message } from "antd";

const ClientOrder = () => {
  const [appointments, setAppointments] = useState([]);
  const [idNumber, setIdNumber] = useState("");
  const [phone, setPhone] = useState("");
  const [selectedAppointment, setSelectedAppointment] = useState(null);
  const [form, setForm] = useState({
    CustomerId: "",
    CustomerPhone: "",
    CustomerAddress: "",
    Gender: "",
    AgeGroup: "",
    Note: ""
  });
  const ageGroupOptions = [
    { value: "小於18", label: "小於18" },
    { value: "18到25", label: "18到25" },
    { value: "26到35", label: "26到35" },
    { value: "36到45", label: "36到45" },
    { value: "46到55", label: "46到55" },
    { value: "56到65", label: "56到65" },
    { value: "66到75", label: "66到75" },
    { value: "大於75", label: "大於75" }
  ];
  const genderOptions = [
    { value: "男", label: "男" },
    { value: "女", label: "女" },
    { value: "其他", label: "其他" }
  ];

  const fetchAppointments = () => {
    axios
      .get(`https://localhost:5000/Appointment/${idNumber}?Phone=${phone}`)
      .then((res) => {
        const { data } = res;
        if (data.isSuccess) {
          setAppointments(data.data);
        } else {
          message.error(data.message);
        }
        if (selectedAppointment !== null) {
          message.success("成功修改該預約");
        } else {
          message.success("成功獲得該預約");
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
        message.success("成功刪除該預約");
        fetchAppointments();
        console.log(appointments);
      })
      .catch((err) => console.log(err));
  };

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  };

  const handleSelectChange = (selectedOption, name) => {
    setForm({
      ...form,
      [name]: selectedOption ? selectedOption.value : ""
    });
  };

  const handleEdit = (appointment) => {
    setSelectedAppointment(appointment);
    setForm({
      CustomerId: appointment.customerId,
      CustomerPhone: appointment.customerPhone,
      CustomerAddress: appointment.customerAddress,
      Gender: appointment.gender,
      AgeGroup: appointment.ageGroup,
      Note: appointment.appointmentDetails[0]?.note
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const payload = [
      
      { op: "replace", path: "/CustomerId", value: form.CustomerId },
      { op: "replace", path: "/CustomerPhone", value: form.CustomerPhone },
      { op: "replace", path: "/CustomerAddress", value: form.CustomerAddress },
      { op: "replace", path: "/Gender", value: form.Gender },
      { op: "replace", path: "/AgeGroup", value: form.AgeGroup },
      { op: "replace", path: "/note", value: form.Note }
    ];
    console.log(form);
    await axios.patch(
      `https://localhost:5000/Appointment/${idNumber}?Phone=${phone}&date=${selectedAppointment.startDt}`,
      payload
    );
    fetchAppointments();
    setSelectedAppointment(null);
    setForm({
      CustomerId: "",
      CustomerPhone: "",
      CustomerAddress: "",
      Gender: "",
      AgeGroup: "",
      Note: ""
    });
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
              <p>本次預約時間: {appointment.startDt}</p>
              {selectedAppointment === appointment ? (
                <form  className="row clientOrde_flex"  onSubmit={handleSubmit}>
                  <table className="col-8 my-2 clientOrde_style2"  >
                    <div >
                      <div className="col-12  my-2 ms-2 clientOrde_from "style={{marginLeft:"20px"}}>
                        <label>身份證字號:</label>
                          <input
                          className="ms-2 col-8 clientOrde_from_input"
                            type="text"
                            name="CustomerId"
                            value={form.CustomerId}
                            onChange={handleChange}
                            placeholder={appointment.customerId}
                          />
                        
                      </div>
                      <div  className="col-12 my-2 ms-2 clientOrde_from">
                        <label className="ms-5 ">手機:</label>
                
                          <input
                            className="ms-2 col-8 clientOrde_from_input"
                            type="text"
                            name="CustomerPhone"
                            value={form.CustomerPhone}
                            onChange={handleChange}
                            placeholder={appointment.customerPhone}
                          />
                      
                      </div>
                      <div className="col-12 my-2 ms-2 clientOrde_from">
                        <label  className="ms-5">地址:</label>
                     
                          <input
                         className="ms-2 col-8 clientOrde_from_input"
                            type="text"
                            name="CustomerAddress"
                            value={form.CustomerAddress}
                            onChange={handleChange}
                            placeholder={appointment.customerAddress}
                          />
                      
                      </div>
                      <div  className="row my-2 ms-5 clientOrde_from ">
                        <label className="col-2   ">性別:     </label>


                          <Select 
                   className="col-3 me-2 sm_select"
                            name="Gender"
                            value={genderOptions.find(
                              (option) => option.value === form.Gender
                            )}
                            options={genderOptions}
                            onChange={(selectedOption) =>
                              handleSelectChange(selectedOption, "Gender")
                            }
                            placeholder={appointment.gender}
                          />

                   
                        
                      </div>
                      <div  className="row my-2 ms-5 clientOrde_from">
                        <label className="col-2 sm_age ">年齡區間:

                        </label>
                          <Select 
                          className="col-3 sm_select"
                            name="AgeGroup"
                            value={ageGroupOptions.find(
                              (option) => option.value === form.AgeGroup
                            )}
                            options={ageGroupOptions}
                            onChange={(selectedOption) =>
                              handleSelectChange(selectedOption, "AgeGroup")
                            }
                            placeholder={appointment.ageGroup}
                          />
               
                     
                      </div>
                      <div  className="col-12  my-2 ms-3 clientOrde_from" >
                        <label>需求備註:</label>
                        
                          <input
                          className="ms-2 col-6 clientOrde_from_input"
                            type="text"
                            name="Note"
                            value={form.Note}
                            onChange={handleChange}
                            placeholder={
                              appointment.appointmentDetails[0]?.note
                            }
                          />
                        
                      </div>
                    </div>
                  </table>
                  <div className="row">

                  <button
                  className="col-md-2 clientOrde_btn_mr btn color_2"
                   type="submit">更新預約</button>
                  <button
                  className="col-md-2 clientOrde_btn_mr btn color_2"
                   onClick={() => setSelectedAppointment(null)}>
                    取消
                  </button>
                  </div>
                </form>
              ) : (
                <div>
                  <table>
                    <tbody>
                      <tr>
                        <td>身份證字號:</td>
                        <td>{appointment.customerId}</td>
                      </tr>
                      <tr>
                        <td>手機:</td>
                        <td>{appointment.customerPhone}</td>
                      </tr>
                      <tr>
                        <td>地址:</td>
                        <td>{appointment.customerAddress}</td>
                      </tr>
                      <tr>
                        <td>性別:</td>
                        <td>{appointment.gender}</td>
                      </tr>
                      <tr>
                        <td>年齡區間:</td>
                        <td>{appointment.ageGroup}</td>
                      </tr>
                      <tr>
                        <td>需求備註:</td>
                        <td>{appointment.appointmentDetails[0].note}</td>
                      </tr>
                    </tbody>
                  </table>
                  <button
                   className="col-md-2 clientOrde_btn_mr btn color_2 me-3"
                   type="submit"
                   onClick={() => handleEdit(appointment)}>
                    修改預約
                  </button>
                  <button
                   className="col-md-2 clientOrde_btn_mr btn color_2"
                   type="submit"
                   onClick={() => deleteAppointment(appointment)}>
                    刪除預約
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ClientOrder;

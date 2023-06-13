import React , { useState } 
from "react";
import Map from "./Map";
import AuthService from "../services/auth.service";
import { Navigate, useNavigate } from "react-router-dom";

import {LayoutMarTop} from "../components/style";

const RegisterComponent = () => {
  const navigate = useNavigate();

  let [messages,setMessage]=useState([]);
  let[username,setUsername]=useState("");
  let[email,setEmil]=useState("");
  // let[addres,setAddres]=useState("");
  let[password,setPassword]=useState("");
  let[role,setRole]=useState("");

  const handleUsername=(e)=>{
    setUsername(e.target.value);
    
  }
  const handleEmail=(e)=>{
    setEmil(e.target.value);
}
const handlePassword=(e)=>{
  setPassword(e.target.value);
}
const handleconfirm=(e)=>{
  setRole(e.target.value);

}

const handleRegister=(e)=>{
  // AuthService.register(username,email,password)
  // .then(()=>{
 
  window.alert("註冊成功,進入登入頁面");
  navigate("/login");

// })
//   .catch((e)=>{setMessage(e.response.data)});
}
  return (
    <div style={{ padding: "3rem" }} className="col-md-12">
      <LayoutMarTop/>
      <div>
        {/* <div className="alert alert-danger">{messages}</div> */}
        <div>
          <label htmlFor="username">用戶名稱:</label>
          <input
             onChange={handleUsername}
            type="text"
            className="form-control"
            name="username"
          />
        </div>
        <br />
        <div className="form-group">
          <label htmlFor="email">電子信箱：</label>
          <input
             onChange={handleEmail}
            type="text"
            className="form-control"
            name="email"
          />
        </div>
        <br />
        <div className="form-group">
          <label htmlFor="password">密碼：</label>
          <input
            onChange={handlePassword}
            type="password"
            className="form-control"
            name="password"
            placeholder="長度至少超過6個英文或數字"
          />
        </div>
        <br />
        <div className="form-group">
          <label htmlFor="password">確認密碼</label>
          <input
             onChange={handleconfirm}
             type="password"
            className="form-control"
            placeholder="長度至少超過6個英文或數字"
            name="confirmPassword"
          />
        </div>
        <br />
        <button 
        onClick={handleRegister} 
        className="btn btn-primary">
          <span>註冊會員</span>
        </button>
      </div>
    
    </div>
  );
};

export default RegisterComponent;

import React, { useState } from "react";
import AuthService from "../services/auth.service";
import { useNavigate } from "react-router-dom";
import { LayoutMarTop } from "./style";


const LoginComponent = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleEmail = (e) => {
    setEmail(e.target.value);
  };

  const handlePassword = (e) => {
    setPassword(e.target.value);
  };

const handleLogin = async () => {
  try {
    const response = await AuthService.login(email, password);
    const token = response.data.data.token;
    const user = { token: token };
    // console.log("handleLogin user.token", user.token);
    localStorage.setItem("user", JSON.stringify(user));

    // window.alert("登入成功，導入個人頁面");
    navigate("/my-appointment");
    window.location.reload();
  } catch (error) {
    console.log(error);
  }
};

  return (
    <div style={{ padding: "3rem" }} >
      <div>
        <LayoutMarTop />
        <div className="container ">
        <div className="row">

        <div className="form-group">
          <label htmlFor="username">電子信箱：</label>
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
          />
        </div>
        <br />
        <div className="form-group">
          <button
            onClick={handleLogin}
            className="btn btn-primary btn-block"
          >
            <span>登入系統</span>
          </button>
        </div>
      </div>
        </div>

        </div>

    </div>
  );
};

export default LoginComponent;

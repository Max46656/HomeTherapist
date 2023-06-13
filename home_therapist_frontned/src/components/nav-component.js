import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "../css/styleTwo.css";
import AuthService from "../services/auth.service";
import styled from "styled-components";


const Nav = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    checkLoginStatus();
  }, []);

  const checkLoginStatus = () => {
    const user = AuthService.getCurrentUser();
    if (user) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  };

  const handleLogout = () => {
    AuthService.logout();
    setIsLoggedIn(false);
  };

  return (
    <div>
      <div className="container-fluid ">
        <nav className="color_1 navbar navbar-expand-lg fixed-top py-3 Navsize">
          <div className="navbar-brand ">
            <h1 className="ddd">Home Therapist</h1>
          </div>
          <button
            className="navbar-toggler "
            data-bs-toggle="collapse"
            data-bs-target=".menu1"
          >
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse menu1 navbar-collapse justify-content-end">
            <ul className="navbar-nav text-center ">
              <li className=" nav-item title_Btn">
                <Link className="nav-link" to="/">
                  首頁
                </Link>
              </li>
              <li className=" nav-item title_Btn">
                <Link className="nav-link" to="/about">
                  關於我
                </Link>
              </li>
              <li className=" nav-item title_Btn">
                <Link className="nav-link" to="/Articles">
                  文章
                </Link>
              </li>
              {!isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/city-selection">
                    新增預約
                  </Link>
                </li>
              )}
              <li className="title_Btn nav-item ">
                <Link className="nav-link" to="/order">
                  查詢預約
                </Link>{" "}
              </li>
               {!isLoggedIn && (
              <li className="title_Btn nav-item ">
                <Link className="nav-link" to="/client/feedback">
                  評價回饋
                </Link>{" "}
              </li>
                )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/OrderStats">
                    訂單統計
                  </Link>{" "}
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/profile">
                    個人頁面
                  </Link>{" "}
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/my-appointment">
                    我的預約
                  </Link>{" "}
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/therapist-open-time">
                    可預約時間管理
                  </Link>
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/therapist-open-services">
                    可預約服務管理
                  </Link>
                </li>
              )}
              {!isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/login">
                    治療師登入
                  </Link>
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" to="/server/:article">
                    發布文章
                  </Link>
                </li>
              )}
              {isLoggedIn && (
                <li className="title_Btn nav-item ">
                  <Link className="nav-link" onClick={handleLogout} to="/">
                    登出
                  </Link>
                </li>
              )}
            </ul>
          </div>
        </nav>
      </div>
    </div>
  );
};

export default Nav;

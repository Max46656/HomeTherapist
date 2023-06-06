import React from "react";
import { Carousel } from "antd";
import { Image,LayoutMarTop } from "./style";
import banner1 from "../../src/image/photo_1.jpg";
import banner2 from "../image/photo_2.jpg";
import banner3 from "../image/photo_3.jpg";
import "../css/style.css";
import { Link } from "react-router-dom/dist";


const HomeComponent = () => {
  return (
    <main className="overflow-hidden">
      <LayoutMarTop/>
      <div className="row position-relative " >
        <Carousel className="container-fluid  p-0 " autoplay>
          <Image url={banner1} />
          <Image url={banner2}/>
          <Image url={banner3}/>
        </Carousel>
            <div className=" p-5 col-md-5 offset-2 position-absolute mt-md-5">
              <h2 className="tx_shadow mb-md-5">作為一個物理治療師</h2>
              <p  className="tx_shadow mb-md-5">
                您可以通過註冊成為一名講師，並開始製作在線課程。本網站僅供練習之用，請勿提供任何個人資料，例如信用卡號碼。
              </p>
              <button className="btn btn-common p-2 mt-2 " type="button">
                今天開始開設課程
              </button>
            </div>
        </div>
        <LayoutMarTop/>
      <div className="container py-4">
      
        <div className="row align-items-md-stretch">
          <div className="col-md-6">  
            <Link to="/client/:appointment"className="btn btn-secondary" >
            <div className="h-100 p-5 text-white  rounded-3">
              <button className="btn btn-outline-light" type="button">
              <h2>馬上使用預約服務</h2>
              </button>
            </div>
            </Link>
          </div>
          <div className="col-md-6">
          </div>
        </div>

        
      </div>
      <LayoutMarTop/>
    </main>
  );
};

export default HomeComponent;

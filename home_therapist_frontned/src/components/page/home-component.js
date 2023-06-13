import React from "react";
import { Carousel } from "antd";
import { Image, LayoutMarTop } from "../style";
import banner1 from "../../image/photo_1.jpg";
import banner2 from "../../image/photo_2.jpg";
import banner3 from "../../image/photo_3.jpg";
import "animate.css";
import { Link } from "react-router-dom/dist";
import "../../css/styleTwo.css";

const HomeComponent = () => {
  return (
    <main className="overflow-hidden">
      <LayoutMarTop />
      <div className="row position-relative ">
        <Carousel className="container-fluid  p-0  mb-lg-5 mb-sm-5 " autoplay>
          <Image url={banner1} height={600} />
          <Image url={banner2} height={600} />
          <Image url={banner3} height={600} />
        </Carousel>

        <div className="title_one  p-5 col-md-5 offset-2  mt-md-5 pt-sm-5 PositionMe ">
          <h2 className="  mb-md-5   ">作為一個物理治療師</h2>
          <p className="  mb-md-5">
         
          對於有到府居家治療有高度熱忱的物理治療師，您可以通過註冊成加入我們的行列，並開始運用你的專業來治療病人。
          </p>
          <Link to="/city-selection">
            <button
              className="puls-e btn btn-common py-2 px-5 mt-2 "
              type="button"
            >
              馬上預約
            </button>
          </Link>
        </div>
      </div>
      <LayoutMarTop />
      <div className="container py-4 mb-lg-5">
        <div className="row align-items-md-stretch">
          <div className="mx-auto col-md-10 home_sm_px ">
            <div className="row">
              <p className="col-lg-12 col-md-12 col-sm-10  serve_item">疼痛管理</p>

              <p className="col-lg-12 col-md-12 col-sm-10  serve_content">
                提供專業的疼痛管理服務，幫助患者緩解和控制疼痛感。
              </p>
            </div>
            <div className="row mb-lg-3">
              <p className="col-lg-12 col-md-12 col-sm-10  serve_item">日常活動訓練</p>
              <p className="col-lg-12 col-md-12 col-sm-10  serve_content">
                透過日常活動的指導和訓練，幫助患者恢復或改善日常生活中的功能和活動能力。
              </p>
            </div>
            <div className="row mb-lg-3">
              <p className="col-lg-12 col-md-12 col-sm-10  serve_item">環境最佳化。</p>
              <p className="col-lg-12 col-md-12 col-sm-10  serve_content">
                評估和優化患者所處環境，包括家庭和社區，以提供最適合患者需求的支持和便利
              </p>
            </div>
            <div className="row mb-lg-3">
              <p className="col-lg-12 col-md-12 col-sm-10  serve_item">環境安全與適應性評估</p>
              <p className="col-lg-12 col-md-12 col-sm-10  serve_content">
                評估患者所處環境的安全性和適應性，提供必要的建議和改善措施，確保患者的居住和生活環境符合其需求和安全要求。
              </p>
            </div>
            <div className="row mb-lg-3">
              <p className="col-lg-12 col-md-12 col-sm-10  serve_item">家庭照顧者訓練</p>
              <p className="col-lg-12 col-md-12 col-sm-10  serve_content ">
                為家庭照顧者提供必要的培訓和指導，幫助他們更好地照顧和支持患者，同時提供必要的知識和技能。
              </p>
            </div>
          </div>
          <div className="col-md-6"></div>
        </div>
      </div>
      <LayoutMarTop />
    </main>
  );
};

export default HomeComponent;

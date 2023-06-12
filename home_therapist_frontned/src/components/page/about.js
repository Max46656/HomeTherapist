import React from "react";
import { LayoutMarTop, Image } from "../style";
import banner2 from "../../image/photo_3.jpg";
import photo5 from "../../image/photo-5.jpg";
import photo7 from "../../image/photo-7.jpg";
import photo4 from "../../image/photo-4.jpg";
import photo6 from "../../image/photo-6.jpg";



const About = () => {
  return (
    <main className="overflow-hidden" >
      
      <LayoutMarTop />

      <div className=" mb-5">
        <Image url={banner2} height={400}>
          <div className="h-100 d-flex justify-content-center border align-items-center">
            <h1 className="about_title">關於我</h1>
          </div>
        </Image>
      </div>
      <LayoutMarTop />
      <div className=" row d-flex justify-content-center mb-5  ">
        <div className="col-lg-4 col-md-12 col-sm-12  order-lg-1 order-sm-2  abouy_sm_p ">
          <Image url={photo5} height={400} />
        </div>
        <div className="col-lg-6 col-md-10 col-sm-10 order-lg-2  order-sm-1 abount_content  pt-3 pb-md-3 mb-md-5 ">
          <p>
            秉持著專業治療，快樂助人的初衷，集結了對於到府居家治療有高度熱忱的物理治療師。
            <br />
            透過密集的內部教育訓練和案例交流分享，長期持續的培養治療師的各項專長及技能，讓專精的物理治療師為您解決累積已久的困擾與疼痛，或是因為運動及其他原因造成的立即不舒服，可以全方位為您解決與分擔，透過物理治療運動訓練，藉以維持肌力、增進活力。
            在治療病人的經驗中，常遇到一些病人因為無法找到適合的醫療而使自己與家人忍受痛苦。
            而這些問題是可以經由物理治療而改善、甚至消除的。
            這些族群以肌肉與肌筋膜轉移疼痛族群為大宗。病人常遇到的問題是，經過X光檢查骨頭結構排列無虞但是就是疼痛!!或是有看到骨頭的些微病徵，但又不至於造成如此長期的疼痛!!此時，物理治療師的角色就出來了，透過專業的徒手AET(Accurate
            Evaluation
            Technique)檢查，往往能揪出機器無法找到的肌肉與肌筋膜問題，並加以解決。
            這項技術在我們所內為物理治療師必修，並且應用在每一位病人身上。而治療的後期，我們著重在動作訓練
          </p>
        </div>
      </div>
      <LayoutMarTop />
      <div className="row d-flex justify-content-center about_introduce py-5  ">
        <h2 className="text-center mb-3 about_title">我們的願景</h2>
        <div className="col-lg-8  col-md-10 col-sm-3">
          <p className="abount_content">
          主要治療方式為西式徒手治療，配合運動治療及姿勢矯正，達到最完善效果，希望藉由客製化的動作來維持每個人的身體健康良好。而且通過到府治療能明確評估出患者生活上需要改善的地方，能有益病情上的恢復。
          此外，腦中風病人、巴金森氏症病人等。我們利用神經物理治療中的肌力肌耐力訓練、平衡訓練、動作控制等技巧，來達到恢復自我能力、維持身體機能、減少照顧者負擔的目的。
         我們希望能拋開傳統醫療院所的既有模式，建立新一代更有效的治療方式，來面對身體各式的病痛。

          </p>
        </div>
      </div>
      <LayoutMarTop />
      <div className="row d-flex justify-content-center py-5 mb-5">
        <div className="col-3">
<Image  url={photo6}  height={350}/>
        </div>
        <div className="col-3">
        <Image url={photo4}  height={350}/>
        </div>
        <div className="col-3">
        <Image url={photo7}  height={350}/>
        </div>
      </div>
      <LayoutMarTop />
    
    </main>
  );
};

export default About;

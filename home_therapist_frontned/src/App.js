import React from 'react';
import { BrowserRouter, Route, Routes ,useParams } from "react-router-dom";
import Layout from"./components/Layout";
import HomeComponent from "./components/page/home-component";
import RegisterComponent from "./components/register-component";
import LoginComponent from "./components/page/login-component"
import ClientOrder from "./components/Client/ClientOrder"
import OrderFeedback  from "./components/Client/OrderFeedback"
import AppointmentForm from './components/page/UserAppointments';
import UserArticles from "./components/Article-component";
import ArticleForm from "./components/ArticleForm";
import ArticleShow from "./components/ArticleShow";

import ScrollToToP from "./components/scrollTop";
import Profile from './components/page/profile';
import OrderStats  from './components/page/Order/OrderStats';
import MyOrder  from './components/page/Order/MyOrder';
import FeedbackSummary  from './components/page/Order/FeedbackSummary';
import TherapistOpenTime from './components/page/TherapistOpenTime';
import TherapistServices from './components/page/TherapistServices';
import CitySelection from './components/AvailableAppointments/CitySelection';
import AvailableDates from './components/AvailableAppointments/AvailableDates';
import ConfirmAppointment from './components/AvailableAppointments/ConfirmAppointment';
import About from './components/page/about';

function App() {

  return (
    <BrowserRouter>
    <ScrollToToP />
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomeComponent />} />
          {/* 取消註冊，商業理由 */}
          <Route path="register" element={<RegisterComponent />} />
          <Route path="login" element={<LoginComponent />} />
          {/* 以下待排版 */}
          <Route path="about" element={<About/>} />
          <Route path="city-selection" element={<CitySelection />} />
          <Route path="/order" element={<ClientOrder />} />
          <Route path="client/feedback" element={<OrderFeedback />} />
          <Route path="/OrderStats" element={<OrderStats />} />
          <Route path="/MyOrder" element={<MyOrder />} />
          <Route path="profile" element={<Profile/>} />
          <Route path="MyFeedback" element={<FeedbackSummary />} />
          <Route path="my-appointment" element={<AppointmentForm />} />
          <Route path="therapist-open-time" element={<TherapistOpenTime />} />
          <Route path="therapist-open-services" element={<TherapistServices />} />
          <Route path="server/:article" element={<UserArticles/>} />
          <Route path="/Articles" element={<ArticleForm/>} />
          <Route path="Article/:id" element={<ArticleShow />} />
          {/* <Route path="client/appointment" element={<AppointmentForm />} /> */}
          <Route path="available-dates" element={<AvailableDates />} />
          <Route path="confirm-appointment" element={<ConfirmAppointment />} />





        </Route>
      </Routes>
    </BrowserRouter>


  );
}


export default App;



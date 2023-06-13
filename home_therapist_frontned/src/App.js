import React from 'react';
import { BrowserRouter, Route, Routes ,useParams } from "react-router-dom";
import Layout from"./components/Layout";
import HomeComponent from "./components/home-component";
import RegisterComponent from "./components/register-component";
import LoginComponent from "./components/login-component"
import ItemComponent from "./components/item"
import ClientOrder from "./components/ClientOrder"
import AppointmentForm from './components/UserAppointments';
import ArticleForm from "./components/Article-component";
import ProfileAppoint from "./components/profile-Appoint";
import ScrollToToP from "./components/scrollTop";
import Profile from './components/profile';
import OrderStats  from './components/Order/OrderStats';
import MyOrder  from './components/Order/MyOrder';
import TherapistOpenTime from './components/TherapistOpenTime';
import TherapistServices from './components/TherapistServices';
import CitySelection from './components/AvailableAppointments/CitySelection';
import AvailableDates from './components/AvailableAppointments/AvailableDates';
import ConfirmAppointment from './components/AvailableAppointments/ConfirmAppointment';
// import PPd from './components/pppd';

function App() {

  return (
    <BrowserRouter>
    <ScrollToToP />
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomeComponent />} />
          <Route path="register" element={<RegisterComponent />} />
          <Route path="login" element={<LoginComponent />} />
          <Route path="/order" element={<ClientOrder />} />
          <Route path="client/appointment" element={<AppointmentForm />} />
          <Route path="my-appointment" element={<AppointmentForm />} />
          <Route path="items/:ttt" element={< ItemComponent/>} />
          <Route path="server/:article" element={<ArticleForm/>} />
          <Route path="profile" element={<Profile/>} />
          <Route path="city-selection" element={<CitySelection />} />
          <Route path="available-dates" element={<AvailableDates />} />
          <Route path="confirm-appointment" element={<ConfirmAppointment />} />
          <Route path="therapist-open-time" element={<TherapistOpenTime />} />
          <Route path="therapist-open-services" element={<TherapistServices />} />
          <Route path="/OrderStats" element={<OrderStats />} />
          <Route path="/MyOrder" element={<MyOrder />} />

          />

          <Route path="profile/:appoint" element={<ProfileAppoint/>} /> {/* 物理預約*/}
          {/* <Route path="ppd" element={<PPd/>} />  */}
          {/* 迦泓物理預約*/}

        </Route>
      </Routes>
    </BrowserRouter>


  );
}


export default App;



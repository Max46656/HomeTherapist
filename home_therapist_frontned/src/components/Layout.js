import { Outlet } from "react-router-dom";
import Nav from "./nav-component";
import Footer from "./footer";

const Layout = () => {
  return (
    <div>
      <Nav />
      <Outlet />
      <Footer/>
    </div>
  );
};

export default Layout;

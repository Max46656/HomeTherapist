import React, { useState, useEffect } from "react";
import { LayoutMarTop } from "../style";
import AuthService from "../../services/auth.service";
import { message } from "antd";

const Profile = () => {
  const [userInfo, setUserInfo] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [newUserInfo, setNewUserInfo] = useState({});
  const [profileImage, setProfileImage] = useState({});
  const fileInput = React.createRef();

  useEffect(() => {
    fetchUserInfo();
    fetchProfileImage();
  }, []);

  const fieldMapping = {
    certificateNumber: "證書號碼",
    address: "地址",
    latitude: "緯度",
    longitude: "經度",
    radius: "半徑（km）",
    userName: "使用者名稱",
    email: "電子郵件",
    phoneNumber: "電話號碼"
  };

  const fetchProfileImage = async () => {
    try {
      const user = AuthService.getCurrentUser();
      const token = user.token;
      const response = await AuthService.getProfileImage(token);
      if (response.status === 200) {
        const photoUrl = `https://localhost:5000${response.data}`;
        setProfileImage(photoUrl);
      } else {
        setProfileImage(null);
      }
    } catch (error) {
      message.error("你還沒有上傳形象照");
      setProfileImage(null);
    }
  };

  const fetchUserInfo = async () => {
    try {
      const user = AuthService.getCurrentUser();
      const token = user.token;
      const response = await AuthService.getUserInfo(token);
      setUserInfo(response.data.data);
    } catch (error) {
      console.error("Error fetching user info:", error);
    }
  };

  const handleUploadClick = () => {
    fileInput.current.click();
  };

  const handleImageChange = async (event) => {
    try {
      const user = AuthService.getCurrentUser();
      const token = user.token;
      await AuthService.updateProfileImage(token, event.target.files[0]);
      fetchProfileImage();
    } catch (error) {
      console.error("Error updating profile image:", error);
    }
  };

  const handleEdit = () => {
    setIsEditing(true);
    setNewUserInfo(userInfo);
  };

  const handleChange = (field, value) => {
    setNewUserInfo({ ...newUserInfo, [field]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const user = AuthService.getCurrentUser();
      const token = user.token;
      const patchData = Object.keys(newUserInfo)
        .filter((key) => Object.keys(fieldMapping).includes(key))
        .map((key) => ({
          op: "replace",
          path: `/${key}`,
          value: newUserInfo[key]
        }));
      await AuthService.updateUserInfo(token, patchData);
      fetchUserInfo();
      setIsEditing(false);
    } catch (error) {
      console.error("Error updating user info:", error);
    }
  };

  return (
    <div>
      <LayoutMarTop />
      <div className="container py-md-5 profile_sm_style">
        <div className="row">
          <h2 className="pt-md-5">治療師資訊</h2>
          {userInfo && (
            <div className="">
              <p className="mb-md-3">形象照：</p>
              {profileImage ? (
                <img className="col-tt-8 profile_sm_style_flex" src={profileImage} alt="Profile" />
              ) : (
                <p></p>
              )}
              <input
                style={{ display: "none" }}
                type="file"
                ref={fileInput}
                onChange={handleImageChange}
              />
              <button
                className="ms-md-3 btn color_2 col-tt-5 profile_sm_style_flex"
                onClick={handleUploadClick}
              >
                上傳新的形象照
              </button>
              <p className="pt-md-5">其他資訊：</p>
              {isEditing ? (
                <form onSubmit={handleSubmit}>
                  <ul>
                    {Object.keys(fieldMapping).map((key) => (
                      <li style={{ margin: "20px" }} className="" key={key}>
                        {fieldMapping[key]}：
                        {key === "certificateNumber" ? (
                          <span>{userInfo[key]}</span>
                        ) : (
                          <input
                            type="text"
                            placeholder={userInfo[key]}
                            value={newUserInfo[key]}
                            onChange={(e) => handleChange(key, e.target.value)}
                          />
                        )}
                      </li>
                    ))}
                  </ul>
                  <div className="col-tt-9 ">

                  <button className="btn color_2 col-tt-4 ggccccc    " type="submit">
                    保存
                  </button>
                  <button
                    className="btn color_2 mx-md-3 col-tt-4 "
                    onClick={() => setIsEditing(false)}
                  >
                    取消
                  </button>
                  </div>
                </form>
              ) : (
                <div>
                  <ul>
                    {Object.keys(fieldMapping).map((key) => (
                      <li key={key}>
                        {fieldMapping[key]}：{userInfo[key]}
                      </li>
                    ))}
                  </ul>
                  <button className="btn color_2 my-md-5" onClick={handleEdit}>
                    更新
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Profile;

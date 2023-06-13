import React, { useState, useEffect } from "react";
import { LayoutMarTop } from "./style";
import AuthService from "../services/auth.service";

const Profile = () => {
  const [userInfo, setUserInfo] = useState(null);
  const [profileImage, setProfileImage] = useState(null);
  const [isEditing, setIsEditing] = useState({
    certificateNumber: false,
    address: false,
    latitude: false,
    longitude: false,
    radius: false,
    userName: false,
    email: false,
    phoneNumber: false,
  });
  const [newUserInfo, setNewUserInfo] = useState({});

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
  phoneNumber: "電話號碼",
};

const fetchUserInfo = async () => {
  try {
    const user = AuthService.getCurrentUser();
    const token = user.token;
    // console.log("fetchUserInfo token", token);
    // console.log("fetchUserInfo user", user);
    const response = await AuthService.getUserInfo(token);
    // console.log("fetchUserInfo response", response.data);
    // 將獲得的使用者資訊設定到狀態中
    setUserInfo(response.data.data);
  } catch (error) {
    console.error("Error fetching user info:", error);
  }
};

const fetchProfileImage = async () => {
  try {
    const user = AuthService.getCurrentUser();
    const token = user.token;
    const response = await AuthService.getProfileImage(token);
    // console.log(token);
    if (response.status !== 200)
      throw new Error("Unable to fetch profile image.");

    const photoUrl = `https://localhost:5000${response.data}`;
    // console.log(photoUrl);
    setProfileImage(photoUrl);
    // return photoUrl;
  } catch (error) {
    console.error("Error fetching profile image:", error);
    throw error;
  }
};

  const handleEdit = (field) => {
    setIsEditing({ ...isEditing, [field]: true });
    setNewUserInfo({ ...newUserInfo, [field]: userInfo[field] });
  };

  const handleChange = (field, value) => {
    setNewUserInfo({ ...newUserInfo, [field]: value });
  };

const handleSaveChanges = async (field) => {
  try {
    const user = AuthService.getCurrentUser();
    const token = user.token;
    const patchData = [
      {
        op: "replace",
        path: `/${field}`,
        value: newUserInfo[field],
      },
    ];
    // console.log(patchData);
    const response = await AuthService.updateUserInfo(token, patchData);
    // console.log(response);
    fetchUserInfo();
    setIsEditing({ ...isEditing, [field]: false });
  } catch (error) {
    console.error("Error updating user info:", error);
  }
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

  return (
    <div>
      <LayoutMarTop  className="mb-5"/>
      <div className="container mb-5">

      <h2>治療師資訊</h2>
      {userInfo && (
        <div>
          <p >形象照：</p>
          {profileImage && <img src={profileImage} alt="Profile" />}
          {isEditing.profileImage ? (
            <input type="file" onChange={handleImageChange} />
          ) : (
            <button className="btn-common ms-3" onClick={() => handleEdit("profileImage")}>上傳新的形象照</button>
          )}

          <p className="ml-3">其他資訊：</p>
          <ul>
            {Object.keys(userInfo).map((key) =>
              key in isEditing ? (
                <li key={key}>
                   {fieldMapping[key]}：{isEditing[key] ? (
                    <div>
                      <input
                        type="text"
                        placeholder={userInfo[key]}
                        value={newUserInfo[key]}
                        onChange={(e) => handleChange(key, e.target.value)}
                      />
                      <button className="btn-common ms-3" onClick={() => handleSaveChanges(key)}>保存</button>
                    </div>
                  ) : (
                    <div>
                      {userInfo[key]}
                      <button className="btn-common ms-3" onClick={() => handleEdit(key)}>更新</button>
                    </div>
                  )}
                </li>
              ) : null
            )}
          </ul>
        </div>
      )}
      </div>
    </div>
  );
};


export default Profile;

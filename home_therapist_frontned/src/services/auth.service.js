import axios from "axios";

const API_URL = "https://localhost:5000";

class AuthService {
  login(email, password) {
    return axios.post(API_URL + "/api/Auth/login", { email, password });
  }

  logout() {
    localStorage.removeItem("user");
  }

  register(username, email, password) {
    return axios.post(API_URL + "/api/Auth/register", {
      userName: username,
      email,
      password,
    });
  }

getUserInfo(token) {
  return axios.get(API_URL+"/User", {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
}

getProfileImage(token) {
    return axios.get(API_URL+"/Photo/ProfilePhotoUrl", {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
}

 getCurrentUser() {
  const user = localStorage.getItem("user");
  // console.log("Stored user data:", user);
  if (user) {
    try {
      const userData = JSON.parse(user);
      // console.log("Parsed user data:", userData);
      return userData;
    } catch (error) {
      console.error("Error parsing user data:", error);
      return null;
    }
  }
  return null;
}

updateUserInfo(token, patchData) {
  const patchArray = Array.isArray(patchData) ? patchData : [patchData];
  // console.log("updateUserInfo patchData", patchArray);
  // console.log("updateUserInfo token", token);

  return axios.patch(API_URL+"/User", patchArray, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json-patch+json",
    },
  });
}

  updateProfileImage(token, imageFile) {
    const formData = new FormData();
    formData.append('image', imageFile);

    const requestOptions = {
      method: 'POST',
      headers: { 'Authorization': `Bearer ${token}` },
      body: formData
    };

    return fetch(API_URL+'/Photo/UploadProfileImage', requestOptions).then(this.handleResponse);
  }

 getAppointmentsByUser(token) {
    return axios.get(API_URL + "/User/GetAppointmentsByUser", {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
  }

}

export default new AuthService();


// 這個頁面是Chart Gpt 寫的
import React,{useState} from "react";

const Map = () => {
  const [address, setAddress] = useState("");
  const [latitude, setLatitude] = useState("");
  const [longitude, setLongitude] = useState("");

  const handleGeocode = async () => {
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(
          address
        )}`
      );
      const data = await response.json();

      if (data && data.length > 0) {
        const { lat, lon } = data[0];
        setLatitude(lat);
        setLongitude(lon);
      } else {
        setLatitude("");
        setLongitude("");
      }
    } catch (error) {
      console.error("Geocoding error:", error);
    }
  };
  return (
    <div>
      <input
        type="text"
        placeholder="Enter an address"
        value={address}
        name="address"
        onChange={(e) => setAddress(e.target.value)}
      />
      <button onClick={handleGeocode}>Geocode</button>

      {latitude && longitude && (
        <div>
          Latitude: {latitude}
          <br />
          Longitude: {longitude}
        </div>
      )}
    </div>
  );
};

export default Map;

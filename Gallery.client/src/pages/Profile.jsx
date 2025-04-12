import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";

const Profile = () => {
  const { userId } = useParams();
  const [profile, setProfile] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await axios.get(
          `https://localhost:32805/Profile/${userId}`
        );
        console.log("API Response:", response.data); // Логируем данные
        setProfile(response.data);
      } catch (err) {
        console.error("API Error:", err); // Логируем ошибку
        if (err.response) {
          setError(`Error: ${err.response.data}`);
        } else {
          setError("An error occurred while fetching the profile.");
        }
      }
    };

    if (userId) {
      fetchProfile();
    }
  }, [userId]);

  if (error) {
    return <div>{error}</div>;
  }

  if (!profile) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>Profile</h1>
      <p>
        <strong>Username:</strong> {profile.username}
      </p>
      <p>
        <strong>Avatar:</strong>
      </p>
      <img
        src={`https://localhost:32805${profile.avatarFilePath}`}
        alt="User Avatar"
        style={{ width: "150px", height: "150px" }}
      />
      <p>
        <strong>Created At:</strong>{" "}
        {new Date(profile.createdAt).toLocaleString()}
      </p>
      <p>
        <strong>Last Login:</strong>{" "}
        {new Date(profile.lastLogin).toLocaleString()}
      </p>
    </div>
  );
};

export default Profile;

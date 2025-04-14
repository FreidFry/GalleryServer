import React, { useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const InitAuth = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const authenticate = async () => {
      try {
        const response = await axios.get(
          "https://localhost:32778/api/auth/init",
          { withCredentials: true }
        );

        if (response.status.ok) {
          navigate("/Gallery");
        } else {
          navigate("/Login");
        }
      } catch (error) {
        navigate("/Login");
      }
    };

    authenticate();
  }, [navigate]);

  return <div>Initializing Authentication...</div>;
};

export default InitAuth;

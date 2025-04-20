import React, { useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

export default function InitAuth({ setIsLogin, setUserId }) {
  const navigate = useNavigate();

  useEffect(() => {
    const authenticate = async () => {
      try {
        const response = await axios.get(
          "https://localhost:32778/api/auth/init",
          { withCredentials: true }
        );

        if (response.status === 200) {
          setIsLogin(true);
          setUserId(response.data.userId);
          navigate(`/Gallery/${response.data.userId}`);
        } else {
          setIsLogin(false);
          navigate("/Login");
        }
      } catch (error) {
        setIsLogin(false);
        navigate("/Login");
      }
    };

    authenticate();
  }, [navigate, setIsLogin]);

  return <div>Initializing Authentication...</div>;
}

import React, { useState } from "react";
import axios from "axios";
import "../css/AuthForm.css";
import { useNavigate } from "react-router-dom";

export default function AuthForm() {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    username: "",
    password: "",
    confirmPassword: "",
  });
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isLogin && formData.password !== formData.confirmPassword) {
      alert("Passwords do not match!");
      return;
    }

    try {
      const url = isLogin
        ? "https://localhost:32778/api/auth/login"
        : "https://localhost:32778/api/auth/register";

      const payload = isLogin
        ? { username: formData.username, password: formData.password }
        : {
            username: formData.username,
            password: formData.password,
            confirmPassword: formData.confirmPassword,
          };

      const response = isLogin
        ? await axios.post(url, payload, { withCredentials: true })
        : await axios.post(url, payload, { withCredentials: true });

      if (response.status === 200) {
        navigate("/Gallery");
      }
      alert(`Successfully ${isLogin ? "logged in" : "registered"}!`);
    } catch (error) {
      console.error(
        `Error during ${isLogin ? "login" : "registration"}:`,
        error
      );
      alert(`An error occurred during ${isLogin ? "login" : "registration"}.`);
    }
  };

  const toggleForm = () => {
    setIsLogin(!isLogin);
    setFormData({ username: "", password: "", confirmPassword: "" });
  };

  return (
    <div className="auth-form">
      <div className="auth-container">
        <h1>{isLogin ? "Welcome Back" : "Create an Account"}</h1>
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            name="username"
            placeholder="username"
            value={formData.username}
            onChange={handleChange}
            required
          />
          <input
            type="password"
            name="password"
            placeholder="Password"
            value={formData.password}
            onChange={handleChange}
            required
          />
          {!isLogin && (
            <input
              type="password"
              name="confirmPassword"
              placeholder="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
            />
          )}
          <button type="submit" className={isLogin ? "login" : "register"}>
            {isLogin ? "Login" : "Register"}
          </button>
        </form>
        <p>
          {isLogin ? (
            <>
              Don't have an account?{" "}
              <button onClick={toggleForm}>Register here</button>
            </>
          ) : (
            <>
              Already have an account?{" "}
              <button onClick={toggleForm}>Login here</button>
            </>
          )}
        </p>
      </div>
    </div>
  );
}

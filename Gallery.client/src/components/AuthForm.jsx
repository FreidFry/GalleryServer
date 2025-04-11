import React, { useState } from "react";
import axios from "axios";

const AuthForm = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    username: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isLogin && formData.password !== formData.confirmPassword) {
      alert("Passwords do not match!");
      return;
    }

    try {
      const url = isLogin
        ? "https://localhost:8080/api/login"
        : "https://localhost:8080/api/register";

      const payload = isLogin
        ? { username: formData.username, password: formData.password }
        : {
            username: formData.username,
            password: formData.password,
            confirmPassword: formData.confirmPassword,
          };

      const response = await axios.post(url, payload);
      console.log(
        `${isLogin ? "Login" : "Registration"} successful:`,
        response.data
      );
      alert(`${isLogin ? "Login" : "Registration"} successful!`);
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
      <h2>{isLogin ? "Login" : "Register"}</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            name="username"
            value={formData.username}
            onChange={handleChange}
            required
          />
        </div>
        <div>
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        {!isLogin && (
          <div>
            <label htmlFor="confirmPassword">Confirm Password:</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
            />
          </div>
        )}
        <button type="submit">{isLogin ? "Login" : "Register"}</button>
      </form>
      <button onClick={toggleForm}>
        {isLogin ? "Switch to Register" : "Switch to Login"}
      </button>
    </div>
  );
};

export default AuthForm;

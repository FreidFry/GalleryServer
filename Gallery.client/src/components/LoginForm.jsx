import React, { useState } from "react";
import axios from "axios";

const LoginForm = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({ username: "", password: "" });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const url = isLogin
        ? "https://localhost:32777/api/auth/login"
        : "https://localhost:32777/api/register";

      const payload = isLogin
        ? { username: formData.username, password: formData.password }
        : { username: formData.username, password: formData.password };

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
    setFormData({ username: "", password: "" }); // Reset form data
  };

  return (
    <div className="login-form">
      <h2>{isLogin ? "Login" : "Register"}</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
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
            onChange={handleInputChange}
            required
          />
        </div>
        <button type="submit">{isLogin ? "Login" : "Register"}</button>
      </form>
      <button onClick={toggleForm}>
        {isLogin ? "Switch to Register" : "Switch to Login"}
      </button>
    </div>
  );
};

export default LoginForm;

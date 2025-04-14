import React from "react";
import { useState } from "react";
import { Route, Routes, BrowserRouter } from "react-router-dom";
import Gallery from "./pages/Gallery";
import Auth from "./pages/Auth";
import Profile from "./pages/Profile";
import Header from "./components/Header";
import InitAuth from "./components/InitAuth";
import "./css/App.css";

export default function App() {
  const [isLogin, setIsLogin] = useState(false);

  return (
    <div className="App">
      <BrowserRouter>
        <Header isLogin={isLogin} />
        <Routes>
          <Route path="/" element={<InitAuth setIsLogin={setIsLogin} />} />
          <Route path="/Login" element={<Auth />} />
          <Route path="Gallery" element={<Gallery />} />
          <Route path="Profile/:userId" element={<Profile />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

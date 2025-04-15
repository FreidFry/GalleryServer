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
  const [userId, setUserId] = useState(null);

  return (
    <div className="App">
      <BrowserRouter>
        <Header isLogin={isLogin} />
        <Routes>
          <Route
            path="/"
            element={<InitAuth setIsLogin={setIsLogin} setUserId={setUserId} />}
          />
          <Route path="/Login" element={<Auth />} />
          <Route
            path="/Gallery/:userId"
            element={<Gallery userId={userId} />}
          />
          <Route path="/Profile/:userId" element={<Profile />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

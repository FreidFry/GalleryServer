import React from "react";
import { Route, Routes, BrowserRouter } from "react-router-dom";
import Gallery from "./pages/Gallery";
import Auth from "./pages/Auth";
import Profile from "./pages/Profile";
import Header from "./components/Header";
import InitAuth from "./components/InitAuth";

export default function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Header />
        <Routes>
          <Route path="/" element={<InitAuth />} />
          <Route path="/Login" element={<Auth />} />
          <Route path="Gallery" element={<Gallery />} />
          <Route path="Profile/:userId" element={<Profile />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

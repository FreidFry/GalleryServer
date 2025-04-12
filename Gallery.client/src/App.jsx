import React from "react";
import { Route, Routes, BrowserRouter } from "react-router-dom";
import Gallery from "./pages/Gallery";
import Auth from "./pages/Auth";
import Profile from "./pages/Profile";
import Header from "./components/Header";

export default function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Header />
        <Routes>
          <Route path="/" element={<Auth />} />
          <Route path="Gallery" element={<Gallery />} />
          <Route path="Profile/:userId" element={<Profile />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

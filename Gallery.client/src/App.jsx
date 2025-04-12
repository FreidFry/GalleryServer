import React from "react";
import { Route, Routes, BrowserRouter } from "react-router-dom";
import Gallery from "./pages/Gallery";
import Auth from "./pages/Auth";

export default function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <>
            <Route path="/" element={<Auth />} />
            <Route path="Gallery" element={<Gallery />} />
          </>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

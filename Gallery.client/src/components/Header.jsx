import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../App.css";

// filepath: d:\Code\GalleryServer\Gallery.client\src\components\Header.jsx

const Header = ({ avatarUrl, userId }) => {
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();

  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  const handleHomeClick = () => {
    navigate("/");
  };

  return (
    <header className="header">
      <button className="home-button" onClick={handleHomeClick}>
        Home
      </button>
      <div className="avatar-container" onClick={toggleMenu}>
        <img
          src={avatarUrl || "https://via.placeholder.com/50"}
          alt="User Avatar"
          className="avatar"
        />
        {menuOpen && (
          <div className="dropdown-menu">
            <Link to={`/Profile/${userId}`}>Profile</Link>{" "}
            {/* Динамическая ссылка */}
            <Link to="/Gallery">Gallery</Link>
            <button
              onClick={() => alert("Logout functionality not implemented yet!")}
            >
              Logout
            </button>
          </div>
        )}
      </div>
    </header>
  );
};

export default Header;

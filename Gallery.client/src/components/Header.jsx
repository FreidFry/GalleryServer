import React, { useState, useEffect, useRef } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../css/Header.css";

export default function Header({ avatarUrl, userId, isLogin }) {
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef(null);
  const avatarRef = useRef(null);
  const navigate = useNavigate();

  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  const handleHomeClick = () => {
    navigate("/");
  };

  const handleClickOutside = (event) => {
    if (
      menuOpen &&
      menuRef.current &&
      !menuRef.current.contains(event.target) &&
      avatarRef.current &&
      !avatarRef.current.contains(event.target)
    ) {
      setMenuOpen(false);
    }
  };

  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [menuOpen]);

  return (
    <header>
      <nav>
        <ul>
          <li>
            <a href="#home" onClick={handleHomeClick}>
              Home
            </a>
          </li>
          <li>
            <a href="#about">Gallery</a>
          </li>
        </ul>
        {isLogin && (
          <div
            className="avatar-container"
            ref={avatarRef}
            onClick={toggleMenu}
          >
            <img
              src={
                avatarUrl ||
                "https://localhost:32778/default/img/defaultUserAvatar.png"
              }
              alt="User Avatar"
              className="avatar"
            />
            {menuOpen && (
              <div className="dropdown-menu" ref={menuRef}>
                <Link to={`/Profile/${userId}`}>Profile</Link>
                <Link to="/Gallery">Gallery</Link>
                <button
                  onClick={() =>
                    alert("Logout functionality not implemented yet!")
                  }
                >
                  Logout
                </button>
              </div>
            )}
          </div>
        )}
      </nav>
    </header>
  );
}

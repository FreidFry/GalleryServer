import React, { useState, useEffect, useRef } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../css/Header.css";
import axios from "axios";

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

  const handleGalleryClick = () => {
    navigate(`/Gallery`);
  };

  const handleLogoutClick = () => {
    axios
      .post(
        "https://localhost:32778/api/auth/logout",
        {},
        {
          withCredentials: true,
        }
      )
      .then(() => {
        navigate("/login");
      })
      .catch((error) => {
        console.error("Logout error:", error);
        alert("Logout failed! Please try again.");
      });
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
            <a href="" onClick={handleHomeClick}>
              Home
            </a>
          </li>
          <li>
            <a href="" onClick={handleGalleryClick}>
              Gallery
            </a>
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
                <Link to={`/Profile`}>Profile</Link>
                <Link to="/Gallery">Gallery</Link>
                <button onClick={() => handleLogoutClick()}>Logout</button>
              </div>
            )}
          </div>
        )}
      </nav>
    </header>
  );
}

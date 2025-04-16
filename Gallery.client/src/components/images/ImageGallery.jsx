import React, { useEffect, useState } from "react";
import axios from "axios";
import "../../css/ImageGallery.css";

export default function ImageGallery({ userId }) {
  const [images, setImages] = useState([]);

  useEffect(() => {
    if (!userId) return;

    axios
      .get(`https://localhost:32778/image/getall/${userId}`, {
        withCredentials: true,
      })
      .then((res) => setImages(res.data))
      .catch((err) => console.error("Ошибка загрузки:", err));
  }, [userId]);

  return (
    <div className="gallery-grid">
      {images.map((img) => (
        <div key={img.imageId} className="gallery-item">
          <img src={`https://localhost:32778/${img.imageUrl}`} alt={img.name} />
          <h3>{img.name}</h3>
          <p>{img.description}</p>
        </div>
      ))}
    </div>
  );
}

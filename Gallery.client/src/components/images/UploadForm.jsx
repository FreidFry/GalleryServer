import React, { useState } from "react";
import axios from "axios";
import "../../css/UploadForm.css";

export default function UploadForm() {
  const [image, setImage] = useState(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [publicity, setPublicity] = useState(false);
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!image) return setMessage("Выберите изображение");

    const formData = new FormData();
    formData.append("image", image);
    formData.append("name", name);
    formData.append("description", description);
    formData.append("publicity", String(publicity));

    try {
      await axios.post("https://localhost:32778/image/upload", formData, {
        withCredentials: true,
        headers: { "Content-Type": "multipart/form-data" },
      });
      alert("Успешно загружено");
    } catch (err) {
      alert("Ошибка: " + JSON.stringify(err.response?.data || err.message));
    }
  };

  return (
    <div>
      <form className="upload-form" onSubmit={handleSubmit}>
        <h2>Загрузка изображения</h2>
        <input
          type="file"
          onChange={(e) => setImage(e.target.files?.[0] || null)}
        />
        <input
          type="text"
          placeholder="Название"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <input
          type="text"
          placeholder="Описание"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
        />
        <label>
          <input
            type="checkbox"
            checked={publicity}
            onChange={(e) => setPublicity(e.target.checked)}
          />
          <p>Сделать приватным</p>
        </label>
        <button type="submit">Загрузить</button>
        {message && <p className="message">{message}</p>}
      </form>
    </div>
  );
}

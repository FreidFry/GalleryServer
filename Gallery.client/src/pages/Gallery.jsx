import React from "react";
import UploadForm from "../components/images/UploadForm";
import ImageGallery from "../components/images/ImageGallery";

export default function Gallery({ userId }) {
  return (
    <div>
      <h1>Галерея</h1>
      <UploadForm />
      <ImageGallery userId={userId} />
    </div>
  );
}

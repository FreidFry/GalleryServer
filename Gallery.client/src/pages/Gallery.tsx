import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

interface Image {
  imageId: string;
  name: string;
  description?: string;
  imageUrl: string;
  createAt: string;
}

const API_BASE_URL = 'https://localhost:32778';

const Gallery: React.FC = () => {
  const { userId } = useParams<{ userId: string }>();
  const { user: currentUser } = useAuth();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [sortBy, setSortBy] = useState<string>('date');
  const [orderBy, setOrderBy] = useState<string>('desc');

  // If no userId is provided, redirect to current user's gallery
  React.useEffect(() => {
    if (!userId && currentUser?.id) {
      navigate(`/gallery/${currentUser.id}`);
    }
  }, [userId, currentUser, navigate]);

  const { data: images, isLoading } = useQuery<Image[]>({
    queryKey: ['images', userId, sortBy, orderBy],
    queryFn: async () => {
      if (!userId) return [];
      const response = await axios.get(
        `${API_BASE_URL}/image/getall/${userId}?SortBy=${sortBy}&OrderBy=${orderBy}`,
        { withCredentials: true }
      );
      return response.data;
    },
    enabled: !!userId
  });

  const uploadMutation = useMutation({
    mutationFn: async (file: File) => {
      const formData = new FormData();
      formData.append('image', file);
      formData.append('name', file.name);
      const response = await axios.post(`${API_BASE_URL}/image/upload`, formData, {
        withCredentials: true,
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images', userId] });
      setSelectedFile(null);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (imageIds: string[]) => {
      await axios.delete(`${API_BASE_URL}/image/remove`, {
        data: imageIds,
        withCredentials: true,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['images', userId] });
    },
  });

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedFile(event.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (selectedFile) {
      await uploadMutation.mutateAsync(selectedFile);
    }
  };

  const handleDelete = async (imageId: string) => {
    if (window.confirm('Are you sure you want to delete this image?')) {
      await deleteMutation.mutateAsync([imageId]);
    }
  };

  if (isLoading) {
    return <div className="text-center">Loading...</div>;
  }

  const isOwner = currentUser?.id === userId;

  return (
    <div className="max-w-7xl mx-auto p-4">
      <div className="mb-6 flex justify-between items-center">
        <div className="flex space-x-4">
          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
            className="rounded-md border-gray-300"
          >
            <option value="date">Date</option>
            <option value="title">Title</option>
          </select>
          <select
            value={orderBy}
            onChange={(e) => setOrderBy(e.target.value)}
            className="rounded-md border-gray-300"
          >
            <option value="desc">Descending</option>
            <option value="asc">Ascending</option>
          </select>
        </div>

        {isOwner && (
          <div className="flex items-center space-x-4">
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="hidden"
              id="image-upload"
            />
            <label
              htmlFor="image-upload"
              className="cursor-pointer bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600"
            >
              Select Image
            </label>
            {selectedFile && (
              <button
                onClick={handleUpload}
                className="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600"
              >
                Upload
              </button>
            )}
          </div>
        )}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {images?.map((image) => (
          <div key={image.imageId} className="relative group">
            <img
              src={`${API_BASE_URL}${image.imageUrl}`}
              alt={image.name}
              className="w-full h-48 object-cover rounded-lg"
            />
            <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-50 transition-opacity duration-200 rounded-lg">
              <div className="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-200">
                {isOwner && (
                  <button
                    onClick={() => handleDelete(image.imageId)}
                    className="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600"
                  >
                    Delete
                  </button>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Gallery; 
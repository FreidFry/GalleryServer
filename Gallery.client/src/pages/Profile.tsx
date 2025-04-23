import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

interface ProfileData {
  id: string;
  username: string;
  email: string;
  avatarFilePath: string;
}

const Profile: React.FC = () => {
  const { userId } = useParams<{ userId: string }>();
  const { user: currentUser } = useAuth();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const { data: profile, isLoading } = useQuery<ProfileData>({
    queryKey: ['profile', userId],
    queryFn: async () => {
      const response = await axios.get(`https://localhost:32778/profile/${userId}`, {
        withCredentials: true
      });
      return response.data;
    }
  });

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedFile(event.target.files[0]);
    }
  };

  const handleAvatarUpdate = async () => {
    if (!selectedFile) return;

    const formData = new FormData();
    formData.append('avatar', selectedFile);

    try {
      await axios.put('https://localhost:32778/profile/updateavatar', formData, {
        withCredentials: true,
        headers: {
          'Content-Type': 'multipart/form-data'
        },
      });
      // Refetch profile data
      window.location.reload();
    } catch (error) {
      console.error('Failed to update avatar:', error);
    }
  };

  const getAvatarUrl = (avatarFilePath: string) => {
    // If the path starts with 'default', it's a default avatar
    if (avatarFilePath.startsWith('default')) {
      return `https://localhost:32778/${avatarFilePath}`;
    }
    // For custom avatars, the path should be relative to /images
    return `https://localhost:32778/${avatarFilePath}`;
  };

  if (isLoading) {
    return <div className="text-center">Loading...</div>;
  }

  if (!profile) {
    return <div className="text-center">Profile not found</div>;
  }

  return (
    <div className="max-w-2xl mx-auto p-4">
      <div className="bg-white shadow rounded-lg p-6">
        <div className="flex items-center space-x-4">
          <div className="relative">
            <img
              src={getAvatarUrl(profile.avatarFilePath)}
              alt={profile.username}
              className="w-24 h-24 rounded-full object-cover"
              onError={(e) => {
                const target = e.target as HTMLImageElement;
                target.src = '/default-avatar.png';
              }}
            />
            {currentUser?.id === userId && (
              <div className="mt-2">
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleFileChange}
                  className="hidden"
                  id="avatar-upload"
                />
                <label
                  htmlFor="avatar-upload"
                  className="cursor-pointer bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600"
                >
                  Change Avatar
                </label>
                {selectedFile && (
                  <button
                    onClick={handleAvatarUpdate}
                    className="mt-2 bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600"
                  >
                    Upload
                  </button>
                )}
              </div>
            )}
          </div>
          <div>
            <h2 className="text-2xl font-bold">{profile.username}</h2>
            <p className="text-gray-600">{profile.email}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile; 
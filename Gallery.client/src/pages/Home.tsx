import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';

interface User {
  id: string;
  username: string;
  avatarFilePath: string;
  createdAt: string;
  lastLogin: string;
}

const Home: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const navigate = useNavigate();
  const { user } = useAuth();

  const { data: users, isLoading, error } = useQuery<User[]>({
    queryKey: ['users', searchQuery],
    queryFn: async () => {
      if (!searchQuery) return [];
      const response = await axios.get(`https://localhost:32778/profile/search?SearchString=${searchQuery}`, {
        withCredentials: true
      });
      console.log('Users data:', response.data); // Debug log
      return response.data;
    },
    enabled: searchQuery.length > 0
  });

  const handleUserClick = (userId: string) => {
    navigate(`/gallery/${userId}`);
  };

  const handleMyGalleryClick = () => {
    if (user?.id) {
      navigate(`/gallery/${user.id}`);
    }
  };

  const handleProfileClick = () => {
    if (user?.id) {
      navigate(`/profile/${user.id}`);
    }
  };

  const getAvatarUrl = (avatarFilePath: string) => {
    // If the path starts with 'default', it's a default avatar
    if (avatarFilePath.startsWith('default')) {
      return `https://localhost:32778${avatarFilePath}`;
    }
    // For custom avatars, the path should be relative to /images
    return `https://localhost:32778${avatarFilePath}`;
  };

  return (
    <div className="container mx-auto px-4 py-8">
      {user && (
        <div className="mb-8 flex justify-center space-x-4">
          <button
            onClick={handleMyGalleryClick}
            className="px-6 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
          >
            My Gallery
          </button>
          <button
            onClick={handleProfileClick}
            className="px-6 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-colors"
          >
            My Profile
          </button>
        </div>
      )}

      <div className="max-w-xl mx-auto">
        <input
          type="text"
          placeholder="Search users..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        
        {isLoading && (
          <div className="mt-4 text-center">
            <p>Loading...</p>
          </div>
        )}

        {error && (
          <div className="mt-4 text-center text-red-500">
            <p>Error loading users. Please try again.</p>
          </div>
        )}

        {users && users.length > 0 && (
          <div className="mt-6 grid gap-4">
            {users.map((user) => {
              const avatarUrl = getAvatarUrl(user.avatarFilePath);
              console.log('Avatar URL for', user.username, ':', avatarUrl); // Debug log
              return (
                <div
                  key={user.id}
                  onClick={() => handleUserClick(user.id)}
                  className="flex items-center space-x-4 p-4 bg-white rounded-lg shadow hover:shadow-md cursor-pointer transition-shadow"
                >
                  <img
                    src={avatarUrl}
                    alt={user.username}
                    className="w-12 h-12 rounded-full object-cover"
                    onError={(e) => {
                      console.log('Image load error for', user.username); // Debug log
                      const target = e.target as HTMLImageElement;
                      target.src = 'https://localhost:32778/default/img/defaultUserAvatar.png';
                    }}
                  />
                  <div>
                    <h3 className="font-medium text-gray-900">{user.username}</h3>
                    <p className="text-sm text-gray-500">
                      Joined: {new Date(user.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {users && users.length === 0 && searchQuery && (
          <div className="mt-4 text-center text-gray-500">
            <p>No users found</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Home; 
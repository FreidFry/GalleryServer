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

interface Image {
  imageId: string;
  name: string;
  description: string;
  imageUrl: string;
  createAt: string;
}

const Home: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const navigate = useNavigate();
  const { user } = useAuth();

  const { data: users, isLoading: isLoadingUsers, error: usersError } = useQuery<User[]>({
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

  const { data: randomImages, isLoading: isLoadingImages, error: imagesError } = useQuery<Image[]>({
    queryKey: ['randomImages'],
    queryFn: async () => {
      const response = await axios.get('https://localhost:32778/image/random', {
        withCredentials: true
      });
      return response.data;
    },
    staleTime: 5 * 60 * 1000 // Cache for 5 minutes
  });

  const handleUserClick = (userId: string) => {
    navigate(`/gallery/${userId}`);
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
      <div className="max-w-xl mx-auto mb-12">
        <input
          type="text"
          placeholder="Search users..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        
        {isLoadingUsers && (
          <div className="mt-4 text-center">
            <p>Loading users...</p>
          </div>
        )}

        {usersError && (
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

      <div className="mb-12">
        <h2 className="text-2xl font-bold mb-6 text-center">Featured Images</h2>
        
        {isLoadingImages && (
          <div className="text-center">
            <p>Loading featured images...</p>
          </div>
        )}

        {imagesError && (
          <div className="text-center text-red-500">
            <p>Error loading featured images. Please try again.</p>
          </div>
        )}

        {randomImages && randomImages.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            {randomImages.map((image) => (
              <div
                key={image.imageId}
                className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow"
              >
                <div className="aspect-w-16 aspect-h-9">
                  <img
                    src={`https://localhost:32778${image.imageUrl}`}
                    alt={image.name || 'Gallery image'}
                    className="w-full h-full object-cover"
                    onError={(e) => {
                      const target = e.target as HTMLImageElement;
                      target.src = 'https://localhost:32778/default/img/imageNotFound.png';
                    }}
                  />
                </div>
                <div className="p-4">
                  {image.name && (
                    <h3 className="font-medium text-gray-900 mb-1">{image.name}</h3>
                  )}
                  {image.description && (
                    <p className="text-sm text-gray-500">{image.description}</p>
                  )}
                  <p className="text-xs text-gray-400 mt-2">
                    {new Date(image.createAt).toLocaleDateString()}
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}

        {randomImages && randomImages.length === 0 && (
          <div className="text-center text-gray-500">
            <p>No featured images available</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Home; 
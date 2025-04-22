import React, { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

interface User {
  id: string;
  username: string;
}

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, password: string, confirmPassword: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    // Check if user is logged in on mount
    const checkAuth = async () => {
      try {
        const response = await axios.get('https://localhost:32778/api/auth/init', {
          withCredentials: true
        });
        if (response.data.userId) {
          setIsAuthenticated(true);
          setUser({ id: response.data.userId, username: '' });
        }
      } catch (error) {
        console.error('Auth check failed:', error);
        setIsAuthenticated(false);
        setUser(null);
      }
    };
    checkAuth();
  }, []);

  const login = async (username: string, password: string) => {
    try {
      const response = await axios.post('https://localhost:32778/api/auth/login', 
        { username, password },
        { withCredentials: true }
      );
      
      // Get user ID after successful login
      const initResponse = await axios.get('https://localhost:32778/api/auth/init', {
        withCredentials: true
      });
      
      if (initResponse.data.userId) {
        setIsAuthenticated(true);
        setUser({ id: initResponse.data.userId, username });
      }
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const register = async (username: string, password: string, confirmPassword: string) => {
    try {
      const response = await axios.post('https://localhost:32778/api/auth/register',
        { username, password, confirmPassword },
        { withCredentials: true }
      );
      
      // Get user ID after successful registration
      const initResponse = await axios.get('https://localhost:32778/api/auth/init', {
        withCredentials: true
      });
      
      if (initResponse.data.userId) {
        setIsAuthenticated(true);
        setUser({ id: initResponse.data.userId, username });
      }
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await axios.post('https://localhost:32778/api/auth/logout', {}, { withCredentials: true });
      setIsAuthenticated(false);
      setUser(null);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}; 
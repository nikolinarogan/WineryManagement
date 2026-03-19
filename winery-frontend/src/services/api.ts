import axios from 'axios'
import router from '@/router'

const TOKEN_KEY = 'winery_token'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5115/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem(TOKEN_KEY)
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

api.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem('winery_user')
      router.push('/login')
    }
    
    if (error.response?.status === 403) {
      console.error('Nemate dozvolu za ovu akciju')
    }
    
    return Promise.reject(error)
  }
)

export default api


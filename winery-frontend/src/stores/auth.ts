import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService from '@/services/authService'
import type { LoginRequest, RegisterRequest, AuthResponse, ChangePasswordRequest } from '@/types/auth'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<AuthResponse | null>(authService.getUser())
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!user.value && authService.isLoggedIn())
  const currentUser = computed(() => user.value)
  const isManager = computed(() => user.value?.kategorija === 'Menadzer')
  const userRole = computed(() => user.value?.kategorija || null)
  const userId = computed(() => user.value?.idzap || null)

  // Actions
  async function login(credentials: LoginRequest) {
    try {
      loading.value = true
      error.value = null
      const response = await authService.login(credentials)
      user.value = response
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Greška pri prijavljivanju'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function register(data: RegisterRequest) {
    try {
      loading.value = true
      error.value = null
      const response = await authService.register(data)
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Greška pri registraciji'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function changePassword(data: ChangePasswordRequest) {
    try {
      loading.value = true
      error.value = null
      await authService.changePassword(data)
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Greška pri promjeni lozinke'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCurrentUser() {
    try {
      const userData = await authService.getCurrentUser()
      user.value = { ...user.value, ...userData } as AuthResponse
    } catch (err) {
      console.error('Greška pri učitavanju korisnika:', err)
    }
  }

  function logout() {
    authService.logout()
    user.value = null
    error.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    user,
    loading,
    error,
    // Getters
    isAuthenticated,
    currentUser,
    isManager,
    userRole,
    userId,
    // Actions
    login,
    register,
    changePassword,
    fetchCurrentUser,
    logout,
    clearError
  }
})


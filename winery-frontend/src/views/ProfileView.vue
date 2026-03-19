<template>
  <div class="profile">
    <div class="profile-container">
      <h1>Moj profil</h1>
      
      <div v-if="loading" class="loading">Učitavanje...</div>
      
      <div v-else-if="profile" class="profile-content">
        <!-- Osnovni podaci -->
        <div class="profile-section">
          <h2>Osnovni podaci</h2>
          <div class="info-grid">
            <div class="info-item">
              <label>Ime i Prezime</label>
              <p>{{ profile.ime }} {{ profile.prez }}</p>
            </div>
            <div class="info-item">
              <label>Email</label>
              <p>{{ profile.email }}</p>
            </div>
            <div class="info-item">
              <label>JMBG</label>
              <p>{{ profile.jmbg }}</p>
            </div>
          </div>
        </div>

        <!-- Dodatni podaci zavisno od kategorije -->
        <div v-if="hasAdditionalData" class="profile-section highlighted">
          <h2>Dodatne informacije</h2>
          <div class="info-grid">
            <!-- Enolog -->
            <div v-if="profile.kategorija === 'Enolog' && profile.brsert" class="info-item">
              <label>Broj Sertifikata</label>
              <p>{{ profile.brsert }}</p>
            </div>
            <!-- Menadžer -->
            <div v-if="profile.kategorija === 'Menadzer' && profile.bonusucinak != null" class="info-item">
              <label>Bonus Učinak</label>
              <p>{{ profile.bonusucinak }}</p>
            </div>
            <!-- Radnik -->
            <div v-if="profile.kategorija === 'Radnik' && profile.fizickaspremnost" class="info-item">
              <label>Fizička Spremnost</label>
              <p>{{ profile.fizickaspremnost }}</p>
            </div>
            <!-- Somleijer -->
            <div v-if="profile.kategorija === 'Somleijer' && profile.specijalnost" class="info-item">
              <label>Specijalnost</label>
              <p>{{ profile.specijalnost }}</p>
            </div>
          </div>
        </div>

        <!-- Promjena lozinke -->
        <div class="profile-section">
          <div class="section-header">
            <h2>Promjena lozinke</h2>
            <button @click="togglePasswordForm" class="btn-toggle">
              {{ showPasswordForm ? 'Odustani' : 'Promijeni lozinku' }}
            </button>
          </div>

          <div v-if="showPasswordForm" class="password-form-container">
            <form @submit.prevent="handleChangePassword" class="password-form">
              <div class="form-group">
                <label for="old-password">Stara lozinka *</label>
                <input
                  id="old-password"
                  v-model="passwordForm.staraLozinka"
                  type="password"
                  required
                />
              </div>
              <div class="form-group">
                <label for="new-password">Nova lozinka *</label>
                <input
                  id="new-password"
                  v-model="passwordForm.novaLozinka"
                  type="password"
                  required
                  minlength="6"
                />
              </div>
              <div class="form-group">
                <label for="confirm-password">Potvrda nove lozinke *</label>
                <input
                  id="confirm-password"
                  v-model="passwordForm.potvrdaLozinke"
                  type="password"
                  required
                  minlength="6"
                />
              </div>
              
              <button type="submit" class="btn-primary" :disabled="changingPassword">
                {{ changingPassword ? 'Mijenjam...' : 'Promijeni lozinku' }}
              </button>

              <div v-if="passwordError" class="error-message">
                {{ passwordError }}
              </div>
              <div v-if="passwordSuccess" class="success-message">
                Lozinka uspješno promijenjena!
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import authService from '@/services/authService'
import type { Profile, ChangePasswordRequest } from '@/types/auth'

const profile = ref<Profile | null>(null)
const loading = ref(true)
const changingPassword = ref(false)
const passwordError = ref('')
const passwordSuccess = ref(false)
const showPasswordForm = ref(false)

const passwordForm = ref<ChangePasswordRequest>({
  staraLozinka: '',
  novaLozinka: '',
  potvrdaLozinke: ''
})

const hasAdditionalData = computed(() => {
  if (!profile.value) return false
  
  switch (profile.value.kategorija) {
    case 'Enolog':
      return !!profile.value.brsert
    case 'Menadzer':
      return profile.value.bonusucinak != null
    case 'Radnik':
      return !!profile.value.fizickaspremnost
    case 'Somleijer':
      return !!profile.value.specijalnost
    default:
      return false
  }
})

const togglePasswordForm = () => {
  showPasswordForm.value = !showPasswordForm.value
  if (!showPasswordForm.value) {
    // Reset form when closing
    passwordForm.value = {
      staraLozinka: '',
      novaLozinka: '',
      potvrdaLozinke: ''
    }
    passwordError.value = ''
    passwordSuccess.value = false
  }
}

const loadProfile = async () => {
  try {
    loading.value = true
    profile.value = await authService.getProfile()
  } catch (error) {
    console.error('Error loading profile:', error)
  } finally {
    loading.value = false
  }
}

const handleChangePassword = async () => {
  try {
    changingPassword.value = true
    passwordError.value = ''
    passwordSuccess.value = false

    await authService.changePassword(passwordForm.value)
    
    passwordSuccess.value = true
    // Reset form
    passwordForm.value = {
      staraLozinka: '',
      novaLozinka: '',
      potvrdaLozinke: ''
    }
    
    // Close form after 2 seconds
    setTimeout(() => {
      showPasswordForm.value = false
      passwordSuccess.value = false
    }, 2000)
  } catch (error: any) {
    passwordError.value = error.response?.data?.message || 'Greška pri promjeni lozinke'
  } finally {
    changingPassword.value = false
  }
}

onMounted(() => {
  loadProfile()
})
</script>

<style scoped>
.profile {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.profile-container {
  max-width: 900px;
  margin: 0 auto;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 40px 0;
}

.loading {
  text-align: center;
  padding: 60px;
  font-size: 18px;
  color: #666;
}

.profile-content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.profile-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.profile-section.highlighted {
  border-left: 4px solid #000;
  background: #f9f9f9;
}

.profile-section h2 {
  font-size: 24px;
  font-weight: 600;
  color: #000;
  margin: 0 0 24px 0;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0;
}

.section-header h2 {
  margin: 0;
  padding: 0;
  border: none;
}

.btn-toggle {
  padding: 10px 20px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-toggle:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.password-form-container {
  margin-top: 24px;
  padding-top: 24px;
  border-top: 2px solid #e0e0e0;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-item label {
  font-size: 12px;
  font-weight: 600;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-item p {
  font-size: 18px;
  font-weight: 500;
  color: #000;
  margin: 0;
}

.password-form {
  max-width: 500px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
  font-size: 14px;
}

.form-group input {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
  box-sizing: border-box;
}

.form-group input:focus {
  outline: none;
  border-color: #000;
}

.btn-primary {
  padding: 12px 24px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  margin-top: 8px;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  margin-top: 16px;
  padding: 12px;
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  font-size: 14px;
}

.success-message {
  margin-top: 16px;
  padding: 12px;
  background: #efe;
  border: 1px solid #cfc;
  border-radius: 6px;
  color: #060;
  font-size: 14px;
}

@media (max-width: 768px) {
  .profile {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .profile-section {
    padding: 24px;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }
}
</style>


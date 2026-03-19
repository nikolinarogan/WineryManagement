<template>
  <div class="create-podrum-container">
    <div class="header">
      <router-link to="/podrumi" class="back-link">← Nazad</router-link>
      <h1>Kreiraj novi podrum</h1>
      <p class="subtitle">Dodajte novi podrum za čuvanje i lagering vina</p>
    </div>

    <form @submit.prevent="handleSubmit" class="podrum-form">
      <div class="form-section">
        <h2>Osnovni podaci</h2>
        
        <div class="form-group">
          <label for="nazivpod">Naziv podruma *</label>
          <input
            id="nazivpod"
            v-model="form.nazivpod"
            type="text"
            required
            maxlength="100"
            placeholder="npr. Podrum 1, Glavni Podrum..."
          />
        </div>

        <div class="form-group">
          <label for="temp">Temperatura (°C) *</label>
          <input
            id="temp"
            v-model.number="form.temp"
            type="number"
            step="0.1"
            min="-5"
            max="30"
            required
            placeholder="npr. 12.5"
          />
          <small>Temperatura mora biti između -5°C i 30°C. Optimalna temperatura za lagering vina je 10-15°C.</small>
        </div>
      </div>

      <div class="form-actions">
        <router-link to="/podrumi" class="btn-secondary">Odustani</router-link>
        <button type="submit" class="btn-primary" :disabled="submitting">
          {{ submitting ? 'Kreiranje...' : 'Kreiraj Podrum' }}
        </button>
      </div>

      <div v-if="error" class="error-message">{{ error }}</div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import podrumService from '@/services/podrumService'

const router = useRouter()

const form = ref({
  nazivpod: '',
  temp: 12
})

const submitting = ref(false)
const error = ref('')

const handleSubmit = async () => {
  try {
    submitting.value = true
    error.value = ''

    await podrumService.createPodrum(form.value)
    
    router.push('/podrumi')
  } catch (err: any) {
    console.error('Greška pri kreiranju podruma:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju podruma'
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.create-podrum-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  margin-bottom: 30px;
}

.back-link {
  display: inline-block;
  color: #666;
  text-decoration: none;
  margin-bottom: 16px;
  font-size: 14px;
  transition: color 0.3s;
}

.back-link:hover {
  color: #000;
}

.header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #000;
  margin-bottom: 8px;
}

.subtitle {
  font-size: 16px;
  color: #666;
}

.podrum-form {
  background: #fff;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-section {
  margin-bottom: 32px;
  padding-bottom: 32px;
  border-bottom: 2px solid #f0f0f0;
}

.form-section:last-of-type {
  border-bottom: none;
}

.form-section h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin-bottom: 16px;
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
}

.form-group input:focus {
  outline: none;
  border-color: #000;
}

.form-group small {
  display: block;
  margin-top: 4px;
  font-size: 12px;
  color: #666;
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.btn-secondary,
.btn-primary {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: inline-block;
  border: none;
}

.btn-secondary {
  background: #f0f0f0;
  color: #000;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.btn-primary {
  background: #000;
  color: #fff;
}

.btn-primary:hover {
  background: #333;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.error-message {
  margin-top: 20px;
  padding: 12px;
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
  text-align: center;
}
</style>


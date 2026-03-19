<template>
  <div class="create-sorta">
    <div class="create-container">
      <div class="header">
        <button @click="router.back()" class="back-btn">← Nazad</button>
        <h1>Dodaj novu sortu grožđa</h1>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="form-section">
          <div class="form-group">
            <label for="nazivsorte">Naziv sorte *</label>
            <input
              id="nazivsorte"
              v-model="form.nazivsorte"
              type="text"
              required
              placeholder="npr. Chardonnay"
            />
          </div>

          <div class="form-group">
            <label for="bojasorte">Boja sorte *</label>
            <select id="bojasorte" v-model="form.bojasorte" required>
              <option value="">Izaberite boju</option>
              <option value="bela">Bela</option>
              <option value="crvena">Crvena</option>
              <option value="roze">Roze</option>
            </select>
          </div>

          <div class="form-group">
            <label for="porijeklosorte">Porijeklo *</label>
            <input
              id="porijeklosorte"
              v-model="form.porijeklosorte"
              type="text"
              required
              placeholder="npr. Francuska"
            />
          </div>

          <div class="form-group">
            <label for="periodsazr">Period sazrijevanja *</label>
            <select id="periodsazr" v-model="form.periodsazr" required>
              <option value="">Izaberite period</option>
              <option value="Rani">Rani</option>
              <option value="Srednji">Srednji</option>
              <option value="Kasni">Kasni</option>
            </select>
          </div>

          <div class="form-group">
            <label for="kiselost">Kiselost (g/L) *</label>
            <input
              id="kiselost"
              v-model.number="form.kiselost"
              type="number"
              step="0.01"
              required
              placeholder="npr. 6.5"
            />
            <small>Prosječna kiselost u gramima po litru</small>
          </div>
        </div>

        <div class="form-actions">
          <router-link to="/sorte" class="btn-secondary">
            Odustani
          </router-link>
          <button type="submit" class="btn-primary" :disabled="submitting">
            {{ submitting ? 'Kreiranje...' : 'Kreiraj sortu' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import sortagrozdjaService from '@/services/sortagrozdjaService'
import type { CreateSortagrozdjaRequest } from '@/types/sortagrozdja'

const router = useRouter()
const submitting = ref(false)
const error = ref('')

const form = ref<CreateSortagrozdjaRequest>({
  nazivsorte: '',
  bojasorte: '',
  porijeklosorte: '',
  periodsazr: '',
  kiselost: 0
})

const handleSubmit = async () => {
  try {
    submitting.value = true
    error.value = ''
    
    await sortagrozdjaService.createSorta(form.value)
    router.push('/sorte')
  } catch (err: any) {
    console.error('Error creating sorta:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju sorte'
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.create-sorta {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.create-container {
  max-width: 700px;
  margin: 0 auto;
}

.header {
  margin-bottom: 40px;
}

.back-btn {
  background: none;
  border: none;
  color: #666;
  font-size: 16px;
  cursor: pointer;
  padding: 8px 0;
  margin-bottom: 16px;
  display: inline-block;
  transition: color 0.2s;
}

.back-btn:hover {
  color: #000;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.form-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.form-group {
  margin-bottom: 24px;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
  font-size: 14px;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #000;
}

.form-group input::placeholder {
  color: #999;
}

.form-group small {
  display: block;
  margin-top: 6px;
  color: #666;
  font-size: 12px;
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
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
  text-decoration: none;
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

.btn-secondary {
  padding: 12px 24px;
  background: #f0f0f0;
  color: #000;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: inline-block;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.error-message {
  margin-top: 16px;
  padding: 12px;
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  font-size: 14px;
  text-align: center;
}

@media (max-width: 768px) {
  .create-sorta {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .form-section {
    padding: 24px;
  }

  .form-actions {
    flex-direction: column-reverse;
  }
}
</style>


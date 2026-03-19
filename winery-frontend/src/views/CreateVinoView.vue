<template>
  <div class="create-vino-container">
    <div class="header">
      <router-link to="/vina" class="back-link">← Nazad</router-link>
      <h1>Kreiraj finalno vino</h1>
      <p class="subtitle">Kreirajte novo finalno vino od jednog ili više sirovih vina</p>
    </div>

    <div v-if="loadingSirovina" class="loading">Učitavanje sirovih vina...</div>

    <div v-else-if="errorSirovina" class="error-message">
      {{ errorSirovina }}
    </div>

    <form v-else @submit.prevent="handleSubmit" class="vino-form">
      <div class="form-section">
        <h2>Osnovni podaci</h2>
        
        <div class="form-group">
          <label for="nazivvina">Naziv vina *</label>
          <input
            id="nazivvina"
            v-model="form.nazivvina"
            type="text"
            required
            maxlength="100"
            placeholder="npr. Cabernet Sauvignon Premium 2024"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="procalk">Procenat alkohola *</label>
            <input
              id="procalk"
              v-model.number="form.procalk"
              type="number"
              step="0.1"
              min="0"
              max="20"
              required
              placeholder="npr. 13.5"
            />
            <small>Unesite procenat alkohola (0-20%)</small>
          </div>

          <div class="form-group">
            <label for="tipvina">Tip vina *</label>
            <select id="tipvina" v-model="form.tipvina" required>
              <option value="">Izaberite tip</option>
              <option value="Crveno">Crveno</option>
              <option value="Belo">Belo</option>
              <option value="Roze">Roze</option>
            </select>
          </div>
        </div>
      </div>

      <div class="form-section">
        <h2>Izbor sirovih vina (blend)</h2>
        <p class="section-note">Izaberite jedno ili više sirovih vina koja će činiti finalno vino</p>

        <div v-if="availableSirovina.length === 0" class="no-data">
          <p>Nema dostupnih sirovih vina. Prvo kreirajte sirovo vino.</p>
        </div>

        <div v-else class="sirovina-selection">
          <div 
            v-for="sirovina in availableSirovina" 
            :key="sirovina.idsirvina"
            class="sirovina-item"
            :class="{ selected: form.sirovaVinaIds.includes(sirovina.idsirvina) }"
            @click="toggleSirovina(sirovina.idsirvina)"
          >
            <input 
              type="checkbox"
              :checked="form.sirovaVinaIds.includes(sirovina.idsirvina)"
              @click.stop="toggleSirovina(sirovina.idsirvina)"
            />
            <div class="sirovina-info">
              <strong>{{ sirovina.nazivsirvina }}</strong>
              <div class="sirovina-details">
                <span class="kvalitet-badge">{{ sirovina.kvalitet }}</span>
                <span>{{ sirovina.kolicinasirvina }} L</span>
                <span>{{ sirovina.godproizvodnje }}</span>
              </div>
            </div>
          </div>
        </div>

        <div v-if="form.sirovaVinaIds.length > 0" class="selected-summary">
          <strong>Izabrano: {{ form.sirovaVinaIds.length }} sirovo vino{{ form.sirovaVinaIds.length > 1 ? '' : '' }}</strong>
        </div>
      </div>

      <div class="form-actions">
        <router-link to="/vina" class="btn-secondary">Odustani</router-link>
        <button type="submit" class="btn-primary" :disabled="submitting || form.sirovaVinaIds.length === 0">
          {{ submitting ? 'Kreiranje...' : 'Kreiraj Vino' }}
        </button>
      </div>

      <div v-if="error" class="error-message">{{ error }}</div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import vinoService from '@/services/vinoService'
import sirovovinoService from '@/services/sirovovinoService'
import type { Sirovovino } from '@/types/sirovovino'

const router = useRouter()

const form = ref({
  nazivvina: '',
  procalk: 0,
  tipvina: '',
  sirovaVinaIds: [] as number[]
})

const availableSirovina = ref<Sirovovino[]>([])
const loadingSirovina = ref(false)
const errorSirovina = ref('')
const submitting = ref(false)
const error = ref('')

const loadSirovina = async () => {
  loadingSirovina.value = true
  errorSirovina.value = ''
  try {
    availableSirovina.value = await sirovovinoService.getAllSirovina()
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovih vina:', err)
    errorSirovina.value = err.response?.data?.message || 'Greška pri učitavanju sirovih vina'
  } finally {
    loadingSirovina.value = false
  }
}

const toggleSirovina = (id: number) => {
  const index = form.value.sirovaVinaIds.indexOf(id)
  if (index > -1) {
    form.value.sirovaVinaIds.splice(index, 1)
  } else {
    form.value.sirovaVinaIds.push(id)
  }
}

const handleSubmit = async () => {
  if (form.value.sirovaVinaIds.length === 0) {
    error.value = 'Morate izabrati bar jedno sirovo vino'
    return
  }

  try {
    submitting.value = true
    error.value = ''

    await vinoService.createVino(form.value)
    
    router.push('/vina')
  } catch (err: any) {
    console.error('Greška pri kreiranju vina:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju vina'
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadSirovina()
})
</script>

<style scoped>
.create-vino-container {
  max-width: 900px;
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

.loading,
.error-message,
.no-data {
  text-align: center;
  padding: 40px;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
  margin-top: 20px;
}

.vino-form {
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

.section-note {
  font-size: 14px;
  color: #666;
  margin-bottom: 16px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
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

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #000;
}

.form-group small {
  display: block;
  margin-top: 4px;
  font-size: 12px;
  color: #666;
}

.sirovina-selection {
  display: grid;
  gap: 12px;
}

.sirovina-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
}

.sirovina-item:hover {
  border-color: #000;
  background: #f9f9f9;
}

.sirovina-item.selected {
  border-color: #000;
  background: #f0f0f0;
}

.sirovina-item input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.sirovina-info {
  flex: 1;
}

.sirovina-info strong {
  display: block;
  font-size: 16px;
  color: #000;
  margin-bottom: 4px;
}

.sirovina-details {
  display: flex;
  gap: 12px;
  font-size: 13px;
  color: #666;
}

.kvalitet-badge {
  padding: 2px 8px;
  background: #000;
  color: #fff;
  border-radius: 8px;
  font-size: 11px;
}

.selected-summary {
  margin-top: 16px;
  padding: 12px;
  background: #e8f5e9;
  border-radius: 8px;
  text-align: center;
  color: #2e7d32;
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
</style>


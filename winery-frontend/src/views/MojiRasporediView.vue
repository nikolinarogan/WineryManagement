<template>
  <div class="moji-rasporedi">
    <div class="container">
      <h1>Moji rasporedi branja</h1>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- No rasporedi -->
      <div v-else-if="rasporedi.length === 0" class="no-results">
        <p>Trenutno niste dodijeljeni ni na jedan raspored branja.</p>
      </div>

      <!-- Rasporedi Grid -->
      <div v-else class="rasporedi-grid">
        <div
          v-for="raspored in rasporedi"
          :key="raspored.idras"
          class="raspored-card"
        >
          <div class="raspored-header">
            <h3>{{ raspored.berbaNaziv }}</h3>
            <span :class="['status-badge', getStatusClass(raspored)]">
              {{ getStatus(raspored) }}
            </span>
          </div>

          <div class="raspored-body">
            <div class="info-section">
              <h4>Lokacija</h4>
              <div class="info-row">
                <span class="text">{{ raspored.parcelaNaziv }} ({{ raspored.vinogradNaziv }})</span>
              </div>
            </div>

            <div class="info-section">
              <h4>Period branja</h4>
              <div class="info-row">
                <span class="text">
                  {{ formatDate(raspored.pocbranja) }} - {{ formatDate(raspored.zavrsetakbranja) }}
                </span>
              </div>
            </div>

            <div class="info-section">
              <h4>Menadžer</h4>
              <div class="info-row">
                <span class="text">{{ raspored.menadzerIme }} {{ raspored.menadzerPrezime }}</span>
              </div>
            </div>

            <!-- Moja količina -->
            <div class="info-section highlighted">
              <h4>Moja količina</h4>
              <div v-if="raspored.mojDatumBranja" class="quantity-info">
                <div class="quantity-display">
                  <span class="quantity">{{ raspored.mojaKolicina }} kg</span>
                  <span class="date">{{ formatDate(raspored.mojDatumBranja) }}</span>
                </div>
                <button @click="openEditModal(raspored)" class="btn-edit">
                  Ažuriraj
                </button>
              </div>
              <div v-else class="no-quantity">
                <p>Niste unijeli količinu</p>
                <button @click="openEditModal(raspored)" class="btn-primary-small">
                  Unesi količinu
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Količina Modal -->
    <div v-if="showEditModal && selectedRaspored" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <h3>{{ selectedRaspored.mojDatumBranja ? 'Ažuriraj' : 'Unesi' }} Količinu</h3>
        
        <form @submit.prevent="saveKolicina">
          <div class="form-group">
            <label for="kolicina">Količina ubrano (kg) *</label>
            <input
              id="kolicina"
              v-model.number="kolicinaForm.kolicinaubrgr"
              type="number"
              step="0.01"
              min="0"
              required
              placeholder="npr. 45.50"
            />
          </div>

          <div class="form-group">
            <label for="datum">Datum branja *</label>
            <input
              id="datum"
              v-model="kolicinaForm.datumbranja"
              type="date"
              :min="formatDateForInput(selectedRaspored.pocbranja)"
              :max="formatDateForInput(selectedRaspored.zavrsetakbranja)"
              required
            />
            <small>Datum mora biti u periodu branja</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Čuvanje...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="saveError" class="error-message">{{ saveError }}</div>
          <div v-if="saveSuccess" class="success-message">Količina uspješno sačuvana!</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import berbaService from '@/services/berbaService'
import type { RadnikRaspored } from '@/types/berba'

const rasporedi = ref<RadnikRaspored[]>([])
const loading = ref(false)
const error = ref('')

const showEditModal = ref(false)
const selectedRaspored = ref<RadnikRaspored | null>(null)
const saving = ref(false)
const saveError = ref('')
const saveSuccess = ref(false)

const kolicinaForm = ref({
  kolicinaubrgr: 0,
  datumbranja: ''
})

const loadRasporedi = async () => {
  try {
    loading.value = true
    error.value = ''
    rasporedi.value = await berbaService.getMojiRasporediDetalji()
  } catch (err: any) {
    console.error('Error loading rasporedi:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju rasporeda'
  } finally {
    loading.value = false
  }
}

const openEditModal = (raspored: RadnikRaspored) => {
  selectedRaspored.value = raspored
  kolicinaForm.value = {
    kolicinaubrgr: raspored.mojaKolicina || 0,
    datumbranja: raspored.mojDatumBranja || new Date().toISOString().split('T')[0]
  }
  saveError.value = ''
  saveSuccess.value = false
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  selectedRaspored.value = null
  kolicinaForm.value = {
    kolicinaubrgr: 0,
    datumbranja: ''
  }
}

const saveKolicina = async () => {
  if (!selectedRaspored.value) return

  try {
    saving.value = true
    saveError.value = ''

    await berbaService.updateMojaKolicina(selectedRaspored.value.idras, kolicinaForm.value)
    
    saveSuccess.value = true
    setTimeout(async () => {
      await loadRasporedi()
      closeEditModal()
    }, 1500)
  } catch (err: any) {
    console.error('Error saving kolicina:', err)
    saveError.value = err.response?.data?.message || 'Greška pri čuvanju količine'
  } finally {
    saving.value = false
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS', { 
    day: '2-digit', 
    month: '2-digit', 
    year: 'numeric' 
  })
}

const formatDateForInput = (dateString: string) => {
  const date = new Date(dateString)
  return date.toISOString().split('T')[0]
}

const getStatus = (raspored: RadnikRaspored) => {
  const today = new Date()
  const start = new Date(raspored.pocbranja)
  const end = new Date(raspored.zavrsetakbranja)

  if (today < start) return 'Planirano'
  if (today > end) return 'Završeno'
  return 'Aktivan'
}

const getStatusClass = (raspored: RadnikRaspored) => {
  const status = getStatus(raspored)
  if (status === 'Aktivan') return 'status-active'
  if (status === 'Završeno') return 'status-finished'
  return 'status-planned'
}

onMounted(() => {
  loadRasporedi()
})
</script>

<style scoped>
.moji-rasporedi {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.container {
  max-width: 1400px;
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

.error-message {
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  padding: 16px;
  text-align: center;
  margin-bottom: 12px;
}

.success-message {
  background: #efe;
  border: 1px solid #cfc;
  border-radius: 6px;
  color: #060;
  padding: 16px;
  text-align: center;
  margin-top: 12px;
}

.no-results {
  background: white;
  padding: 60px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.no-results p {
  font-size: 18px;
  color: #666;
  margin: 0;
}

.rasporedi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(450px, 1fr));
  gap: 24px;
}

.raspored-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: all 0.3s;
}

.raspored-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.raspored-header {
  padding: 24px;
  background: linear-gradient(135deg, #1a1a1a 0%, #333 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.raspored-header h3 {
  margin: 0;
  font-size: 22px;
  font-weight: 600;
}

.status-badge {
  padding: 6px 16px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
}

.status-active {
  background: rgba(76, 175, 80, 0.9);
}

.status-finished {
  background: rgba(158, 158, 158, 0.9);
}

.status-planned {
  background: rgba(33, 150, 243, 0.9);
}

.raspored-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.info-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-section.highlighted {
  background: #f9f9f9;
  padding: 16px;
  border-radius: 8px;
  border: 2px solid #e0e0e0;
}

.info-section h4 {
  margin: 0;
  font-size: 13px;
  font-weight: 600;
  color: #999;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-row {
  display: flex;
  align-items: center;
}

.info-row .text {
  font-size: 16px;
  color: #000;
  font-weight: 500;
}

.quantity-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.quantity-display {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.quantity {
  font-size: 24px;
  font-weight: 700;
  color: #000;
}

.date {
  font-size: 14px;
  color: #666;
}

.no-quantity {
  text-align: center;
}

.no-quantity p {
  margin: 0 0 12px 0;
  color: #666;
}

.btn-edit {
  padding: 8px 16px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-edit:hover {
  background: #333;
}

.btn-primary-small {
  padding: 8px 16px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-primary-small:hover {
  background: #333;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  padding: 32px;
  border-radius: 12px;
  max-width: 500px;
  width: 90%;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.modal h3 {
  margin: 0 0 24px 0;
  font-size: 24px;
  color: #000;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
  font-size: 15px;
}

.form-group input {
  width: 100%;
  padding: 12px;
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
  margin-top: 6px;
  font-size: 13px;
  color: #666;
  font-style: italic;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.btn-secondary {
  padding: 10px 20px;
  background: #f0f0f0;
  color: #000;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.btn-primary {
  padding: 10px 20px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .moji-rasporedi {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .rasporedi-grid {
    grid-template-columns: 1fr;
  }
}
</style>

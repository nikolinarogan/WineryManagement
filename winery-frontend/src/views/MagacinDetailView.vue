<template>
  <div class="magacin-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="magacin" class="magacin-content">
      <div class="header">
        <router-link to="/magacini" class="back-link">← Nazad na magacine</router-link>
        <h1>{{ magacin.nazivmag }}</h1>
      </div>

      <!-- Osnovni podaci -->
      <div class="section">
        <div class="section-header">
          <h2>Osnovne informacije</h2>
          <button @click="openEditModal" class="btn-primary">✎</button>
        </div>

        <div class="capacity-card">
          <h3>Popunjenost magacina</h3>
          <div class="capacity-visual">
            <div class="capacity-bar-large">
              <div 
                class="capacity-fill-large" 
                :style="{ width: magacin.popunjenost + '%' }"
                :class="getCapacityClass(magacin.popunjenost)"
              >
                <span class="percentage-label">{{ magacin.popunjenost }}%</span>
              </div>
            </div>
            <div class="capacity-info-text">
              {{ magacin.brojBoca }} / {{ magacin.kapacitetmag }} boca
            </div>
          </div>
        </div>

        <div class="info-grid">
          <div class="info-item">
            <span class="label">Kapacitet:</span>
            <span class="value"><strong>{{ magacin.kapacitetmag }} boca</strong></span>
          </div>
          <div class="info-item">
            <span class="label">Temperatura:</span>
            <span class="value" :class="getTempClass(magacin.tempmag)">{{ magacin.tempmag }}°C</span>
          </div>
          <div class="info-item">
            <span class="label">Broj boca:</span>
            <span class="value">{{ magacin.brojBoca }}</span>
          </div>
          <div class="info-item">
            <span class="label">Slobodno:</span>
            <span class="value">{{ magacin.kapacitetmag - magacin.brojBoca }} boca</span>
          </div>
        </div>
      </div>

      <!-- Boce u Magacinu Section -->
      <div class="section">
        <h2>Boce u magacinu</h2>

        <div v-if="loadingBoce" class="loading-text">Učitavanje...</div>

        <div v-else-if="boce.length === 0" class="no-data">
          <p>Nema boca u ovom magacinu.</p>
        </div>

        <div v-else class="table-container">
          <table class="boce-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Vino</th>
                <th>Tip</th>
                <th>Zapremina</th>
                <th>Cena</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="boca in boce" :key="boca.idboce">
                <td>{{ boca.idboce }}</td>
                <td><strong>{{ boca.nazivVina }}</strong></td>
                <td>
                  <span class="tip-badge" :class="getTipClass(boca.tipVina)">
                    {{ boca.tipVina }}
                  </span>
                </td>
                <td>{{ boca.zapremina }} L</td>
                <td>{{ boca.cena ? boca.cena.toFixed(2) + ' RSD' : '-' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <h3>Uredi Magacin</h3>
        <form @submit.prevent="handleUpdate">
          <div class="form-group">
            <label for="edit-nazivmag">Naziv Magacina *</label>
            <input
              id="edit-nazivmag"
              v-model="editForm.nazivmag"
              type="text"
              required
              maxlength="100"
              placeholder="npr. Magacin 1"
            />
          </div>

          <div class="form-group">
            <label for="edit-kapacitetmag">Kapacitet (broj boca) *</label>
            <input
              id="edit-kapacitetmag"
              v-model.number="editForm.kapacitetmag"
              type="number"
              min="1"
              required
              placeholder="npr. 10000"
            />
          </div>

          <div class="form-group">
            <label for="edit-tempmag">Temperatura (°C) *</label>
            <input
              id="edit-tempmag"
              v-model.number="editForm.tempmag"
              type="number"
              step="0.1"
              min="-5"
              max="30"
              required
              placeholder="npr. 15"
            />
            <small>Temperatura mora biti između -5°C i 30°C</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="updating">
              {{ updating ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="editError" class="error-message">{{ editError }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import magacinService from '@/services/magacinService'
import bocaService from '@/services/bocaService'
import type { MagacinDetail } from '@/types/magacin'
import type { Boca } from '@/types/boca'

const route = useRoute()
const magacinId = parseInt(route.params.id as string)

const magacin = ref<MagacinDetail | null>(null)
const boce = ref<Boca[]>([])
const loading = ref(false)
const loadingBoce = ref(false)
const error = ref('')

// Edit modal
const showEditModal = ref(false)
const updating = ref(false)
const editError = ref('')
const editForm = ref({
  nazivmag: '',
  kapacitetmag: 0,
  tempmag: 0
})

const loadMagacin = async () => {
  loading.value = true
  error.value = ''
  try {
    magacin.value = await magacinService.getMagacinById(magacinId)
  } catch (err: any) {
    console.error('Greška pri učitavanju magacina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju magacina'
  } finally {
    loading.value = false
  }
}

const loadBoce = async () => {
  loadingBoce.value = true
  try {
    boce.value = await bocaService.getBoceByMagacin(magacinId)
  } catch (err: any) {
    console.error('Greška pri učitavanju boca:', err)
  } finally {
    loadingBoce.value = false
  }
}

const openEditModal = () => {
  if (!magacin.value) return
  
  editForm.value = {
    nazivmag: magacin.value.nazivmag,
    kapacitetmag: magacin.value.kapacitetmag,
    tempmag: magacin.value.tempmag
  }
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  editError.value = ''
}

const handleUpdate = async () => {
  if (!magacin.value) return

  try {
    updating.value = true
    editError.value = ''

    await magacinService.updateMagacin(magacin.value.idmag, editForm.value)
    
    await loadMagacin()
    closeEditModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju magacina:', err)
    editError.value = err.response?.data?.message || 'Greška pri ažuriranju magacina'
  } finally {
    updating.value = false
  }
}

const getTempClass = (temp: number) => {
  if (temp < 10) return 'temp-cold'
  if (temp < 18) return 'temp-optimal'
  return 'temp-warm'
}

const getCapacityClass = (percentage: number) => {
  if (percentage >= 90) return 'capacity-full'
  if (percentage >= 70) return 'capacity-high'
  if (percentage >= 40) return 'capacity-medium'
  return 'capacity-low'
}

const getTipClass = (tip: string) => {
  if (tip === 'Crveno') return 'tip-crveno'
  if (tip === 'Belo') return 'tip-belo'
  if (tip === 'Roze') return 'tip-roze'
  return ''
}

onMounted(async () => {
  await loadMagacin()
  await loadBoce()
})
</script>

<style scoped>
.magacin-detail-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 20px;
}

.loading,
.error-message {
  text-align: center;
  padding: 40px;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
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
  font-size: 32px;
  font-weight: 700;
  color: #000;
}

.section {
  background: #fff;
  border-radius: 12px;
  padding: 24px;
  margin-bottom: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin-bottom: 16px;
}

.capacity-card {
  background: linear-gradient(135deg, #f5f5f5, #e8e8e8);
  padding: 20px;
  border-radius: 12px;
  margin-bottom: 24px;
}

.capacity-card h3 {
  font-size: 18px;
  font-weight: 600;
  color: #000;
  margin-bottom: 16px;
}

.capacity-visual {
  margin-top: 12px;
}

.capacity-bar-large {
  height: 48px;
  background: #fff;
  border-radius: 24px;
  overflow: hidden;
  margin-bottom: 12px;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.1);
}

.capacity-fill-large {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: width 0.5s ease;
  border-radius: 24px;
  position: relative;
}

.capacity-fill-large.capacity-low {
  background: linear-gradient(90deg, #4CAF50, #66BB6A);
}

.capacity-fill-large.capacity-medium {
  background: linear-gradient(90deg, #FFC107, #FFD54F);
}

.capacity-fill-large.capacity-high {
  background: linear-gradient(90deg, #FF9800, #FFB74D);
}

.capacity-fill-large.capacity-full {
  background: linear-gradient(90deg, #F44336, #E57373);
}

.percentage-label {
  font-size: 20px;
  font-weight: 700;
  color: #fff;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.capacity-info-text {
  text-align: center;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 16px;
  background: #f9f9f9;
  border-radius: 8px;
}

.info-item .label {
  font-size: 14px;
  color: #666;
}

.info-item .value {
  font-size: 14px;
  font-weight: 600;
  color: #000;
}

.info-item .value.temp-cold {
  color: #2196F3;
}

.info-item .value.temp-optimal {
  color: #4CAF50;
}

.info-item .value.temp-warm {
  color: #FF9800;
}

/* Modal styles */
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

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.btn-secondary,
.btn-primary {
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
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

/* Boce Table */
.table-container {
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
  margin-top: 20px;
}

.boce-table {
  width: 100%;
  border-collapse: collapse;
}

.boce-table thead {
  background: #000;
  color: #fff;
}

.boce-table th {
  padding: 12px 16px;
  text-align: left;
  font-weight: 600;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.boce-table tbody tr {
  border-bottom: 1px solid #e0e0e0;
  transition: background 0.3s;
}

.boce-table tbody tr:hover {
  background: #f9f9f9;
}

.boce-table tbody tr:last-child {
  border-bottom: none;
}

.boce-table td {
  padding: 12px 16px;
  font-size: 14px;
  color: #000;
}

.tip-badge {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 700;
}

.tip-badge.tip-crveno {
  background: #ffebee;
  color: #c62828;
}

.tip-badge.tip-belo {
  background: #fff9c4;
  color: #f57f17;
}

.tip-badge.tip-roze {
  background: #fce4ec;
  color: #c2185b;
}

.loading-text {
  text-align: center;
  padding: 20px;
  color: #666;
}

.no-data {
  text-align: center;
  padding: 20px;
  color: #666;
}
</style>


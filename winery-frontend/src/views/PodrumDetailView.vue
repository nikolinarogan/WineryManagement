<template>
  <div class="podrum-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="podrum" class="podrum-content">
      <div class="header">
        <router-link to="/podrumi" class="back-link">← Nazad na podrume</router-link>
        <h1>{{ podrum.nazivpod }}</h1>
      </div>

      <!-- Osnovni podaci -->
      <div class="section">
        <div class="section-header">
          <h2>Osnovne informacije</h2>
          <button @click="openEditModal" class="btn-primary">✎ </button>
        </div>
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Temperatura:</span>
            <span class="value" :class="getTempClass(podrum.temp)">{{ podrum.temp }}°C</span>
          </div>
          <div class="info-item">
            <span class="label">Broj buradi:</span>
            <span class="value">{{ buradi.length }}</span>
          </div>
        </div>
      </div>

      <!-- Buradi Section -->
      <div class="section">
        <div class="section-header">
          <h2>Buradi u podrumu</h2>
          <button @click="openAddBureModal" class="btn-primary">+ </button>
        </div>

        <div v-if="loadingBuradi" class="loading-mini">Učitavanje buradi...</div>

        <div v-else-if="buradi.length === 0" class="no-data-mini">
          <p>Nema buradi u ovom podrumu.</p>
        </div>

        <div v-else class="buradi-grid">
          <div 
            v-for="bure in buradi" 
            :key="bure.idbur"
            class="bure-card"
          >
            <div class="bure-header">
              <h4>{{ bure.oznakabur }}</h4>
              <!-- <div class="bure-actions">
                <button @click="openEditBureModal(bure)" class="btn-icon" title="Uredi">✎</button>
                <button @click="deleteBure(bure)" class="btn-icon-delete" title="Obriši">×</button>
              </div> -->
            </div>
            <div class="bure-body">
              <div class="bure-info">
                <span class="label">Zapremina:</span>
                <span class="value"><strong>{{ bure.zapremina }} L</strong></span>
              </div>
              <div class="bure-info">
                <span class="label">Materijal:</span>
                <span class="value">{{ bure.materijal }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <h3>Uredi podrum</h3>
        <form @submit.prevent="handleUpdate">
          <div class="form-group">
            <label for="edit-nazivpod">Naziv podruma *</label>
            <input
              id="edit-nazivpod"
              v-model="editForm.nazivpod"
              type="text"
              required
              maxlength="100"
              placeholder="npr. Podrum 1"
            />
          </div>

          <div class="form-group">
            <label for="edit-temp">Temperatura (°C) *</label>
            <input
              id="edit-temp"
              v-model.number="editForm.temp"
              type="number"
              step="0.1"
              min="-5"
              max="30"
              required
              placeholder="npr. 12.5"
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

    <!-- Add Bure Modal -->
    <div v-if="showAddBureModal" class="modal-overlay" @click="closeAddBureModal">
      <div class="modal" @click.stop>
        <h3>Dodaj novo bure</h3>
        <form @submit.prevent="handleAddBure">
          <div class="form-group">
            <label for="add-oznakabur">Oznaka bureta *</label>
            <input
              id="add-oznakabur"
              v-model="addBureForm.oznakabur"
              type="text"
              required
              maxlength="50"
              placeholder="npr. B-001, Hrastovina-01"
            />
          </div>

          <div class="form-group">
            <label for="add-zapremina">Zapremina (L) *</label>
            <input
              id="add-zapremina"
              v-model.number="addBureForm.zapremina"
              type="number"
              step="0.01"
              min="0.01"
              required
              placeholder="npr. 225"
            />
            <small>Standardne zapreminen: 225L (Bordeaux), 228L (Burgundy), 300L</small>
          </div>

          <div class="form-group">
            <label for="add-materijal">Materijal *</label>
            <select id="add-materijal" v-model="addBureForm.materijal" required>
              <option value="">Izaberite materijal</option>
              <option value="Hrastovina (Francuski)">Hrastovina (Francuski)</option>
              <option value="Hrastovina (Američki)">Hrastovina (Američki)</option>
              <option value="Hrastovina (Srpski)">Hrastovina (Srpski)</option>
              <option value="Nerđajući čelik">Nerđajući čelik</option>
              <option value="Beton">Beton</option>
            </select>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeAddBureModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="submittingBure">
              {{ submittingBure ? 'Dodavanje...' : 'Dodaj Bure' }}
            </button>
          </div>

          <div v-if="bureError" class="error-message">{{ bureError }}</div>
        </form>
      </div>
    </div>

    <!-- Edit Bure Modal -->
    <div v-if="showEditBureModal" class="modal-overlay" @click="closeEditBureModal">
      <div class="modal" @click.stop>
        <h3>Uredi bure</h3>
        <form @submit.prevent="handleUpdateBure">
          <div class="form-group">
            <label for="edit-oznakabur-bure">Oznaka bureta *</label>
            <input
              id="edit-oznakabur-bure"
              v-model="editBureForm.oznakabur"
              type="text"
              required
              maxlength="50"
              placeholder="npr. B-001"
            />
          </div>

          <div class="form-group">
            <label for="edit-zapremina-bure">Zapremina (L) *</label>
            <input
              id="edit-zapremina-bure"
              v-model.number="editBureForm.zapremina"
              type="number"
              step="0.01"
              min="0.01"
              required
              placeholder="npr. 225"
            />
          </div>

          <div class="form-group">
            <label for="edit-materijal-bure">Materijal *</label>
            <select id="edit-materijal-bure" v-model="editBureForm.materijal" required>
              <option value="">Izaberite materijal</option>
              <option value="Hrastovina (Francuski)">Hrastovina (Francuski)</option>
              <option value="Hrastovina (Američki)">Hrastovina (Američki)</option>
              <option value="Hrastovina (Srpski)">Hrastovina (Srpski)</option>
              <option value="Nerđajući čelik">Nerđajući čelik</option>
              <option value="Beton">Beton</option>
            </select>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditBureModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="submittingBure">
              {{ submittingBure ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="bureError" class="error-message">{{ bureError }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import podrumService from '@/services/podrumService'
import bureService from '@/services/bureService'
import type { PodrumDetail } from '@/types/podrum'
import type { Bure } from '@/types/bure'

const route = useRoute()
const podrumId = parseInt(route.params.id as string)

const podrum = ref<PodrumDetail | null>(null)
const loading = ref(false)
const error = ref('')

// Buradi
const buradi = ref<Bure[]>([])
const loadingBuradi = ref(false)

// Edit podrum modal
const showEditModal = ref(false)
const updating = ref(false)
const editError = ref('')
const editForm = ref({
  nazivpod: '',
  temp: 0
})

// Add bure modal
const showAddBureModal = ref(false)
const submittingBure = ref(false)
const bureError = ref('')
const addBureForm = ref({
  oznakabur: '',
  zapremina: 225,
  materijal: ''
})

// Edit bure modal
const showEditBureModal = ref(false)
const selectedBureId = ref<number | null>(null)
const editBureForm = ref({
  oznakabur: '',
  zapremina: 0,
  materijal: ''
})

const loadPodrum = async () => {
  loading.value = true
  error.value = ''
  try {
    podrum.value = await podrumService.getPodrumById(podrumId)
  } catch (err: any) {
    console.error('Greška pri učitavanju podruma:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju podruma'
  } finally {
    loading.value = false
  }
}

const loadBuradi = async () => {
  loadingBuradi.value = true
  try {
    buradi.value = await bureService.getBuradiByPodrumId(podrumId)
  } catch (err: any) {
    console.error('Greška pri učitavanju buradi:', err)
  } finally {
    loadingBuradi.value = false
  }
}

const openEditModal = () => {
  if (!podrum.value) return
  
  editForm.value = {
    nazivpod: podrum.value.nazivpod,
    temp: podrum.value.temp
  }
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  editError.value = ''
}

const handleUpdate = async () => {
  if (!podrum.value) return

  try {
    updating.value = true
    editError.value = ''

    await podrumService.updatePodrum(podrum.value.idpod, editForm.value)
    
    await loadPodrum()
    closeEditModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju podruma:', err)
    editError.value = err.response?.data?.message || 'Greška pri ažuriranju podruma'
  } finally {
    updating.value = false
  }
}

const getTempClass = (temp: number) => {
  if (temp < 10) return 'temp-cold'
  if (temp < 18) return 'temp-optimal'
  return 'temp-warm'
}

// Bure functions
const openAddBureModal = () => {
  addBureForm.value = {
    oznakabur: '',
    zapremina: 225,
    materijal: ''
  }
  bureError.value = ''
  showAddBureModal.value = true
}

const closeAddBureModal = () => {
  showAddBureModal.value = false
  bureError.value = ''
}

const handleAddBure = async () => {
  try {
    submittingBure.value = true
    bureError.value = ''

    await bureService.createBure({
      ...addBureForm.value,
      podrumIdpod: podrumId
    })
    
    await loadBuradi()
    await loadPodrum()
    closeAddBureModal()
  } catch (err: any) {
    console.error('Greška pri dodavanju bureta:', err)
    bureError.value = err.response?.data?.message || 'Greška pri dodavanju bureta'
  } finally {
    submittingBure.value = false
  }
}

const openEditBureModal = (bure: Bure) => {
  selectedBureId.value = bure.idbur
  editBureForm.value = {
    oznakabur: bure.oznakabur,
    zapremina: bure.zapremina,
    materijal: bure.materijal
  }
  bureError.value = ''
  showEditBureModal.value = true
}

const closeEditBureModal = () => {
  showEditBureModal.value = false
  bureError.value = ''
  selectedBureId.value = null
}

const handleUpdateBure = async () => {
  if (!selectedBureId.value) return

  try {
    submittingBure.value = true
    bureError.value = ''

    await bureService.updateBure(selectedBureId.value, editBureForm.value)
    
    await loadBuradi()
    closeEditBureModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju bureta:', err)
    bureError.value = err.response?.data?.message || 'Greška pri ažuriranju bureta'
  } finally {
    submittingBure.value = false
  }
}

const deleteBure = async (bure: Bure) => {
  if (!confirm(`Da li ste sigurni da želite obrisati bure "${bure.oznakabur}"?`)) {
    return
  }

  try {
    await bureService.deleteBure(bure.idbur)
    await loadBuradi()
    await loadPodrum()
  } catch (err: any) {
    console.error('Greška pri brisanju bureta:', err)
    alert(err.response?.data?.message || 'Greška pri brisanju bureta')
  }
}

onMounted(() => {
  loadPodrum()
  loadBuradi()
})
</script>

<style scoped>
.podrum-detail-container {
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

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 12px;
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

/* Buradi Section */
.loading-mini,
.no-data-mini {
  text-align: center;
  padding: 20px;
  color: #666;
}

.buradi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 16px;
}

.bure-card {
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s;
}

.bure-card:hover {
  border-color: #000;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.bure-header {
  padding: 12px 16px;
  background: #f5f5f5;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 2px solid #e0e0e0;
}

.bure-header h4 {
  font-size: 16px;
  font-weight: 600;
  color: #000;
  margin: 0;
}

.bure-actions {
  display: flex;
  gap: 8px;
}

.btn-icon,
.btn-icon-delete {
  background: none;
  border: none;
  font-size: 16px;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.2s;
}

.btn-icon {
  color: #666;
}

.btn-icon:hover {
  background: #e0e0e0;
  color: #000;
}

.btn-icon-delete {
  color: #c62828;
}

.btn-icon-delete:hover {
  background: #ffebee;
  color: #b71c1c;
}

.bure-body {
  padding: 16px;
}

.bure-info {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}

.bure-info:last-child {
  border-bottom: none;
}

.bure-info .label {
  font-size: 13px;
  color: #666;
}

.bure-info .value {
  font-size: 13px;
  color: #000;
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
</style>


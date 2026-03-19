<template>
  <div class="vino-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="vino" class="vino-content">
      <div class="header">
        <router-link to="/vina" class="back-link">← Nazad na vina</router-link>
        <h1>{{ vino.nazivvina }}</h1>
      </div>

      <!-- Osnovni podaci -->
      <div class="section">
        <div class="section-header">
          <h2>Osnovne informacije</h2>
          <button @click="openEditModal" class="btn-primary">✎ </button>
        </div>
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Procenat alkohola:</span>
            <span class="value">{{ vino.procalk }}%</span>
          </div>
          <div class="info-item">
            <span class="label">Tip vina:</span>
            <span class="value">{{ vino.tipvina }}</span>
          </div>
        </div>
      </div>

      <!-- Sirova vina -->
      <div class="section">
        <h2>Sastav blenda</h2>
        <p class="section-note">Ovo vino je kreirano od sledećih sirovih vina:</p>
        
        <div class="sirova-vina-grid">
          <div 
            v-for="sv in vino.sirovaVina" 
            :key="sv.idsirvina"
            class="sirovo-vino-card"
          >
            <div class="card-header">
              <h3>{{ sv.nazivsirvina }}</h3>
              <span class="kvalitet-badge">{{ sv.kvalitet }}</span>
            </div>
            <div class="card-body">
              <div class="info-row">
                <span class="label">Količina:</span>
                <span class="value">{{ sv.kolicinasirvina }} L</span>
              </div>
              <div class="info-row">
                <span class="label">Godina:</span>
                <span class="value">{{ sv.godproizvodnje }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <h3>Uredi vino</h3>
        <form @submit.prevent="handleUpdate">
          <div class="form-group">
            <label for="edit-nazivvina">Naziv vina *</label>
            <input
              id="edit-nazivvina"
              v-model="editForm.nazivvina"
              type="text"
              required
              maxlength="100"
              placeholder="npr. Cabernet Sauvignon Premium 2024"
            />
          </div>

          <div class="form-group">
            <label for="edit-procalk">Procenat alkohola *</label>
            <input
              id="edit-procalk"
              v-model.number="editForm.procalk"
              type="number"
              step="0.1"
              min="0"
              max="20"
              required
              placeholder="npr. 13.5"
            />
          </div>

          <div class="form-group">
            <label for="edit-tipvina">Tip vina *</label>
            <select id="edit-tipvina" v-model="editForm.tipvina" required>
              <option value="">Izaberite tip</option>
              <option value="Crveno">Crveno</option>
              <option value="Belo">Belo</option>
              <option value="Roze">Roze</option>
            </select>
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
import vinoService from '@/services/vinoService'
import type { VinoDetail } from '@/types/vino'

const route = useRoute()
const vinoId = parseInt(route.params.id as string)

const vino = ref<VinoDetail | null>(null)
const loading = ref(false)
const error = ref('')

// Edit modal
const showEditModal = ref(false)
const updating = ref(false)
const editError = ref('')
const editForm = ref({
  nazivvina: '',
  procalk: 0,
  tipvina: ''
})

const loadVino = async () => {
  loading.value = true
  error.value = ''
  try {
    vino.value = await vinoService.getVinoById(vinoId)
  } catch (err: any) {
    console.error('Greška pri učitavanju vina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju vina'
  } finally {
    loading.value = false
  }
}

const openEditModal = () => {
  if (!vino.value) return
  
  editForm.value = {
    nazivvina: vino.value.nazivvina,
    procalk: vino.value.procalk,
    tipvina: vino.value.tipvina
  }
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  editError.value = ''
}

const handleUpdate = async () => {
  if (!vino.value) return

  try {
    updating.value = true
    editError.value = ''

    await vinoService.updateVino(vino.value.idvina, editForm.value)
    
    await loadVino()
    closeEditModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju vina:', err)
    editError.value = err.response?.data?.message || 'Greška pri ažuriranju vina'
  } finally {
    updating.value = false
  }
}

onMounted(() => {
  loadVino()
})
</script>

<style scoped>
.vino-detail-container {
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

.section-note {
  font-size: 14px;
  color: #666;
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

.sirova-vina-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}

.sirovo-vino-card {
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s;
}

.sirovo-vino-card:hover {
  border-color: #000;
}

.sirovo-vino-card .card-header {
  padding: 16px;
  background: #000;
  color: #fff;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sirovo-vino-card .card-header h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0;
}

.kvalitet-badge {
  padding: 4px 8px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  font-size: 11px;
}

.sirovo-vino-card .card-body {
  padding: 16px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-row:last-child {
  border-bottom: none;
}

.info-row .label {
  font-size: 13px;
  color: #666;
}

.info-row .value {
  font-size: 13px;
  font-weight: 600;
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


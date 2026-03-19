<template>
  <div class="sirova-vina-container">
    <div class="header">
      <h1>Sirova vina</h1>
      <p class="subtitle">Pregled svih kreiranih sirovih vina</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="vina.length === 0" class="no-data">
      <p>Još nema kreiranih sirovih vina.</p>
    </div>

    <div v-else class="content">
      <div class="vina-grid">
        <div 
          v-for="vino in vina" 
          :key="vino.idsirvina"
          class="vino-card"
        >
          <div class="card-header">
            <h3>{{ vino.nazivsirvina }}</h3>
            <span class="kvalitet-badge">{{ vino.kvalitet }}</span>
          </div>

          <div class="card-body">
            <div class="main-info">
              <div class="info-item">
                <span class="label">Količina</span>
                <span class="value"><strong>{{ vino.kolicinasirvina }} L</strong></span>
              </div>
              <div class="info-item">
                <span class="label">Godina</span>
                <span class="value"><strong>{{ vino.godproizvodnje }}</strong></span>
              </div>
            </div>

            <div class="poreklo-section">
              <h4>Porijeklo:</h4>
              <div v-for="(p, index) in vino.poreklo" :key="index" class="poreklo-item">
                <div class="poreklo-info">
                  <strong>{{ p.berbaNaziv }}</strong> - {{ p.parcelaNaziv }}
                </div>
                <div class="poreklo-details">
                  <span v-if="p.sortaNaziv" class="sorta">{{ p.sortaNaziv }}</span>
                  <span class="tretmani">{{ p.brojTretmana }} tretmana</span>
                </div>
              </div>
            </div>
          </div>

          <div class="card-actions">
            <button @click="viewDetail(vino.idsirvina)" class="btn-view">Vidi detalje</button>
            <button @click="viewDetail(vino.idsirvina)" class="btn-create">Lagerovanje</button>
            <button @click="openEditModal(vino)" class="btn-edit">✎ Uredi</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <h3>Uredi sirovo vino</h3>
        <form @submit.prevent="handleUpdate">
          <div class="form-group">
            <label for="edit-naziv">Naziv sirovog vina *</label>
            <input
              id="edit-naziv"
              v-model="editForm.nazivsirvina"
              type="text"
              required
              maxlength="100"
              placeholder="npr. Merlot Sirovo 2024"
            />
          </div>

          <div class="form-group">
            <label for="edit-kolicina">Količina (litri) *</label>
            <input
              id="edit-kolicina"
              v-model.number="editForm.kolicinasirvina"
              type="number"
              required
              step="0.01"
              min="0.01"
              placeholder="Unesite količinu u litrima"
            />
          </div>

          <div class="form-group">
            <label for="edit-kvalitet">Kvalitet *</label>
            <select id="edit-kvalitet" v-model="editForm.kvalitet" required>
              <option value="">Izaberite kvalitet</option>
              <option value="Premium">Premium</option>
              <option value="Standard">Standard</option>
              <option value="Osnovno">Osnovno</option>
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
import { useRouter } from 'vue-router'
import sirovovinoService from '@/services/sirovovinoService'
import type { Sirovovino } from '@/types/sirovovino'

const router = useRouter()
const vina = ref<Sirovovino[]>([])
const loading = ref(false)
const error = ref('')

// Edit modal
const showEditModal = ref(false)
const updating = ref(false)
const editError = ref('')
const selectedVinoId = ref<number | null>(null)
const editForm = ref({
  nazivsirvina: '',
  kolicinasirvina: 0,
  kvalitet: ''
})

const loadVina = async () => {
  loading.value = true
  error.value = ''
  try {
    vina.value = await sirovovinoService.getAllSirovina()
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovih vina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju sirovih vina'
  } finally {
    loading.value = false
  }
}

const viewDetail = (id: number) => {
  router.push(`/sirova-vina/${id}`)
}

const openEditModal = (vino: Sirovovino) => {
  selectedVinoId.value = vino.idsirvina
  editForm.value = {
    nazivsirvina: vino.nazivsirvina,
    kolicinasirvina: vino.kolicinasirvina,
    kvalitet: vino.kvalitet
  }
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  editError.value = ''
  selectedVinoId.value = null
}

const handleUpdate = async () => {
  if (!selectedVinoId.value) return

  try {
    updating.value = true
    editError.value = ''

    await sirovovinoService.updateSirovovino(selectedVinoId.value, editForm.value)
    
    await loadVina()
    closeEditModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju sirovog vina:', err)
    editError.value = err.response?.data?.message || 'Greška pri ažuriranju sirovog vina'
  } finally {
    updating.value = false
  }
}

onMounted(() => {
  loadVina()
})
</script>

<style scoped>
.sirova-vina-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  margin-bottom: 30px;
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
}

.vina-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 20px;
}

.vino-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.vino-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.card-header {
  padding: 20px;
  background: #000;
  color: #fff;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}

.kvalitet-badge {
  padding: 4px 12px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.card-body {
  padding: 20px;
}

.main-info {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
  padding-bottom: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.info-item {
  flex: 1;
  text-align: center;
}

.info-item .label {
  display: block;
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 4px;
}

.info-item .value {
  display: block;
  font-size: 18px;
  color: #000;
}

.poreklo-section h4 {
  font-size: 14px;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 12px;
}

.poreklo-item {
  background: #f5f5f5;
  padding: 12px;
  border-radius: 8px;
  margin-bottom: 8px;
}

.poreklo-info {
  font-size: 14px;
  color: #000;
  margin-bottom: 4px;
}

.poreklo-details {
  display: flex;
  gap: 12px;
  font-size: 12px;
}

.sorta {
  padding: 2px 8px;
  background: #000;
  color: #fff;
  border-radius: 8px;
}

.tretmani {
  color: #666;
}

.card-actions {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  gap: 8px;
}

.btn-view,
.btn-create,
.btn-edit,
.btn-delete {
  flex: 1;
  padding: 10px 16px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: inline-block;
  text-align: center;
}

.btn-view {
  background: #e3f2fd;
  color: #1976d2;
}

.btn-view:hover {
  background: #1976d2;
  color: #fff;
}

.btn-create {
  background: #fff3e0;
  color: #f57c00;
}

.btn-create:hover {
  background: #f57c00;
  color: #fff;
}

.btn-edit {
  background: #e3f2fd;
  color: #1976d2;
}

.btn-edit:hover {
  background: #1976d2;
  color: #fff;
}

.btn-delete {
  background: #ffebee;
  color: #c62828;
}

.btn-delete:hover {
  background: #c62828;
  color: #fff;
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

.btn-primary:hover {
  background: #333;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}
</style>


<template>
  <div class="degustacija-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="degustacija" class="content">
      <!-- Header -->
      <div class="header">
        <div>
          <h1>{{ degustacija.nazivdeg }}</h1>
          <p class="subtitle">{{ formatDate(degustacija.datdeg) }}</p>
        </div>
        <div class="header-actions">
          <button @click="showEditModal = true" class="btn-edit">✎ </button>
          <button @click="confirmDelete" class="btn-delete">Obriši</button>
          <router-link to="/degustacije" class="btn-secondary">← Nazad</router-link>
        </div>
      </div>

      <!-- Info Section -->
      <div class="info-card">
        <h2>Informacije o degustaciji</h2>
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Datum</span>
            <span class="value">{{ formatDate(degustacija.datdeg) }}</span>
          </div>
          <div class="info-item">
            <span class="label">Kapacitet</span>
            <span class="value">{{ degustacija.kapacitetdeg }} osoba</span>
          </div>
          <div class="info-item">
            <span class="label">Broj vina</span>
            <span class="value">{{ degustacija.vina.length }}</span>
          </div>
        </div>
      </div>

      <!-- Vina Section -->
      <div class="section-card">
        <h2>Vina na degustaciji</h2>
        <div v-if="degustacija.vina.length === 0" class="no-data">
          Nema izabranih vina.
        </div>
        <div v-else class="vina-grid">
          <div v-for="vino in degustacija.vina" :key="vino.idvina" class="vino-card">
            <strong>{{ vino.nazivvina }}</strong>
            <span class="tip-badge" :class="getTipClass(vino.tipvina)">
              {{ vino.tipvina }}
            </span>
          </div>
        </div>
      </div>

      <!-- Somelijeri Section -->
      <div class="section-card">
        <h2>Organizatori</h2>
        <div v-if="degustacija.somelijeri.length === 0" class="no-data">
          Nema dodeljenih somelijera.
        </div>
        <div v-else class="somelijeri-list">
          <div v-for="someljer in degustacija.somelijeri" :key="someljer.idzap" class="someljer-item">
            <div>
              <strong>{{ someljer.ime }} {{ someljer.prezime }}</strong>
              <p class="specijalnost">{{ someljer.specijalnost }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>Uredi degustaciju</h2>
          <button @click="closeEditModal" class="close-btn">×</button>
        </div>

        <form @submit.prevent="handleUpdate" class="modal-body">
          <div class="form-group">
            <label for="edit-naziv">Naziv degustacije *</label>
            <input
              id="edit-naziv"
              v-model="editForm.nazivdeg"
              type="text"
              required
            />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="edit-datum">Datum *</label>
              <input
                id="edit-datum"
                v-model="editForm.datdeg"
                type="date"
                required
              />
            </div>

            <div class="form-group">
              <label for="edit-kapacitet">Kapacitet *</label>
              <input
                id="edit-kapacitet"
                v-model.number="editForm.kapacitetdeg"
                type="number"
                min="1"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label>Vina *</label>
            <div v-if="loadingVina" class="loading-small">Učitavanje...</div>
            <div v-else class="vina-selection">
              <div v-for="vino in availableVina" :key="vino.idvina" class="vino-item">
                <label class="checkbox-label">
                  <input
                    type="checkbox"
                    :value="vino.idvina"
                    v-model="editForm.vinaIds"
                  />
                  <span>{{ vino.nazivvina }} ({{ vino.tipvina }})</span>
                </label>
              </div>
            </div>
          </div>

          <div v-if="editError" class="error-message">{{ editError }}</div>

          <div class="modal-actions">
            <button type="button" @click="closeEditModal" class="btn-secondary">Odustani</button>
            <button type="submit" class="btn-primary" :disabled="updating">
              {{ updating ? 'Čuvanje...' : 'Sačuvaj' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import degustacijaService from '@/services/degustacijaService'
import vinoService from '@/services/vinoService'
import type { DegustacijaDetail } from '@/types/degustacija'
import type { Vino } from '@/types/vino'

const route = useRoute()
const router = useRouter()

const degustacija = ref<DegustacijaDetail | null>(null)
const loading = ref(false)
const error = ref('')

const showEditModal = ref(false)
const loadingVina = ref(false)
const updating = ref(false)
const editError = ref('')
const availableVina = ref<Vino[]>([])

const editForm = ref({
  nazivdeg: '',
  datdeg: '',
  kapacitetdeg: 0,
  vinaIds: [] as number[]
})

const loadDegustacija = async () => {
  loading.value = true
  error.value = ''
  try {
    const id = Number(route.params.id)
    degustacija.value = await degustacijaService.getDegustacijaById(id)
  } catch (err: any) {
    console.error('Greška pri učitavanju degustacije:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju degustacije'
  } finally {
    loading.value = false
  }
}

const loadVina = async () => {
  loadingVina.value = true
  try {
    availableVina.value = await vinoService.getAllVina()
  } catch (err: any) {
    console.error('Greška pri učitavanju vina:', err)
  } finally {
    loadingVina.value = false
  }
}

const openEditModal = () => {
  if (degustacija.value) {
    editForm.value = {
      nazivdeg: degustacija.value.nazivdeg,
      datdeg: degustacija.value.datdeg,
      kapacitetdeg: degustacija.value.kapacitetdeg,
      vinaIds: degustacija.value.vina.map(v => v.idvina)
    }
    showEditModal.value = true
    loadVina()
  }
}

const closeEditModal = () => {
  showEditModal.value = false
  editError.value = ''
}

const handleUpdate = async () => {
  updating.value = true
  editError.value = ''

  try {
    const id = Number(route.params.id)
    const updated = await degustacijaService.updateDegustacija(id, editForm.value)
    degustacija.value = updated
    closeEditModal()
  } catch (err: any) {
    console.error('Greška pri ažuriranju:', err)
    editError.value = err.response?.data?.message || 'Greška pri ažuriranju degustacije'
  } finally {
    updating.value = false
  }
}

const confirmDelete = () => {
  if (confirm('Da li ste sigurni da želite da obrišete ovu degustaciju?')) {
    deleteDegustacija()
  }
}

const deleteDegustacija = async () => {
  try {
    const id = Number(route.params.id)
    await degustacijaService.deleteDegustacija(id)
    router.push('/degustacije')
  } catch (err: any) {
    console.error('Greška pri brisanju:', err)
    error.value = err.response?.data?.message || 'Greška pri brisanju degustacije'
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}

const getTipClass = (tip: string) => {
  if (tip === 'Crveno') return 'tip-crveno'
  if (tip === 'Belo') return 'tip-belo'
  if (tip === 'Roze') return 'tip-roze'
  return ''
}

onMounted(() => {
  loadDegustacija()
  // Otvori edit modal ako je potrebno
  if (showEditModal.value) {
    openEditModal()
  }
})
</script>

<style scoped>
.degustacija-detail-container {
  max-width: 1200px;
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
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 30px;
}

.header h1 {
  font-size: 32px;
  font-weight: 700;
  color: #000;
  margin-bottom: 8px;
}

.subtitle {
  font-size: 18px;
  color: #666;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.btn-secondary,
.btn-edit,
.btn-delete {
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 14px;
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

.btn-edit {
  background: #000;
  color: #fff;
}

.btn-edit:hover {
  background: #333;
}

.btn-delete {
  background: #c62828;
  color: #fff;
}

.btn-delete:hover {
  background: #8e0000;
}

.info-card,
.section-card {
  background: #fff;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.info-card h2,
.section-card h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin-bottom: 20px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.label {
  font-size: 13px;
  color: #666;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.value {
  font-size: 20px;
  color: #000;
  font-weight: 700;
}

.no-data {
  text-align: center;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 8px;
  color: #666;
}

.vina-grid {
  display: grid;
  gap: 12px;
}

.vino-card {
  background: #f9f9f9;
  padding: 16px;
  border-radius: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.vino-card strong {
  font-size: 15px;
  color: #000;
}

.tip-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
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

.somelijeri-list {
  display: grid;
  gap: 12px;
}

.someljer-item {
  background: #f9f9f9;
  padding: 16px;
  border-radius: 8px;
}

.someljer-item strong {
  font-size: 15px;
  color: #000;
}

.specijalnost {
  margin-top: 4px;
  font-size: 13px;
  color: #666;
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
  background: #fff;
  border-radius: 12px;
  width: 90%;
  max-width: 700px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 24px;
  border-bottom: 2px solid #f0f0f0;
}

.modal-header h2 {
  font-size: 22px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 32px;
  color: #666;
  cursor: pointer;
  line-height: 1;
  padding: 0;
  width: 32px;
  height: 32px;
}

.close-btn:hover {
  color: #000;
}

.modal-body {
  padding: 24px;
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

.form-group input[type="text"],
.form-group input[type="date"],
.form-group input[type="number"] {
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

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.loading-small {
  text-align: center;
  padding: 20px;
  color: #666;
}

.vina-selection {
  max-height: 300px;
  overflow-y: auto;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  padding: 12px;
}

.vino-item {
  padding: 8px;
  border-radius: 6px;
  transition: all 0.3s;
}

.vino-item:hover {
  background: #f9f9f9;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  width: 18px;
  height: 18px;
  cursor: pointer;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.btn-primary {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  border: none;
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


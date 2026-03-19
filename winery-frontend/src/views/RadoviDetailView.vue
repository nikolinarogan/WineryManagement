<template>
  <div class="radovi-detail">
    <div class="container">
      <div v-if="loading" class="loading">Učitavanje...</div>
      <div v-else-if="error" class="error-message">{{ error }}</div>
      
      <div v-else-if="rad" class="rad-content">
        <div class="header">
          <router-link to="/radovi" class="back-link">← Nazad</router-link>
          <h1>Rad #{{ rad.idrad }}</h1>
        </div>

        <!-- Osnovni podaci -->
        <div class="section">
          <div class="section-header">
            <h2>Osnovne informacije</h2>
            <button @click="openEditRadoviModal" class="btn-primary">✎ Uredi radove</button>
          </div>
          <div class="info-grid">
            <div class="info-item">
              <span class="label">Početak:</span>
              <span class="value">{{ formatDate(rad.pocrad) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Završetak:</span>
              <span class="value">{{ formatDate(rad.zavrrad) }}</span>
            </div>
            <div class="info-item">
              <span class="label">Oprema:</span>
              <span class="value">{{ rad.oprema }}</span>
            </div>
          </div>
        </div>

        <!-- Parcele -->
        <div class="section">
          <h2>Parcele</h2>
          <div class="parcele-list">
            <div v-for="parcela in rad.parcele" :key="parcela.idp" class="parcela-item">
              <strong>{{ parcela.nazivparcele }}</strong> - {{ parcela.vinogradNaziv }} ({{ parcela.povrsina }} ha)
            </div>
          </div>
        </div>

        <!-- Radnici -->
        <div class="section">
          <div class="section-header">
            <h2>Angažovani radnici</h2>
            <button @click="showAddRadnikModal = true" class="btn-primary">+ Dodaj radnika</button>
          </div>

          <div v-if="rad.radnici.length === 0" class="no-results">
            <p>Nema dodijeljenih radnika.</p>
          </div>

          <div v-else class="radnici-table">
            <table>
              <thead>
                <tr>
                  <th>Ime i prezime</th>
                  <th>Email</th>
                  <th>Akcije</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="radnik in rad.radnici" :key="radnik.radnikIdzap">
                  <td>{{ radnik.ime }} {{ radnik.prezime }}</td>
                  <td>{{ radnik.email }}</td>
                  <td>
                    <button @click="confirmRemoveRadnik(radnik)" class="btn-delete-small">✕</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- <div class="actions">
          <button @click="confirmDelete" class="btn-danger">Obriši Rad</button>
        </div> -->
      </div>

      <!-- Add Radnik Modal -->
      <div v-if="showAddRadnikModal" class="modal-overlay" @click="closeAddRadnikModal">
        <div class="modal" @click.stop>
          <h3>Dodaj radnika</h3>
          <form @submit.prevent="addRadnik">
            <div class="form-group">
              <label>Radnik *</label>
              <select v-model="radnikForm.radnikIdzap" required>
                <option value="">Izaberite radnika</option>
                <option v-for="r in availableRadnici" :key="r.idzap" :value="r.idzap">
                  {{ r.ime }} {{ r.prez }}
                </option>
              </select>
            </div>
            <div class="modal-actions">
              <button type="button" @click="closeAddRadnikModal" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="addingRadnik">
                {{ addingRadnik ? 'Dodavanje...' : 'Dodaj' }}
              </button>
            </div>
            <div v-if="addRadnikError" class="error-message">{{ addRadnikError }}</div>
          </form>
        </div>
      </div>

      <!-- Edit Radovi Modal -->
      <div v-if="showEditRadoviModal" class="modal-overlay" @click="closeEditRadoviModal">
        <div class="modal" @click.stop>
          <h3>Uredi radove</h3>
          <form @submit.prevent="handleEditRadovi">
            <div class="form-group">
              <label for="edit-pocrad">Početak radova *</label>
              <input
                id="edit-pocrad"
                v-model="editRadoviForm.pocrad"
                type="date"
                required
              />
            </div>

            <div class="form-group">
              <label for="edit-zavrrad">Završetak radova *</label>
              <input
                id="edit-zavrrad"
                v-model="editRadoviForm.zavrrad"
                type="date"
                required
              />
            </div>

            <div class="form-group">
              <label for="edit-oprema">Oprema *</label>
              <input
                id="edit-oprema"
                v-model="editRadoviForm.oprema"
                type="text"
                required
                placeholder="npr. Makaze, lopate, traktor"
              />
            </div>

            <div class="modal-actions">
              <button type="button" @click="closeEditRadoviModal" class="btn-secondary">
                Odustani
              </button>
              <button type="submit" class="btn-primary" :disabled="editingRadovi">
                {{ editingRadovi ? 'Čuvam...' : 'Sačuvaj' }}
              </button>
            </div>

            <div v-if="editRadoviError" class="error-message">{{ editRadoviError }}</div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import radoviService from '@/services/radoviService'
import authService from '@/services/authService'
import type { RadoviDetail, RadnikNaRadu } from '@/types/radovi'
import type { Employee } from '@/types/auth'

const route = useRoute()
const router = useRouter()

const rad = ref<RadoviDetail | null>(null)
const loading = ref(false)
const error = ref('')

const showAddRadnikModal = ref(false)
const availableRadnici = ref<Employee[]>([])
const radnikForm = ref({ radnikIdzap: '' as number | '' })
const addingRadnik = ref(false)
const addRadnikError = ref('')

// Edit Radovi
const showEditRadoviModal = ref(false)
const editingRadovi = ref(false)
const editRadoviError = ref('')
const editRadoviForm = ref({
  pocrad: '',
  zavrrad: '',
  oprema: ''
})

const loadRad = async () => {
  try {
    loading.value = true
    error.value = ''
    const id = Number(route.params.id)
    rad.value = await radoviService.getRadoviById(id)
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška'
  } finally {
    loading.value = false
  }
}

const loadRadnici = async () => {
  try {
    const employees = await authService.getAllEmployees()
    availableRadnici.value = employees.filter(e => e.kategorija === 'Radnik')
  } catch (err) {
    console.error('Error loading radnici:', err)
  }
}

const addRadnik = async () => {
  if (!rad.value || radnikForm.value.radnikIdzap === '') return

  try {
    addingRadnik.value = true
    addRadnikError.value = ''
    await radoviService.addRadnikToRad(rad.value.idrad, { radnikIdzap: radnikForm.value.radnikIdzap as number })
    await loadRad()
    closeAddRadnikModal()
  } catch (err: any) {
    addRadnikError.value = err.response?.data?.message || 'Greška'
  } finally {
    addingRadnik.value = false
  }
}

const confirmRemoveRadnik = async (radnik: RadnikNaRadu) => {
  if (!confirm(`Ukloniti radnika ${radnik.ime} ${radnik.prezime}?`)) return
  
  try {
    await radoviService.removeRadnikFromRad(rad.value!.idrad, radnik.radnikIdzap)
    await loadRad()
  } catch (err: any) {
    alert(err.response?.data?.message || 'Greška')
  }
}

const confirmDelete = async () => {
  if (!confirm('Da li ste sigurni?')) return
  
  try {
    await radoviService.deleteRadovi(rad.value!.idrad)
    router.push('/radovi')
  } catch (err: any) {
    alert(err.response?.data?.message || 'Greška')
  }
}

const closeAddRadnikModal = () => {
  showAddRadnikModal.value = false
  radnikForm.value = { radnikIdzap: '' }
  addRadnikError.value = ''
}

// Edit Radovi functions
const openEditRadoviModal = () => {
  if (!rad.value) return
  
  editRadoviForm.value = {
    pocrad: rad.value.pocrad,
    zavrrad: rad.value.zavrrad,
    oprema: rad.value.oprema
  }
  showEditRadoviModal.value = true
}

const closeEditRadoviModal = () => {
  showEditRadoviModal.value = false
  editRadoviError.value = ''
}

const handleEditRadovi = async () => {
  if (!rad.value) return
  
  try {
    editingRadovi.value = true
    editRadoviError.value = ''
    
    await radoviService.updateRadovi(rad.value.idrad, editRadoviForm.value)
    
    // Osvježi podatke radova
    await loadRad()
    
    closeEditRadoviModal()
  } catch (err: any) {
    console.error('Error updating radovi:', err)
    editRadoviError.value = err.response?.data?.message || 'Greška pri ažuriranju radova'
  } finally {
    editingRadovi.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('sr-RS')
}

onMounted(async () => {
  await loadRad()
  await loadRadnici()
})
</script>

<style scoped>
.radovi-detail {
  width: 100%;
  min-height: calc(100vh - 64px);
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.loading, .error-message {
  text-align: center;
  padding: 60px;
}

.back-link {
  display: inline-block;
  color: #000;
  text-decoration: none;
  font-weight: 600;
  margin-bottom: 16px;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  margin: 0 0 40px 0;
}

.section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

h2 {
  font-size: 24px;
  font-weight: 600;
  margin: 0 0 20px 0;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.info-item {
  display: flex;
  justify-content: space-between;
}

.label {
  font-weight: 600;
  color: #666;
}

.value {
  font-weight: 500;
  color: #000;
}

.parcele-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.parcela-item {
  padding: 12px;
  background: #f9f9f9;
  border-radius: 6px;
}

.radnici-table table {
  width: 100%;
  border-collapse: collapse;
}

.radnici-table th {
  text-align: left;
  padding: 12px;
  background: #f9f9f9;
  font-weight: 600;
  border-bottom: 2px solid #e0e0e0;
}

.radnici-table td {
  padding: 12px;
  border-bottom: 1px solid #e0e0e0;
}

.btn-primary {
  padding: 12px 24px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
}

.btn-danger {
  padding: 12px 24px;
  background: #d32f2f;
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
}

.btn-delete-small {
  padding: 4px 8px;
  background: #d32f2f;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

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
}

.modal h3 {
  margin: 0 0 20px 0;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-weight: 600;
  margin-bottom: 8px;
}

.form-group select {
  width: 100%;
  padding: 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 16px;
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
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
}

.no-results {
  text-align: center;
  padding: 40px;
  background: #f9f9f9;
  border-radius: 8px;
  color: #666;
}

@media (max-width: 768px) {
  .radovi-detail {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }
}
</style>


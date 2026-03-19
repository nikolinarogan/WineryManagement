<template>
  <div class="raspored-detail">
    <div class="detail-container">
      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Raspored Details -->
      <div v-else-if="raspored" class="raspored-content">
        <!-- Header -->
        <div class="detail-header">
          <div class="header-top">
            <router-link :to="`/berbe/${raspored.seodrzavaIdber}`" class="back-link">
              ← Nazad na berbu
            </router-link>
          </div>
          <div class="header-content">
            <h1>Raspored branja</h1>
          </div>
        </div>

        <!-- Osnovni podaci -->
        <div class="section">
          <h2>Osnovni podaci</h2>
          <div class="info-grid">
            <div class="info-item">
              <label>Berba:</label>
              <p>{{ raspored.berbaNaziv }}</p>
            </div>
            <div class="info-item">
              <label>Parcela:</label>
              <p>{{ raspored.parcelaNaziv }} ({{ raspored.vinogradNaziv }})</p>
            </div>
            <div class="info-item">
              <label>Početak branja:</label>
              <p>{{ formatDate(raspored.pocbranja) }}</p>
            </div>
            <div class="info-item">
              <label>Završetak branja:</label>
              <p>{{ formatDate(raspored.zavrsetakbranja) }}</p>
            </div>
            <div class="info-item">
              <label>Očekivani prinos:</label>
              <p>{{ raspored.ocekivaniprinos }} kg</p>
            </div>
            <div class="info-item">
              <label>Menadžer:</label>
              <p>{{ raspored.menadzerIme }} {{ raspored.menadzerPrezime }}</p>
            </div>
          </div>
        </div>

        <!-- Radnici na rasporedu -->
        <div class="section">
          <div class="section-header">
            <h2>Radnici na rasporedu</h2>
            <button @click="showAddRadnikModal = true" class="btn-primary">
              + Dodaj radnika
            </button>
          </div>

          <div v-if="raspored.radnici.length === 0" class="no-results">
            <p>Nema dodijeljenih radnika za ovaj raspored.</p>
            <button @click="showAddRadnikModal = true" class="btn-primary">
              Dodaj prvog radnika
            </button>
          </div>

          <div v-else class="radnici-table">
            <table>
              <thead>
                <tr>
                  <th>Ime i prezime</th>
                  <th>Količina ubrano (kg)</th>
                  <th>Datum branja</th>
                  <th>Akcije</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="radnik in raspored.radnici" :key="radnik.radnikIdzap">
                  <td>{{ radnik.ime }} {{ radnik.prezime }}</td>
                  <td>{{ radnik.kolicinaubrgr }} kg</td>
                  <td>{{ formatDate(radnik.datumbranja) }}</td>
                  <td>
                    <button
                      @click="confirmRemoveRadnik(radnik)"
                      class="btn-delete-small"
                    >
                      Ukloni
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Actions -->
        <div class="actions">
          <button @click="confirmDeleteRaspored" class="btn-danger">
            Obriši raspored
          </button>
        </div>
      </div>
    </div>

    <!-- Add Radnik Modal -->
    <div v-if="showAddRadnikModal" class="modal-overlay" @click="closeAddRadnikModal">
      <div class="modal" @click.stop>
        <h3>Dodaj radnika na raspored</h3>

        <div v-if="loadingRadnici" class="loading">Učitavanje...</div>

        <form v-else @submit.prevent="addRadnik">
          <div class="form-group">
            <label for="radnik">Radnik *</label>
            <select id="radnik" v-model="radnikForm.radnikIdzap" required>
              <option value="">Izaberite radnika</option>
              <option v-for="radnik in availableRadnici" :key="radnik.idzap" :value="radnik.idzap">
                {{ radnik.ime }} {{ radnik.prez }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="datum">Datum branja *</label>
            <input
              id="datum"
              v-model="radnikForm.datumbranja"
              type="date"
              required
            />
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeAddRadnikModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="addingRadnik">
              {{ addingRadnik ? 'Dodavanje...' : 'Dodaj' }}
            </button>
          </div>

          <div v-if="addRadnikError" class="error-message">{{ addRadnikError }}</div>
        </form>
      </div>
    </div>

    <!-- Remove Radnik Confirmation -->
    <div v-if="radnikToRemove" class="modal-overlay" @click="cancelRemoveRadnik">
      <div class="modal" @click.stop>
        <h3>Ukloni radnika</h3>
        <p>
          Da li ste sigurni da želite ukloniti radnika
          <strong>{{ radnikToRemove.ime }} {{ radnikToRemove.prezime }}</strong> sa rasporeda?
        </p>
        <div class="modal-actions">
          <button @click="cancelRemoveRadnik" class="btn-secondary">Odustani</button>
          <button @click="removeRadnik" class="btn-danger">Ukloni</button>
        </div>
      </div>
    </div>

    <!-- Delete Raspored Confirmation -->
    <div v-if="showDeleteConfirmation" class="modal-overlay" @click="showDeleteConfirmation = false">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja</h3>
        <p>Da li ste sigurni da želite obrisati ovaj raspored branja?</p>
        <div class="modal-actions">
          <button @click="showDeleteConfirmation = false" class="btn-secondary">Odustani</button>
          <button @click="deleteRaspored" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import berbaService from '@/services/berbaService'
import authService from '@/services/authService'
import type { RasporedbranjaDetail, RadnikNaRaspored } from '@/types/berba'
import type { Employee } from '@/types/auth'

const route = useRoute()
const router = useRouter()

const raspored = ref<RasporedbranjaDetail | null>(null)
const loading = ref(false)
const error = ref('')

const showAddRadnikModal = ref(false)
const loadingRadnici = ref(false)
const allRadnici = ref<Employee[]>([])
const addingRadnik = ref(false)
const addRadnikError = ref('')
const radnikForm = ref({
  radnikIdzap: '' as number | '',
  kolicinaubrgr: 0,
  datumbranja: ''
})

const radnikToRemove = ref<RadnikNaRaspored | null>(null)
const showDeleteConfirmation = ref(false)

const availableRadnici = computed(() => {
  if (!raspored.value) return allRadnici.value

  const assignedRadniciIds = raspored.value.radnici.map(r => r.radnikIdzap)
  return allRadnici.value.filter(r => !assignedRadniciIds.includes(r.idzap))
})

const loadRaspored = async () => {
  try {
    loading.value = true
    error.value = ''
    const id = Number(route.params.id)
    raspored.value = await berbaService.getRasporedById(id)
  } catch (err: any) {
    console.error('Error loading raspored:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju rasporeda'
  } finally {
    loading.value = false
  }
}

const loadRadnici = async () => {
  try {
    loadingRadnici.value = true
    // Get all radnici (filter by Radnik category)
    allRadnici.value = await authService.getAllEmployees('Radnik')
  } catch (err) {
    console.error('Error loading radnici:', err)
  } finally {
    loadingRadnici.value = false
  }
}

const closeAddRadnikModal = () => {
  showAddRadnikModal.value = false
  radnikForm.value = {
    radnikIdzap: '',
    kolicinaubrgr: 0,
    datumbranja: ''
  }
  addRadnikError.value = ''
}

const addRadnik = async () => {
  if (!raspored.value || radnikForm.value.radnikIdzap === '') return

  try {
    addingRadnik.value = true
    addRadnikError.value = ''

    await berbaService.addRadnikToRaspored(raspored.value.idras, {
      radnikIdzap: radnikForm.value.radnikIdzap as number,
      kolicinaubrgr: radnikForm.value.kolicinaubrgr,
      datumbranja: radnikForm.value.datumbranja
    })

    await loadRaspored()
    closeAddRadnikModal()
  } catch (err: any) {
    console.error('Error adding radnik:', err)
    addRadnikError.value = err.response?.data?.message || 'Greška pri dodavanju radnika'
  } finally {
    addingRadnik.value = false
  }
}

const confirmRemoveRadnik = (radnik: RadnikNaRaspored) => {
  radnikToRemove.value = radnik
}

const cancelRemoveRadnik = () => {
  radnikToRemove.value = null
}

const removeRadnik = async () => {
  if (!raspored.value || !radnikToRemove.value) return

  try {
    await berbaService.removeRadnikFromRaspored(raspored.value.idras, radnikToRemove.value.radnikIdzap)
    await loadRaspored()
    radnikToRemove.value = null
  } catch (err: any) {
    console.error('Error removing radnik:', err)
    error.value = err.response?.data?.message || 'Greška pri uklanjanju radnika'
    radnikToRemove.value = null
  }
}

const confirmDeleteRaspored = () => {
  showDeleteConfirmation.value = true
}

const deleteRaspored = async () => {
  if (!raspored.value) return

  try {
    await berbaService.deleteRaspored(raspored.value.idras)
    router.push(`/berbe/${raspored.value.seodrzavaIdber}`)
  } catch (err: any) {
    console.error('Error deleting raspored:', err)
    error.value = err.response?.data?.message || 'Greška pri brisanju rasporeda'
    showDeleteConfirmation.value = false
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

onMounted(async () => {
  await loadRaspored()
  await loadRadnici()
})
</script>

<style scoped>
.raspored-detail {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.detail-container {
  max-width: 1200px;
  margin: 0 auto;
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
  margin-bottom: 20px;
}

.detail-header {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.header-top {
  margin-bottom: 20px;
}

.back-link {
  color: #000;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s;
}

.back-link:hover {
  color: #666;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.section h2 {
  font-size: 24px;
  font-weight: 600;
  color: #000;
  margin: 0 0 24px 0;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
}

.section-header h2 {
  margin: 0;
  padding: 0;
  border: none;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-item label {
  font-weight: 600;
  color: #666;
  font-size: 14px;
}

.info-item p {
  margin: 0;
  font-size: 16px;
  color: #000;
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

.no-results {
  text-align: center;
  padding: 40px;
  background: #f9f9f9;
  border-radius: 8px;
  border: 2px dashed #ccc;
}

.no-results p {
  margin: 0 0 16px 0;
  color: #666;
  font-size: 16px;
}

.radnici-table {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background: #f9f9f9;
}

th {
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: #000;
  border-bottom: 2px solid #e0e0e0;
}

td {
  padding: 12px;
  border-bottom: 1px solid #e0e0e0;
}

tbody tr:hover {
  background: #f9f9f9;
}

.btn-delete-small {
  padding: 6px 12px;
  background: transparent;
  color: #c00;
  border: 1px solid #c00;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-delete-small:hover {
  background: #c00;
  color: white;
}

.actions {
  display: flex;
  justify-content: flex-end;
}

.btn-danger {
  padding: 12px 24px;
  background: #c00;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-danger:hover {
  background: #a00;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(204, 0, 0, 0.3);
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
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.modal h3 {
  margin: 0 0 16px 0;
  font-size: 24px;
  color: #000;
}

.modal p {
  margin: 0 0 24px 0;
  color: #666;
  font-size: 16px;
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

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px;
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

@media (max-width: 768px) {
  .raspored-detail {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }

  table {
    font-size: 14px;
  }

  th, td {
    padding: 8px;
  }
}
</style>


<template>
  <div class="tretman-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="tretmanData" class="content">
      <div class="header">
        <button @click="goBack" class="btn-back">← Nazad</button>
        <div class="header-info">
          <h1>{{ tretmanData.naziv }}</h1>
          <span class="status-badge" :class="{ 'active': tretmanData.jeAktivan }">
            {{ tretmanData.jeAktivan ? 'Aktivan' : 'Završen' }}
          </span>
        </div>
      </div>

      <div class="cards-grid">
        <!-- Osnovne Informacije -->
        <div class="info-card">
          <h2>Osnovne Informacije</h2>
          <div class="info-content">
            <div class="info-row">
              <span class="label">Datum početka:</span>
              <span class="value">{{ formatDate(tretmanData.datpocetkatret) }}</span>
            </div>
            <div class="info-row">
              <span class="label">Datum završetka:</span>
              <span class="value">
                {{ tretmanData.datzavresetkatret ? formatDate(tretmanData.datzavresetkatret) : 'U toku' }}
              </span>
            </div>
            <div class="info-row">
              <span class="label">Trajanje:</span>
              <span class="value">{{ tretmanData.trajanjeUDanima }} dana</span>
            </div>
          </div>
        </div>

        <!-- Ubrana Sirovina -->
        <div class="info-card">
          <h2>Ubrana Sirovina</h2>
          <div class="info-content">
            <div class="info-row">
              <span class="label">Berba:</span>
              <span class="value">{{ tretmanData.ubranaSirovina.berbaNaziv }}</span>
            </div>
            <div class="info-row">
              <span class="label">Parcela:</span>
              <span class="value">{{ tretmanData.ubranaSirovina.parcelaNaziv }}</span>
            </div>
            <div class="info-row">
              <span class="label">Sorta:</span>
              <span class="value">{{ tretmanData.ubranaSirovina.sortaNaziv || 'Nije definisana' }}</span>
            </div>
            <div class="info-row">
              <span class="label">Količina:</span>
              <span class="value"><strong>{{ tretmanData.ubranaSirovina.kolicina }} kg</strong></span>
            </div>
          </div>
        </div>
      </div>

      <!-- Sirovine u Tretmanu -->
      <div class="sirovine-card">
        <div class="card-header">
          <h2>Sirovine u Tretmanu</h2>
          <button 
            v-if="tretmanData.jeAktivan"
            @click="showAddSirovinaModal = true"
            class="btn-add"
          >
            + Dodaj Sirovinu
          </button>
        </div>

        <div v-if="tretmanData.sirovine.length === 0" class="no-data">
          Nema dodatih sirovina
        </div>

        <table v-else class="sirovine-table">
          <thead>
            <tr>
              <th>Naziv Sirovine</th>
              <th>Količina</th>
              <th v-if="tretmanData.jeAktivan">Akcije</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="sirovina in tretmanData.sirovine" :key="sirovina.sirovinazatretmanIdsir">
              <td>{{ sirovina.nazivSirovine }}</td>
              <td><strong>{{ sirovina.kolicina }}</strong></td>
              <td v-if="tretmanData.jeAktivan">
                <button 
                  @click="removeSirovina(sirovina)"
                  class="btn-remove"
                >
                  Ukloni
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Akcije -->
      <div v-if="tretmanData.jeAktivan" class="actions-card">
        <button @click="closeTretman" class="btn-close-tretman">
          Zatvori Tretman
        </button>
      </div>
    </div>

    <!-- Modal za dodavanje sirovine -->
    <div v-if="showAddSirovinaModal" class="modal-overlay" @click="showAddSirovinaModal = false">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>Dodaj Sirovinu u Tretman</h2>
          <button @click="showAddSirovinaModal = false" class="btn-close">×</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleAddSirovina">
            <div class="form-group">
              <label for="sirovina">Sirovina *</label>
              <select id="sirovina" v-model.number="addSirovinaForm.sirovinazatretmanIdsir" required class="form-control">
                <option value="">Izaberite sirovinu</option>
                <option v-for="s in availableSirovine" :key="s.idsir" :value="s.idsir">
                  {{ s.naziv }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label for="kolicina">Količina *</label>
              <input
                type="number"
                id="kolicina"
                v-model.number="addSirovinaForm.kolicina"
                required
                step="0.01"
                min="0.01"
                placeholder="Unesite količinu"
                class="form-control"
              />
              <small class="form-hint">Unesite količinu sirovine koja se dodaje</small>
            </div>
            <div class="form-actions">
              <button type="button" @click="showAddSirovinaModal = false" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? 'Dodavanje...' : 'Dodaj Sirovinu' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Modal za zatvaranje tretmana -->
    <div v-if="showCloseTretmanModal" class="modal-overlay" @click="cancelCloseTretman">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>Zatvori Tretman</h2>
          <button @click="cancelCloseTretman" class="btn-close">×</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="confirmCloseTretman">
            <div class="form-group">
              <label for="datum-zavrsetka">Datum Završetka *</label>
              <input
                type="date"
                id="datum-zavrsetka"
                v-model="closeTretmanForm.datzavresetkatret"
                required
                :min="tretmanData?.datpocetkatret"
                :max="new Date().toISOString().split('T')[0]"
                class="form-control"
              />
              <small class="form-hint">Datum završetka ne može biti prije početka tretmana</small>
            </div>
            <div class="form-actions">
              <button type="button" @click="cancelCloseTretman" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="closingTretman">
                {{ closingTretman ? 'Zatvaranje...' : 'Zatvori Tretman' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import tretmanService from '@/services/tretmanService'
import sirovinazatretmanService from '@/services/sirovinazatretmanService'
import type { TretmanDetail } from '@/types/tretman'
import type { Sirovinazatretman } from '@/types/sirovinazatretman'

const route = useRoute()
const router = useRouter()
const tretmanId = parseInt(route.params.id as string)

const tretmanData = ref<TretmanDetail | null>(null)
const availableSirovine = ref<Sirovinazatretman[]>([])
const loading = ref(false)
const error = ref('')
const submitting = ref(false)
const showAddSirovinaModal = ref(false)
const showCloseTretmanModal = ref(false)
const closingTretman = ref(false)

const addSirovinaForm = ref({
  sirovinazatretmanIdsir: 0,
  kolicina: 0
})

const closeTretmanForm = ref({
  datzavresetkatret: new Date().toISOString().split('T')[0]
})

const loadTretman = async () => {
  loading.value = true
  error.value = ''
  try {
    tretmanData.value = await tretmanService.getTretmanDetail(tretmanId)
  } catch (err: any) {
    console.error('Greška pri učitavanju tretmana:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju tretmana'
  } finally {
    loading.value = false
  }
}

const loadSirovine = async () => {
  try {
    availableSirovine.value = await sirovinazatretmanService.getAllSirovine()
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovina:', err)
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const goBack = () => {
  router.back()
}

const handleAddSirovina = async () => {
  submitting.value = true
  try {
    await tretmanService.addSirovinaToTretman(tretmanId, {
      sirovinazatretmanIdsir: addSirovinaForm.value.sirovinazatretmanIdsir,
      kolicina: addSirovinaForm.value.kolicina
    })
    showAddSirovinaModal.value = false
    addSirovinaForm.value = { sirovinazatretmanIdsir: 0, kolicina: 0 }
    await loadTretman()
  } catch (err: any) {
    console.error('Greška pri dodavanju sirovine:', err)
    alert(err.response?.data?.message || 'Greška pri dodavanju sirovine')
  } finally {
    submitting.value = false
  }
}

const removeSirovina = async (sirovina: any) => {
  if (!confirm(`Da li ste sigurni da želite ukloniti "${sirovina.nazivSirovine}"?`)) {
    return
  }

  try {
    await tretmanService.removeSirovinaFromTretman(tretmanId, sirovina.sirovinazatretmanIdsir)
    alert('Sirovina uspješno uklonjena!')
    await loadTretman()
  } catch (err: any) {
    console.error('Greška pri uklanjanju sirovine:', err)
    alert(err.response?.data?.message || 'Greška pri uklanjanju sirovine')
  }
}

const closeTretman = () => {
  showCloseTretmanModal.value = true
  closeTretmanForm.value.datzavresetkatret = new Date().toISOString().split('T')[0]
}

const cancelCloseTretman = () => {
  showCloseTretmanModal.value = false
  closeTretmanForm.value.datzavresetkatret = new Date().toISOString().split('T')[0]
}

const confirmCloseTretman = async () => {
  closingTretman.value = true
  try {
    await tretmanService.closeTretman(tretmanId, {
      datzavresetkatret: closeTretmanForm.value.datzavresetkatret
    })
    showCloseTretmanModal.value = false
    await loadTretman()
  } catch (err: any) {
    console.error('Greška pri zatvaranju tretmana:', err)
    alert(err.response?.data?.message || 'Greška pri zatvaranju tretmana')
  } finally {
    closingTretman.value = false
  }
}

onMounted(() => {
  loadTretman()
  loadSirovine()
})
</script>

<style scoped>
.tretman-detail-container {
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
  margin-bottom: 30px;
}

.btn-back {
  background: none;
  border: none;
  font-size: 16px;
  color: #666;
  cursor: pointer;
  margin-bottom: 16px;
  padding: 8px 0;
}

.btn-back:hover {
  color: #000;
}

.header-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-info h1 {
  font-size: 28px;
  font-weight: 700;
  color: #000;
}

.status-badge {
  padding: 8px 16px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  background: #e0e0e0;
  color: #666;
}

.status-badge.active {
  background: #c8e6c9;
  color: #2e7d32;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.info-card,
.sirovine-card,
.actions-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  padding: 20px;
}

.info-card h2,
.sirovine-card h2 {
  font-size: 18px;
  font-weight: 700;
  color: #000;
  margin-bottom: 16px;
}

.info-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f5f5f5;
}

.label {
  color: #666;
  font-size: 14px;
}

.value {
  color: #000;
  font-size: 14px;
  font-weight: 500;
  text-align: right;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.btn-add {
  background: #000;
  color: #fff;
  padding: 8px 16px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.btn-add:hover {
  background: #333;
}

.no-data {
  text-align: center;
  color: #666;
  padding: 20px;
}

.sirovine-table {
  width: 100%;
  border-collapse: collapse;
}

.sirovine-table thead {
  background: #f5f5f5;
}

.sirovine-table th,
.sirovine-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.sirovine-table th {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 700;
}

.btn-remove {
  background: #ffebee;
  color: #c62828;
  padding: 4px 12px;
  border: none;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
}

.btn-remove:hover {
  background: #c62828;
  color: #fff;
}

.actions-card {
  display: flex;
  justify-content: flex-end;
}

.btn-close-tretman {
  background: #000;
  color: #fff;
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
}

.btn-close-tretman:hover {
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
  background: #fff;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.btn-close {
  background: none;
  border: none;
  font-size: 32px;
  color: #666;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-close:hover {
  color: #000;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
}

.form-control {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.3s ease;
}

.form-control:focus {
  outline: none;
  border-color: #000;
  box-shadow: 0 0 0 3px rgba(0, 0, 0, 0.1);
}

.form-hint {
  display: block;
  margin-top: 4px;
  font-size: 12px;
  color: #666;
  font-style: italic;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 20px;
}

.btn-secondary {
  background: #fff;
  color: #000;
  padding: 10px 20px;
  border: 2px solid #000;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-secondary:hover {
  background: #f5f5f5;
}

.btn-primary {
  background: #000;
  color: #fff;
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background: #333;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}
</style>


<template>
  <div class="ubrane-sirovine-container">
    <div class="header">
      <h1>Ubrane sirovine za tretman</h1>
      <p class="subtitle">Pregled ubranih sirovina i njihov status u procesu tretmana</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="sirovine.length === 0" class="no-data">
      <p>Trenutno nema ubranih sirovina.</p>
    </div>

    <div v-else class="content">
      <div class="sirovine-grid">
        <div 
          v-for="sirovina in sirovine" 
          :key="sirovina.idubrsir"
          class="sirovina-card"
        >
          <div class="card-header">
            <div class="header-info">
              <h3>{{ sirovina.berbaNaziv }} ({{ sirovina.sezona }})</h3>
              <span class="status-badge" :class="`status-${sirovina.status.toLowerCase()}`">
                {{ getStatusLabel(sirovina.status) }}
              </span>
            </div>
          </div>

          <div class="card-body">
            <div class="info-row">
              <span class="label">Vinograd:</span>
              <span class="value">{{ sirovina.vinogradNaziv }}</span>
            </div>
            <div class="info-row">
              <span class="label">Parcela:</span>
              <span class="value">{{ sirovina.parcelaNaziv }}</span>
            </div>
            <div class="info-row">
              <span class="label">Sorta:</span>
              <span class="value">{{ sirovina.sortaNaziv || 'Nije definisana' }}</span>
            </div>
            <div class="info-row">
              <span class="label">Količina:</span>
              <span class="value"><strong>{{ sirovina.kolicina }} kg</strong></span>
            </div>
            <div class="info-row">
              <span class="label">Datum prijema:</span>
              <span class="value">{{ formatDate(sirovina.datprijema) }}</span>
            </div>
            <div class="info-row">
              <span class="label">Tretmani:</span>
              <span class="value">{{ sirovina.brojTretmana }} ({{ sirovina.aktivniTretmani }} aktivnih)</span>
            </div>
          </div>

          <div class="card-actions">
            <router-link 
              :to="`/ubrane-sirovine/${sirovina.idubrsir}/tretmani`" 
              class="btn-view"
            >
              Vidi tretmane ({{ sirovina.brojTretmana }})
            </router-link>
            <button 
              v-if="sirovina.status === 'Nova' || sirovina.status === 'SpremaZaVino'"
              @click="openCreateTretmanModal(sirovina)"
              class="btn-create"
            >
              {{ sirovina.status === 'Nova' ? 'Prvi Tretman' : 'Novi tretman' }}
            </button>
            <button 
              v-if="sirovina.status === 'SpremaZaVino'"
              @click="openCreateSirovovinoModal(sirovina)"
              class="btn-primary"
            >
              Kreiraj sirovo vino
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal za kreiranje tretmana -->
    <div v-if="showCreateTretmanModal" class="modal-overlay" @click="closeModals">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>Kreiraj novi tretman</h2>
          <button @click="closeModals" class="btn-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="selectedSirovina" class="modal-info">
            <p><strong>Berba:</strong> {{ selectedSirovina.berbaNaziv }} ({{ selectedSirovina.sezona }})</p>
            <p><strong>Parcela:</strong> {{ selectedSirovina.parcelaNaziv }}</p>
            <p><strong>Količina:</strong> {{ selectedSirovina.kolicina }} kg</p>
          </div>
          <form @submit.prevent="handleCreateTretman">
            <div class="form-group">
              <label for="naziv">Naziv tretmana *</label>
              <input
                type="text"
                id="naziv"
                v-model="tretmanForm.naziv"
                required
                maxlength="200"
                placeholder="npr. Alkoholna fermentacija, Malolaktična fermentacija..."
                class="form-control"
              />
            </div>
            <div class="form-group">
              <label for="datum">Datum početka *</label>
              <input
                type="date"
                id="datum"
                v-model="tretmanForm.datpocetkatret"
                required
                :max="todayDate"
                class="form-control"
              />
            </div>
            <div class="form-actions">
              <button type="button" @click="closeModals" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? 'Kreiranje...' : 'Kreiraj tretman' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Modal za kreiranje sirovog vina -->
    <div v-if="showCreateSirovovinoModal" class="modal-overlay" @click="closeModals">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>Kreiraj sirovo vino</h2>
          <button @click="closeModals" class="btn-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="selectedSirovina" class="modal-info">
            <p><strong>Berba:</strong> {{ selectedSirovina.berbaNaziv }} ({{ selectedSirovina.sezona }})</p>
            <p><strong>Sorta:</strong> {{ selectedSirovina.sortaNaziv || 'Nije definisana' }}</p>
            <p><strong>Tretmani:</strong> {{ selectedSirovina.brojTretmana }} (svi završeni)</p>
          </div>
          <form @submit.prevent="handleCreateSirovovino">
            <div class="form-group">
              <label for="nazivsirvina">Naziv sirovog vina *</label>
              <input
                type="text"
                id="nazivsirvina"
                v-model="sirovovinoForm.nazivsirvina"
                required
                maxlength="100"
                placeholder="npr. Merlot Sirovo 2024"
                class="form-control"
              />
            </div>
            <div class="form-group">
              <label for="kolicina-grozdja">Količina grožđa (kg) *</label>
              <input
                type="number"
                id="kolicina-grozdja"
                v-model.number="sirovovinoForm.kolicinaGrozdja"
                required
                step="0.01"
                min="0.01"
                :max="selectedSirovina.preostalaKolicina"
                placeholder="Unesite količinu grožđa u kg"
                class="form-control"
              />
              <small class="form-hint">
                Preostalo grožđa: <strong>{{ selectedSirovina.preostalaKolicina }} kg</strong>
                (od ukupno {{ selectedSirovina.kolicina }} kg)
              </small>
            </div>
            <div class="form-group">
              <label for="kolicina">Količina vina (litri) *</label>
              <input
                type="number"
                id="kolicina"
                v-model.number="sirovovinoForm.kolicinasirvina"
                required
                step="0.01"
                min="0.01"
                placeholder="Unesite količinu u litrima"
                class="form-control"
              />
              <small class="form-hint">
                Preporuka: 65-75% od količine grožđa 
                ({{ Math.round(sirovovinoForm.kolicinaGrozdja * 0.65) }}-{{ Math.round(sirovovinoForm.kolicinaGrozdja * 0.75) }} L)
              </small>
            </div>
            <div class="form-group">
              <label for="kvalitet">Kvalitet *</label>
              <select id="kvalitet" v-model="sirovovinoForm.kvalitet" required class="form-control">
                <option value="">Izaberite kvalitet</option>
                <option value="Premium">Premium</option>
                <option value="Standard">Standard</option>
                <option value="Osnovno">Osnovno</option>
              </select>
            </div>
            <div class="form-group">
              <label for="godina">Godina proizvodnje *</label>
              <input
                type="number"
                id="godina"
                v-model.number="sirovovinoForm.godproizvodnje"
                required
                :min="2000"
                :max="currentYear"
                class="form-control"
              />
            </div>
            <div class="form-actions">
              <button type="button" @click="closeModals" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? 'Kreiranje...' : 'Kreiraj sirovo vino' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import tretmanService from '@/services/tretmanService'
import sirovovinoService from '@/services/sirovovinoService'
import type { UbranasirovinaZaTretman } from '@/types/tretman'

const sirovine = ref<UbranasirovinaZaTretman[]>([])
const loading = ref(false)
const error = ref('')
const submitting = ref(false)

const showCreateTretmanModal = ref(false)
const showCreateSirovovinoModal = ref(false)
const selectedSirovina = ref<UbranasirovinaZaTretman | null>(null)

const tretmanForm = ref({
  naziv: '',
  datpocetkatret: new Date().toISOString().split('T')[0]
})

const sirovovinoForm = ref({
  nazivsirvina: '',
  kolicinasirvina: 0,
  kolicinaGrozdja: 0,  // NOVO: Količina grožđa koju koristimo (kg)
  kvalitet: '',
  godproizvodnje: new Date().getFullYear()
})

const todayDate = computed(() => new Date().toISOString().split('T')[0])
const currentYear = computed(() => new Date().getFullYear())

const loadSirovine = async () => {
  loading.value = true
  error.value = ''
  try {
    sirovine.value = await tretmanService.getAllUbraneSirovine()
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju sirovina'
  } finally {
    loading.value = false
  }
}

const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    'Nova': 'Nova',
    'UTretmanu': 'U tretmanu',
    'SpremaZaVino': 'Spremna za kreiranje vina',
    'Obradjena': 'Obrađena'
  }
  return labels[status] || status
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const openCreateTretmanModal = (sirovina: UbranasirovinaZaTretman) => {
  selectedSirovina.value = sirovina
  tretmanForm.value.naziv = ''
  tretmanForm.value.datpocetkatret = new Date().toISOString().split('T')[0]
  showCreateTretmanModal.value = true
}

const openCreateSirovovinoModal = (sirovina: UbranasirovinaZaTretman) => {
  selectedSirovina.value = sirovina
  sirovovinoForm.value.nazivsirvina = `${sirovina.sortaNaziv || 'Mix'} Sirovo ${sirovina.sezona}`
  sirovovinoForm.value.kolicinaGrozdja = sirovina.preostalaKolicina // Default: svo preostalo grožđe
  sirovovinoForm.value.kolicinasirvina = Math.round(sirovina.preostalaKolicina * 0.7) // Default 70% od grožđa
  sirovovinoForm.value.kvalitet = ''
  sirovovinoForm.value.godproizvodnje = sirovina.sezona
  showCreateSirovovinoModal.value = true
}

const handleCreateTretman = async () => {
  if (!selectedSirovina.value) return
  
  submitting.value = true
  try {
    await tretmanService.createTretman({
      naziv: tretmanForm.value.naziv,
      datpocetkatret: tretmanForm.value.datpocetkatret,
      ubranasirovinaIdubrsir: selectedSirovina.value.idubrsir
    })
    closeModals()
    await loadSirovine()
  } catch (err: any) {
    console.error('Greška pri kreiranju tretmana:', err)
    alert(err.response?.data?.message || 'Greška pri kreiranju tretmana')
  } finally {
    submitting.value = false
  }
}

const handleCreateSirovovino = async () => {
  if (!selectedSirovina.value) return
  
  submitting.value = true
  try {
    await sirovovinoService.createSirovovino({
      nazivsirvina: sirovovinoForm.value.nazivsirvina,
      kolicinasirvina: sirovovinoForm.value.kolicinasirvina,
      kvalitet: sirovovinoForm.value.kvalitet,
      godproizvodnje: sirovovinoForm.value.godproizvodnje,
      ubraneSirovine: [  // NOVI FORMAT: Lista sa količinom grožđa
        {
          ubranasirovinaId: selectedSirovina.value.idubrsir,
          kolicinaGrozdja: sirovovinoForm.value.kolicinaGrozdja
        }
      ]
    })
    closeModals()
    await loadSirovine()
  } catch (err: any) {
    console.error('Greška pri kreiranju sirovog vina:', err)
    alert(err.response?.data?.message || 'Greška pri kreiranju sirovog vina')
  } finally {
    submitting.value = false
  }
}

const closeModals = () => {
  showCreateTretmanModal.value = false
  showCreateSirovovinoModal.value = false
  selectedSirovina.value = null
}

onMounted(() => {
  loadSirovine()
})
</script>

<style scoped>
.ubrane-sirovine-container {
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

.sirovine-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.sirovina-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.sirovina-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.card-header {
  padding: 20px;
  background: #f5f5f5;
  border-bottom: 1px solid #e0e0e0;
}

.header-info {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 12px;
}

.header-info h3 {
  font-size: 18px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}

.status-nova {
  background: #e3f2fd;
  color: #1976d2;
}

.status-utretmanu {
  background: #fff3e0;
  color: #f57c00;
}

.status-spremazavino {
  background: #c8e6c9;
  color: #2e7d32;
}

.status-obradjena {
  background: #e0e0e0;
  color: #666;
}

.card-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f5f5f5;
}

.info-row:last-child {
  border-bottom: none;
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

.card-actions {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.btn-view,
.btn-create,
.btn-primary {
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
  flex: 1;
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

.btn-primary {
  background: #000;
  color: #fff;
}

.btn-primary:hover {
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

.modal-info {
  background: #f5f5f5;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.modal-info p {
  margin: 4px 0;
  font-size: 14px;
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
</style>


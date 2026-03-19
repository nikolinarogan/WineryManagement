<template>
  <div class="sirovo-vino-detail-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="vino" class="vino-content">
      <div class="header">
        <router-link to="/sirova-vina" class="back-link">← Nazad na sirova vina</router-link>
        <h1>{{ vino.nazivsirvina }}</h1>
      </div>

      <!-- Osnovne informacije -->
      <div class="section">
        <h2>Osnovne Informacije</h2>
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Količina:</span>
            <span class="value"><strong>{{ vino.kolicinasirvina }} L</strong></span>
          </div>
          <div class="info-item">
            <span class="label">Kvalitet:</span>
            <span class="value">
              <span class="kvalitet-badge">{{ vino.kvalitet }}</span>
            </span>
          </div>
          <div class="info-item">
            <span class="label">Godina proizvodnje:</span>
            <span class="value"><strong>{{ vino.godproizvodnje }}</strong></span>
          </div>
        </div>

        <div class="poreklo-section">
          <h3>Poreklo:</h3>
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

      <!-- Lagerovanja Section -->
      <div class="section">
        <div class="section-header">
          <h2>Istorija lagerovanja</h2>
          <button @click="openAddLagerovanjeModal" class="btn-primary">+ Dodaj lagerovanje</button>
        </div>

        <div v-if="loadingLagerovanja" class="loading-text">Učitavanje...</div>

        <div v-else-if="lagerovanja.length === 0" class="no-data">
          <p>Ovo vino još nije bilo lagerovano.</p>
        </div>

        <div v-else class="lagerovanja-list">
          <div 
            v-for="lagerovanje in lagerovanja" 
            :key="`${lagerovanje.sirovovinoIdsirvina}-${lagerovanje.bureIdbur}`"
            class="lagerovanje-card"
            :class="{ 'aktivno': lagerovanje.jeAktivno }"
          >
            <div class="lagerovanje-header">
              <h3>{{ lagerovanje.oznakaBureta }}</h3>
              <span v-if="lagerovanje.jeAktivno" class="status-badge aktivno">Aktivno</span>
              <span v-else class="status-badge zavrseno">Završeno</span>
            </div>

            <div class="lagerovanje-info">
              <div class="info-row">
                <span class="label">Podrum:</span>
                <span class="value">{{ lagerovanje.nazivPodruma }}</span>
              </div>
              <div class="info-row">
                <span class="label">Materijal:</span>
                <span class="value">{{ lagerovanje.materijalBureta }}</span>
              </div>
              <div class="info-row">
                <span class="label">Zapremina:</span>
                <span class="value">{{ lagerovanje.zapreminaBureta }} L</span>
              </div>
              <div class="info-row">
                <span class="label">Datum punjenja:</span>
                <span class="value">{{ formatDate(lagerovanje.datpunjenja) }}</span>
              </div>
              <div class="info-row">
                <span class="label">Datum pražnjenja:</span>
                <span class="value">
                  {{ lagerovanje.jeAktivno ? '-' : formatDate(lagerovanje.datpraznjenja) }}
                </span>
              </div>
              <div class="info-row">
                <span class="label">Trajanje:</span>
                <span class="value"><strong>{{ lagerovanje.brojDana }} dana</strong></span>
              </div>
            </div>

            <div v-if="lagerovanje.jeAktivno" class="lagerovanje-actions">
              <button @click="openEndLagerovanjeModal(lagerovanje)" class="btn-end">
                Završi lagerovanje
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Lagerovanje Modal -->
    <div v-if="showAddModal" class="modal-overlay" @click="closeAddModal">
      <div class="modal" @click.stop>
        <h3>Dodaj lagerovanje</h3>
        <form @submit.prevent="handleAddLagerovanje">
          <div class="form-group">
            <label for="add-bure">Bure *</label>
            <select id="add-bure" v-model.number="addForm.bureIdbur" required>
              <option value="">Izaberite bure</option>
              <option 
                v-for="bure in dostupnaBuradi" 
                :key="bure.idbur" 
                :value="bure.idbur"
                :disabled="bure.jeZauzeto"
              >
                {{ bure.oznakabur }} - {{ bure.nazivPodruma }} 
                ({{ bure.materijal }}, {{ bure.zapremina }}L)
                {{ bure.jeZauzeto ? ' [Zauzeto]' : '' }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="add-datpunjenja">Datum punjenja *</label>
            <input
              id="add-datpunjenja"
              v-model="addForm.datpunjenja"
              type="date"
              required
              :max="today"
            />
            <small>Datum punjenja ne može biti u budućnosti</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeAddModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="adding">
              {{ adding ? 'Dodajem...' : 'Dodaj' }}
            </button>
          </div>

          <div v-if="addError" class="error-message">{{ addError }}</div>
        </form>
      </div>
    </div>

    <!-- End Lagerovanje Modal -->
    <div v-if="showEndModal" class="modal-overlay" @click="closeEndModal">
      <div class="modal" @click.stop>
        <h3>Završi lagerovanje</h3>
        <p>Unesite datum pražnjenja bureta</p>
        <form @submit.prevent="handleEndLagerovanje">
          <div class="form-group">
            <label for="end-datpraznjenja">Datum Pražnjenja *</label>
            <input
              id="end-datpraznjenja"
              v-model="endForm.datpraznjenja"
              type="date"
              required
              :max="today"
              :min="selectedLagerovanje?.datpunjenja"
            />
            <small>Datum pražnjenja mora biti nakon datuma punjenja i ne može biti u budućnosti</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEndModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="ending">
              {{ ending ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="endError" class="error-message">{{ endError }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import sirovovinoService from '@/services/sirovovinoService'
import lagerovanjeService from '@/services/lagerovanjeService'
import type { Sirovovino } from '@/types/sirovovino'
import type { LagerovanoVino, DostupnoBure } from '@/types/lagerovanje'

const route = useRoute()
const vinoId = parseInt(route.params.id as string)

console.log('SirovoVinoDetailView - vinoId:', vinoId)
console.log('Route params:', route.params)

const vino = ref<Sirovovino | null>(null)
const lagerovanja = ref<LagerovanoVino[]>([])
const dostupnaBuradi = ref<DostupnoBure[]>([])

const loading = ref(false)
const error = ref('')
const loadingLagerovanja = ref(false)

// Add Modal
const showAddModal = ref(false)
const adding = ref(false)
const addError = ref('')
const addForm = ref({
  bureIdbur: 0,
  datpunjenja: ''
})

// End Modal
const showEndModal = ref(false)
const ending = ref(false)
const endError = ref('')
const selectedLagerovanje = ref<LagerovanoVino | null>(null)
const endForm = ref({
  datpraznjenja: ''
})

const today = computed(() => {
  const date = new Date()
  return date.toISOString().split('T')[0]
})

const loadVino = async () => {
  loading.value = true
  error.value = ''
  try {
    vino.value = await sirovovinoService.getSirovoVinoById(vinoId)
    console.log('Sirovo vino učitano:', vino.value)
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovog vina:', err)
    console.error('Response:', err.response)
    console.error('Status:', err.response?.status)
    console.error('Data:', err.response?.data)
    
    if (err.response?.status === 404) {
      error.value = 'Sirovo vino nije pronađeno'
    } else if (err.response?.status === 401 || err.response?.status === 403) {
      error.value = 'Nemate pristup ovom resursu'
    } else {
      error.value = err.response?.data?.message || err.message || 'Greška pri učitavanju sirovog vina'
    }
  } finally {
    loading.value = false
  }
}

const loadLagerovanja = async () => {
  loadingLagerovanja.value = true
  try {
    lagerovanja.value = await lagerovanjeService.getLagerovanjaZaSirovoVino(vinoId)
  } catch (err: any) {
    console.error('Greška pri učitavanju lagerovanja:', err)
  } finally {
    loadingLagerovanja.value = false
  }
}

const loadDostupnaBuradi = async () => {
  try {
    dostupnaBuradi.value = await lagerovanjeService.getDostupnaBurad()
  } catch (err: any) {
    console.error('Greška pri učitavanju buradi:', err)
  }
}

const openAddLagerovanjeModal = async () => {
  await loadDostupnaBuradi()
  addForm.value = {
    bureIdbur: 0,
    datpunjenja: today.value
  }
  showAddModal.value = true
}

const closeAddModal = () => {
  showAddModal.value = false
  addError.value = ''
}

const handleAddLagerovanje = async () => {
  if (!vino.value) return

  try {
    adding.value = true
    addError.value = ''

    await lagerovanjeService.startLagerovanje({
      sirovovinoIdsirvina: vino.value.idsirvina,
      bureIdbur: addForm.value.bureIdbur,
      datpunjenja: addForm.value.datpunjenja
    })

    await loadLagerovanja()
    closeAddModal()
  } catch (err: any) {
    console.error('Greška pri dodavanju lagerovanja:', err)
    addError.value = err.response?.data?.message || 'Greška pri dodavanju lagerovanja'
  } finally {
    adding.value = false
  }
}

const openEndLagerovanjeModal = (lagerovanje: LagerovanoVino) => {
  selectedLagerovanje.value = lagerovanje
  endForm.value = {
    datpraznjenja: today.value
  }
  showEndModal.value = true
}

const closeEndModal = () => {
  showEndModal.value = false
  endError.value = ''
  selectedLagerovanje.value = null
}

const handleEndLagerovanje = async () => {
  if (!selectedLagerovanje.value) return

  try {
    ending.value = true
    endError.value = ''

    await lagerovanjeService.updateLagerovanje(
      selectedLagerovanje.value.sirovovinoIdsirvina,
      selectedLagerovanje.value.bureIdbur,
      { datpraznjenja: endForm.value.datpraznjenja }
    )

    await loadLagerovanja()
    closeEndModal()
  } catch (err: any) {
    console.error('Greška pri završavanju lagerovanja:', err)
    endError.value = err.response?.data?.message || 'Greška pri završavanju lagerovanja'
  } finally {
    ending.value = false
  }
}

const formatDate = (dateString: string) => {
  if (!dateString || dateString === '9999-12-31') return '-'
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

onMounted(async () => {
  await loadVino()
  await loadLagerovanja()
})
</script>

<style scoped>
.sirovo-vino-detail-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.loading,
.loading-text,
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
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 20px;
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

.kvalitet-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 700;
  background: #000;
  color: #fff;
}

.poreklo-section {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 2px solid #f0f0f0;
}

.poreklo-section h3 {
  font-size: 16px;
  font-weight: 600;
  color: #000;
  margin-bottom: 12px;
}

.poreklo-item {
  padding: 12px;
  background: #f9f9f9;
  border-radius: 8px;
  margin-bottom: 8px;
}

.poreklo-info {
  font-size: 14px;
  color: #000;
  margin-bottom: 6px;
}

.poreklo-details {
  display: flex;
  gap: 12px;
  font-size: 12px;
}

.sorta {
  color: #666;
}

.tretmani {
  color: #999;
}

/* Lagerovanja */
.lagerovanja-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 16px;
}

.lagerovanje-card {
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  padding: 16px;
  transition: all 0.3s ease;
}

.lagerovanje-card.aktivno {
  border-color: #4CAF50;
  background: rgba(76, 175, 80, 0.05);
}

.lagerovanje-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.lagerovanje-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.lagerovanje-header h3 {
  font-size: 18px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.status-badge {
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
}

.status-badge.aktivno {
  background: #4CAF50;
  color: #fff;
}

.status-badge.zavrseno {
  background: #9E9E9E;
  color: #fff;
}

.lagerovanje-info {
  margin-bottom: 12px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 6px 0;
  border-bottom: 1px solid #f0f0f0;
  font-size: 13px;
}

.info-row:last-child {
  border-bottom: none;
}

.info-row .label {
  color: #666;
}

.info-row .value {
  color: #000;
  font-weight: 500;
}

.lagerovanje-actions {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 2px solid #f0f0f0;
}

.btn-end {
  width: 100%;
  padding: 10px;
  background: #FF9800;
  color: #fff;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-end:hover {
  background: #F57C00;
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
  margin: 0 0 16px 0;
  font-size: 24px;
  color: #000;
}

.modal p {
  margin-bottom: 20px;
  color: #666;
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

.no-data {
  text-align: center;
  padding: 20px;
  color: #666;
}
</style>


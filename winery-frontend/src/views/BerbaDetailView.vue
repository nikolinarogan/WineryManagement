<template>
  <div class="berba-detail">
    <div class="detail-container">
      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Berba Details -->
      <div v-else-if="berba" class="berba-content">
        <!-- Header -->
        <div class="detail-header">
          <div class="header-top">
            <router-link to="/berbe" class="back-link">← Nazad na berbe</router-link>
          </div>
          <div class="header-content">
            <div class="title-section">
              <h1>{{ berba.nazber }}</h1>
              <span class="sezona-badge">{{ berba.sezona }}</span>
            </div>
          </div>
        </div>

        <!-- Osnovni Podaci -->
        <div class="section">
          <div class="section-header">
            <h2>Osnovni podaci</h2>
            <button @click="openEditBerbaModal" class="btn-primary">
              ✎ Uredi berbu
            </button>
          </div>
          <div class="info-grid">
            <div class="info-item">
              <label>Naziv berbe</label>
              <p>{{ berba.nazber }}</p>
            </div>
            <div class="info-item">
              <label>Sezona</label>
              <p>{{ berba.sezona }}</p>
            </div>
            <div class="info-item">
              <label>Broj parcela</label>
              <p>{{ berba.parcele.length }}</p>
            </div>
          </div>
        </div>

        <!-- Parcele u berbi -->
        <div class="section">
          <div class="section-header">
            <h2>Parcele u berbi</h2>
            <button @click="showAddParcelaModal = true" class="btn-primary">
              + Dodaj parcelu
            </button>
          </div>

          <div v-if="berba.parcele.length === 0" class="no-results">
            <p>Nema dodijeljenih parcela za ovu berbu.</p>
            <button @click="showAddParcelaModal = true" class="btn-primary">
              Dodaj prvu parcelu
            </button>
          </div>

          <div v-else class="parcele-grid">
            <div
              v-for="parcela in berba.parcele"
              :key="parcela.parcelaIdp"
              class="parcela-card"
            >
              <div class="parcela-header">
                <h3>{{ parcela.nazivparcele }}</h3>
                <button @click="confirmRemoveParcela(parcela)" class="btn-delete-small">
                  ✕
                </button>
              </div>
              <div class="parcela-info">
                <div class="info-row">
                  <span class="label">Vinograd:</span>
                  <span class="value">{{ parcela.vinogradNaziv }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Površina:</span>
                  <span class="value">{{ parcela.povrsina }} ha</span>
                </div>
                <div class="info-row">
                  <span class="label">Broj čokota:</span>
                  <span class="value">{{ parcela.brojcokota }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Sorta grožđa:</span>
                  <span class="value" :class="{ 'sorta-badge': parcela.sortaNaziv }">
                    {{ parcela.sortaNaziv || 'Nije dodijeljeno' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Rasporedi Branja -->
        <div class="section">
          <div class="section-header">
            <h2>Rasporedi branja</h2>
            <button @click="showCreateRasporedModal = true" class="btn-primary">
              + Kreiraj raspored
            </button>
          </div>

          <div v-if="rasporedi.length === 0" class="no-results">
            <p>Nema kreiranih rasporeda branja za ovu berbu.</p>
            <button @click="showCreateRasporedModal = true" class="btn-primary">
              Kreiraj prvi raspored
            </button>
          </div>

          <div v-else class="rasporedi-grid">
            <div
              v-for="raspored in rasporedi"
              :key="raspored.idras"
              class="raspored-card"
              @click="navigateToRaspored(raspored.idras)"
            >
              <div class="raspored-header">
                <h3>{{ raspored.parcelaNaziv }}</h3>
                <span class="radnici-badge">{{ raspored.brojRadnika }} radnika</span>
              </div>
              <div class="raspored-info">
                <div class="info-row">
                  <span class="label">Početak:</span>
                  <span class="value">{{ formatDate(raspored.pocbranja) }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Završetak:</span>
                  <span class="value">{{ formatDate(raspored.zavrsetakbranja) }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Očekivani prinos:</span>
                  <span class="value">{{ raspored.ocekivaniprinos }} kg</span>
                </div>
                <div class="info-row">
                  <span class="label">Menadžer:</span>
                  <span class="value">{{ raspored.menadzerIme }} {{ raspored.menadzerPrezime }}</span>
                </div>
                
                <!-- Uvid u unos radnika -->
                <div class="info-row progress-row">
                  <span class="label">Radnici sa unosom:</span>
                  <span class="value">
                    <span :class="['progress-text', getProgressClass(raspored)]">
                      {{ raspored.brojRadnikaSaUnosom }} / {{ raspored.brojRadnika }}
                    </span>
                  </span>
                </div>
                <div class="info-row">
                  <span class="label">Ukupno ubrano:</span>
                  <span class="value">
                    <strong>{{ raspored.ukupnoUbrano.toFixed(2) }} kg</strong>
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="actions">
          <router-link :to="`/berbe/${berba.idber}/statistika`" class="btn-stats">
            📊 Statistika i učinak
          </router-link>
          <button @click="confirmDeleteBerba" class="btn-danger">
            Obriši berbu
          </button>
        </div>
      </div>
    </div>

    <!-- Add Parcela Modal -->
    <div v-if="showAddParcelaModal" class="modal-overlay" @click="closeAddParcelaModal">
      <div class="modal" @click.stop>
        <h3>Dodaj parcelu u berbu</h3>
        
        <div v-if="loadingVinogradi" class="loading">Učitavanje...</div>
        
        <div v-else>
          <div class="form-group">
            <label>Izaberite parcelu:</label>
            <div class="vinogradi-select">
              <div v-for="vinograd in availableVinogradi" :key="vinograd.idv" class="vinograd-group">
                <h4>{{ vinograd.naziv }}</h4>
                <div class="parcele-list">
                  <label
                    v-for="parcela in vinograd.parcele"
                    :key="parcela.idp"
                    class="parcela-option"
                  >
                    <input
                      type="radio"
                      name="parcela"
                      :value="parcela.idp"
                      v-model="selectedParcelaId"
                    />
                    <div class="parcela-info-compact">
                      <strong>{{ parcela.nazivparcele }}</strong>
                      <span>{{ parcela.povrsina }} ha • {{ parcela.brojcokota }} čokota</span>
                      <span v-if="parcela.sortaNaziv" class="sorta-tag">{{ parcela.sortaNaziv }}</span>
                    </div>
                  </label>
                </div>
              </div>
            </div>
          </div>

          <div v-if="availableVinogradi.length === 0" class="no-results">
            <p>Sve parcele su već dodijeljene ovoj berbi.</p>
          </div>
        </div>

        <div class="modal-actions">
          <button @click="closeAddParcelaModal" class="btn-secondary">Odustani</button>
          <button
            @click="addParcela"
            class="btn-primary"
            :disabled="!selectedParcelaId || addingParcela"
          >
            {{ addingParcela ? 'Dodavanje...' : 'Dodaj' }}
          </button>
        </div>

        <div v-if="addParcelaError" class="error-message">{{ addParcelaError }}</div>
      </div>
    </div>

    <!-- Remove Parcela Confirmation -->
    <div v-if="parcelaToRemove" class="modal-overlay" @click="cancelRemoveParcela">
      <div class="modal" @click.stop>
        <h3>Ukloni parcelu</h3>
        <p>Da li ste sigurni da želite ukloniti parcelu <strong>{{ parcelaToRemove.nazivparcele }}</strong> iz berbe?</p>
        <div class="modal-actions">
          <button @click="cancelRemoveParcela" class="btn-secondary">Odustani</button>
          <button @click="removeParcela" class="btn-danger">Ukloni</button>
        </div>
      </div>
    </div>

    <!-- Delete Berba Confirmation -->
    <div v-if="showDeleteConfirmation" class="modal-overlay" @click="showDeleteConfirmation = false">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja</h3>
        <p>Da li ste sigurni da želite obrisati berbu <strong>{{ berba?.nazber }}</strong>?</p>
        <div class="modal-actions">
          <button @click="showDeleteConfirmation = false" class="btn-secondary">Odustani</button>
          <button @click="deleteBerba" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>

    <!-- Create Raspored Modal -->
    <div v-if="showCreateRasporedModal" class="modal-overlay" @click="closeCreateRasporedModal">
      <div class="modal" @click.stop>
        <h3>Kreiraj raspored branja</h3>
        
        <form @submit.prevent="createRaspored">
          <div class="form-group">
            <label for="parcela">Parcela *</label>
            <select id="parcela" v-model="rasporedForm.parcelaIdp" required>
              <option value="">Izaberite parcelu</option>
              <option v-for="parcela in berba?.parcele" :key="parcela.parcelaIdp" :value="parcela.parcelaIdp">
                {{ parcela.nazivparcele }} ({{ parcela.vinogradNaziv }})
              </option>
            </select>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="pocbranja">Početak branja *</label>
              <input
                id="pocbranja"
                v-model="rasporedForm.pocbranja"
                type="date"
                required
              />
            </div>

            <div class="form-group">
              <label for="zavrsetakbranja">Završetak branja *</label>
              <input
                id="zavrsetakbranja"
                v-model="rasporedForm.zavrsetakbranja"
                type="date"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label for="ocekivaniprinos">Očekivani prinos (kg) *</label>
            <input
              id="ocekivaniprinos"
              v-model.number="rasporedForm.ocekivaniprinos"
              type="number"
              step="0.01"
              min="0"
              required
              placeholder="npr. 225.50"
            />
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeCreateRasporedModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="creatingRaspored">
              {{ creatingRaspored ? 'Kreiranje...' : 'Kreiraj' }}
            </button>
          </div>

          <div v-if="createRasporedError" class="error-message">{{ createRasporedError }}</div>
        </form>
      </div>
    </div>

    <!-- Edit Berba Modal -->
    <div v-if="showEditBerbaModal" class="modal-overlay" @click="closeEditBerbaModal">
      <div class="modal" @click.stop>
        <h3>Uredi berbu</h3>
        <form @submit.prevent="handleEditBerba">
          <div class="form-group">
            <label for="edit-nazber">Naziv berbe *</label>
            <input
              id="edit-nazber"
              v-model="editBerbaForm.nazber"
              type="text"
              required
              placeholder="npr. Berba 2024"
            />
          </div>

          <div class="form-group">
            <label for="edit-sezona">Sezona *</label>
            <input
              id="edit-sezona"
              v-model.number="editBerbaForm.sezona"
              type="number"
              required
              :min="2000"
              :max="2100"
              placeholder="npr. 2024"
            />
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditBerbaModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="editingBerba">
              {{ editingBerba ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="editBerbaError" class="error-message">{{ editBerbaError }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import berbaService from '@/services/berbaService'
import vinogradService from '@/services/vinogradService'
import { useAuthStore } from '@/stores/auth'
import type { BerbaDetail, ParcelaBerba, Rasporedbranja } from '@/types/berba'
import type { VinogradDetail } from '@/types/vinograd'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const berba = ref<BerbaDetail | null>(null)
const loading = ref(false)
const error = ref('')

const showAddParcelaModal = ref(false)
const loadingVinogradi = ref(false)
const allVinogradi = ref<VinogradDetail[]>([])
const selectedParcelaId = ref<number | null>(null)
const addingParcela = ref(false)
const addParcelaError = ref('')

const parcelaToRemove = ref<ParcelaBerba | null>(null)
const showDeleteConfirmation = ref(false)

// Rasporedi Branja
const rasporedi = ref<Rasporedbranja[]>([])
const showCreateRasporedModal = ref(false)
const creatingRaspored = ref(false)
const createRasporedError = ref('')
const rasporedForm = ref({
  parcelaIdp: '' as number | '',
  pocbranja: '',
  zavrsetakbranja: '',
  ocekivaniprinos: 0
})

// Edit Berba
const showEditBerbaModal = ref(false)
const editingBerba = ref(false)
const editBerbaError = ref('')
const editBerbaForm = ref({
  nazber: '',
  sezona: 0
})

const availableVinogradi = computed(() => {
  if (!berba.value) return []
  
  const existingParcelaIds = berba.value.parcele.map(p => p.idp)
  
  return allVinogradi.value
    .map(vinograd => ({
      ...vinograd,
      parcele: vinograd.parcele.filter(p => !existingParcelaIds.includes(p.idp))
    }))
    .filter(vinograd => vinograd.parcele.length > 0)
})

const loadBerba = async () => {
  try {
    loading.value = true
    error.value = ''
    const id = Number(route.params.id)
    berba.value = await berbaService.getBerbaById(id)
  } catch (err: any) {
    console.error('Error loading berba:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju berbe'
  } finally {
    loading.value = false
  }
}

const loadVinogradi = async () => {
  try {
    loadingVinogradi.value = true
    const vinogradi = await vinogradService.getAllVinogradi()
    
    const vinogradiWithParcele = await Promise.all(
      vinogradi.map(v => vinogradService.getVinogradById(v.idv))
    )
    
    allVinogradi.value = vinogradiWithParcele.filter(v => v && v.parcele.length > 0) as VinogradDetail[]
  } catch (err) {
    console.error('Error loading vinogradi:', err)
  } finally {
    loadingVinogradi.value = false
  }
}

const closeAddParcelaModal = () => {
  showAddParcelaModal.value = false
  selectedParcelaId.value = null
  addParcelaError.value = ''
}

const addParcela = async () => {
  if (!selectedParcelaId.value || !berba.value) return

  try {
    addingParcela.value = true
    addParcelaError.value = ''
    
    await berbaService.addParcelaToBerba(berba.value.idber, selectedParcelaId.value)
    await loadBerba()
    closeAddParcelaModal()
  } catch (err: any) {
    console.error('Error adding parcela:', err)
    addParcelaError.value = err.response?.data?.message || 'Greška pri dodavanju parcele'
  } finally {
    addingParcela.value = false
  }
}

const confirmRemoveParcela = (parcela: ParcelaBerba) => {
  parcelaToRemove.value = parcela
}

const cancelRemoveParcela = () => {
  parcelaToRemove.value = null
}

const removeParcela = async () => {
  if (!parcelaToRemove.value || !berba.value) return

  try {
    await berbaService.removeParcelaFromBerba(berba.value.idber, parcelaToRemove.value.parcelaIdp)
    await loadBerba()
    parcelaToRemove.value = null
  } catch (err: any) {
    console.error('Error removing parcela:', err)
    error.value = err.response?.data?.message || 'Greška pri uklanjanju parcele'
    parcelaToRemove.value = null
  }
}

const confirmDeleteBerba = () => {
  showDeleteConfirmation.value = true
}

const deleteBerba = async () => {
  if (!berba.value) return

  try {
    await berbaService.deleteBerba(berba.value.idber)
    router.push('/berbe')
  } catch (err: any) {
    console.error('Error deleting berba:', err)
    error.value = err.response?.data?.message || 'Greška pri brisanju berbe'
    showDeleteConfirmation.value = false
  }
}

// ==================== Raspored Branja Functions ====================

const loadRasporedi = async () => {
  if (!berba.value) return

  try {
    rasporedi.value = await berbaService.getAllRasporedi(berba.value.idber)
  } catch (err) {
    console.error('Error loading rasporedi:', err)
  }
}

const closeCreateRasporedModal = () => {
  showCreateRasporedModal.value = false
  rasporedForm.value = {
    parcelaIdp: '',
    pocbranja: '',
    zavrsetakbranja: '',
    ocekivaniprinos: 0
  }
  createRasporedError.value = ''
}

const createRaspored = async () => {
  if (!berba.value || !authStore.currentUser || rasporedForm.value.parcelaIdp === '') return

  try {
    creatingRaspored.value = true
    createRasporedError.value = ''

    await berbaService.createRaspored({
      pocbranja: rasporedForm.value.pocbranja,
      zavrsetakbranja: rasporedForm.value.zavrsetakbranja,
      ocekivaniprinos: rasporedForm.value.ocekivaniprinos,
      menadzerIdzap: authStore.currentUser.idzap,
      berbaIdber: berba.value.idber,
      parcelaIdp: rasporedForm.value.parcelaIdp as number
    })

    await loadRasporedi()
    closeCreateRasporedModal()
  } catch (err: any) {
    console.error('Error creating raspored:', err)
    createRasporedError.value = err.response?.data?.message || 'Greška pri kreiranju rasporeda'
  } finally {
    creatingRaspored.value = false
  }
}

// Edit Berba functions
const openEditBerbaModal = () => {
  if (!berba.value) return
  
  editBerbaForm.value = {
    nazber: berba.value.nazber,
    sezona: berba.value.sezona
  }
  showEditBerbaModal.value = true
}

const closeEditBerbaModal = () => {
  showEditBerbaModal.value = false
  editBerbaError.value = ''
}

const handleEditBerba = async () => {
  if (!berba.value) return
  
  try {
    editingBerba.value = true
    editBerbaError.value = ''
    
    await berbaService.updateBerba(berba.value.idber, editBerbaForm.value)
    
    // Osvježi podatke berbe
    await loadBerba()
    
    closeEditBerbaModal()
  } catch (err: any) {
    console.error('Error updating berba:', err)
    editBerbaError.value = err.response?.data?.message || 'Greška pri ažuriranju berbe'
  } finally {
    editingBerba.value = false
  }
}

const navigateToRaspored = (id: number) => {
  router.push(`/rasporedi/${id}`)
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const getProgressClass = (raspored: Rasporedbranja) => {
  if (raspored.brojRadnika === 0) return 'progress-none'
  const percentage = (raspored.brojRadnikaSaUnosom / raspored.brojRadnika) * 100
  if (percentage === 100) return 'progress-complete'
  if (percentage >= 50) return 'progress-partial'
  if (percentage > 0) return 'progress-started'
  return 'progress-none'
}

onMounted(async () => {
  await loadBerba()
  await loadVinogradi()
  await loadRasporedi()
})
</script>

<style scoped>
.berba-detail {
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

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.title-section {
  display: flex;
  align-items: center;
  gap: 20px;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.sezona-badge {
  background: #000;
  color: white;
  padding: 8px 20px;
  border-radius: 12px;
  font-size: 24px;
  font-weight: 700;
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
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
}

h2 {
  font-size: 28px;
  font-weight: 600;
  color: #000;
  margin: 0;
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

.parcele-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.rasporedi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.raspored-card {
  background: #f9f9f9;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.3s;
}

.raspored-card:hover {
  border-color: #000;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.raspored-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #ddd;
}

.raspored-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #000;
}

.radnici-badge {
  background: #000;
  color: white;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
}

.raspored-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.parcela-card {
  background: #f9f9f9;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  transition: all 0.3s;
}

.parcela-card:hover {
  border-color: #000;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.parcela-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #ddd;
}

.parcela-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #000;
}

.btn-delete-small {
  background: transparent;
  color: #c00;
  border: 1px solid #c00;
  border-radius: 4px;
  width: 28px;
  height: 28px;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-delete-small:hover {
  background: #c00;
  color: white;
}

.parcela-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-row .label {
  font-weight: 600;
  color: #666;
  font-size: 14px;
}

.info-row .value {
  color: #000;
  font-size: 15px;
}

.info-row .value.sorta-badge {
  background: #000;
  color: white !important;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.btn-stats {
  padding: 12px 24px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  text-decoration: none;
  display: inline-block;
  transition: all 0.3s;
}

.btn-stats:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
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
  margin-bottom: 12px;
  font-weight: 600;
  color: #000;
  font-size: 15px;
}

.form-group small {
  display: block;
  margin-top: 6px;
  font-size: 13px;
  color: #666;
  font-style: italic;
}

.vinogradi-select {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.vinograd-group h4 {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #000;
}

.parcele-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.parcela-option {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 12px;
  background: #f9f9f9;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
}

.parcela-option:hover {
  border-color: #000;
  background: white;
}

.parcela-option input[type="radio"] {
  margin-top: 4px;
  cursor: pointer;
}

.parcela-info-compact {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.parcela-info-compact strong {
  font-size: 15px;
  color: #000;
}

.parcela-info-compact span {
  font-size: 13px;
  color: #666;
}

.sorta-tag {
  background: #000;
  color: white;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 600;
  width: fit-content;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-top: 16px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-item label {
  font-size: 14px;
  font-weight: 600;
  color: #666;
}

.info-item p {
  margin: 0;
  font-size: 16px;
  font-weight: 500;
  color: #000;
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

/* Progress indicators for worker input status */
.progress-row {
  font-weight: 600;
}

.progress-text {
  padding: 4px 8px;
  border-radius: 4px;
  font-weight: 700;
}

.progress-complete {
  color: #2e7d32;
  background: #e8f5e9;
}

.progress-partial {
  color: #f57c00;
  background: #fff3e0;
}

.progress-started {
  color: #1976d2;
  background: #e3f2fd;
}

.progress-none {
  color: #d32f2f;
  background: #ffebee;
}

@media (max-width: 768px) {
  .berba-detail {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .title-section {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .parcele-grid {
    grid-template-columns: 1fr;
  }
}
</style>


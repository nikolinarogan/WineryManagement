<template>
  <div class="vinograd-detail">
    <div class="detail-container">
      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Vinograd Detail -->
      <div v-else-if="vinograd">
        <!-- Header -->
        <div class="header">
          <router-link to="/vinogradi" class="back-link">← Nazad</router-link>
          <h1>{{ vinograd.naziv }}</h1>
        </div>

        <!-- Osnovni Podaci -->
        <div class="info-section">
          <div class="section-header">
            <h2>Osnovni podaci</h2>
            <div class="header-actions">
              <button @click="openEditVinogradModal" class="btn-primary">
                ✎ Uredi vinograd
              </button>
              <button @click="confirmDeleteVinograd" class="btn-danger">
                Obriši vinograd
              </button>
            </div>
          </div>
          <div class="info-grid">
            <div class="info-item">
              <label>Površina</label>
              <p>{{ vinograd.povrsina }} ha</p>
            </div>
            <div class="info-item">
              <label>Nadmorska visina</label>
              <p>{{ vinograd.nadmorskavis }} m</p>
            </div>
            <div class="info-item">
              <label>Tip zemljišta</label>
              <p>{{ vinograd.tipzemljista }}</p>
            </div>
            <div class="info-item">
              <label>Navodnjavanje</label>
              <p>{{ vinograd.navodnjavanje === 'D' ? 'Da' : 'Ne' }}</p>
            </div>
            <div class="info-item">
              <label>Datum osnivanja</label>
              <p>{{ formatDate(vinograd.datosn) }}</p>
            </div>
          </div>
        </div>

        <!-- Parcele -->
        <div class="parcele-section">
          <div class="section-header">
            <h2>Parcele ({{ vinograd.parcele.length }})</h2>
            <button @click="showAddParcelaModal = true" class="btn-primary">
              + Dodaj parcelu
            </button>
          </div>

          <div v-if="vinograd.parcele.length === 0" class="no-parcele">
            <p>Vinograd još nema parcele.</p>
          </div>

          <div v-else class="parcele-grid">
            <div v-for="parcela in vinograd.parcele" :key="parcela.idp" class="parcela-card">
              <div class="parcela-header">
                <h3>{{ parcela.nazivparcele }}</h3>
                <div class="header-actions">
                  <button @click="openEditParcelaModal(parcela)" class="btn-edit-small">
                    ✎
                  </button>
                  <button @click="confirmDeleteParcela(parcela)" class="btn-delete-small">
                    ✕
                  </button>
                </div>
              </div>
              <div class="parcela-info">
                <div class="info-row">
                  <span class="label">Broj čokota:</span>
                  <span class="value">{{ parcela.brojcokota }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Površina:</span>
                  <span class="value">{{ parcela.povrsina }} ha</span>
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
      </div>
    </div>

    <!-- Add Parcela Modal -->
    <div v-if="showAddParcelaModal" class="modal-overlay" @click="closeAddParcelaModal">
      <div class="modal" @click.stop>
        <h3>Dodaj parcelu</h3>
        <form @submit.prevent="addParcela">
          <div class="form-group">
            <label for="nazivparcele">Naziv parcele *</label>
            <input
              id="nazivparcele"
              v-model="newParcela.nazivparcele"
              type="text"
              required
              placeholder="npr. Parcela A1"
            />
          </div>

          <div class="form-group">
            <label for="brojcokota">Broj čokota *</label>
            <input
              id="brojcokota"
              v-model.number="newParcela.brojcokota"
              type="number"
              required
              placeholder="npr. 150"
            />
          </div>

          <div class="form-group">
            <label for="povrsina">Površina (ha) *</label>
            <input
              id="povrsina"
              v-model.number="newParcela.povrsina"
              type="number"
              step="0.01"
              required
              placeholder="npr. 2.5"
            />
          </div>

          <div class="form-group">
            <label for="sortagrozdja">Sorta grožđa</label>
            <select id="sortagrozdja" v-model="newParcela.sortagrozdjaIdsrt">
              <option :value="undefined">Nije dodijeljeno</option>
              <option v-for="sorta in sorte" :key="sorta.idsrt" :value="sorta.idsrt">
                {{ sorta.nazivsorte }} ({{ sorta.bojasorte }})
              </option>
            </select>
            <small>Opciono - možete kasnije dodati</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeAddParcelaModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="addingParcela">
              {{ addingParcela ? 'Dodajem...' : 'Dodaj' }}
            </button>
          </div>

          <div v-if="addParcelaError" class="error-message">{{ addParcelaError }}</div>
        </form>
      </div>
    </div>

    <!-- Edit Parcela Modal -->
    <div v-if="showEditParcelaModal && parcelaToEdit" class="modal-overlay" @click="closeEditParcelaModal">
      <div class="modal" @click.stop>
        <h3>Uredi parcelu</h3>
        <form @submit.prevent="editParcela">
          <div class="form-group">
            <label for="edit-nazivparcele">Naziv parcele *</label>
            <input
              id="edit-nazivparcele"
              v-model="editParcelaForm.nazivparcele"
              type="text"
              required
              placeholder="npr. Parcela A1"
            />
          </div>

          <div class="form-group">
            <label for="edit-brojcokota">Broj čokota *</label>
            <input
              id="edit-brojcokota"
              v-model.number="editParcelaForm.brojcokota"
              type="number"
              required
              placeholder="npr. 150"
            />
          </div>

          <div class="form-group">
            <label for="edit-povrsina">Površina (ha) *</label>
            <input
              id="edit-povrsina"
              v-model.number="editParcelaForm.povrsina"
              type="number"
              step="0.01"
              required
              placeholder="npr. 2.5"
            />
          </div>

          <div class="form-group">
            <label for="edit-sortagrozdja">Sorta grožđa</label>
            <select id="edit-sortagrozdja" v-model="editParcelaForm.sortagrozdjaIdsrt">
              <option :value="undefined">Nije dodijeljeno</option>
              <option v-for="sorta in sorte" :key="sorta.idsrt" :value="sorta.idsrt">
                {{ sorta.nazivsorte }} ({{ sorta.bojasorte }})
              </option>
            </select>
            <small>Izaberite sortu grožđa koja se uzgaja na ovoj parceli</small>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditParcelaModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="editingParcela">
              {{ editingParcela ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="editParcelaError" class="error-message">{{ editParcelaError }}</div>
        </form>
      </div>
    </div>

    <!-- Delete Parcela Confirmation Modal -->
    <div v-if="parcelaToDelete" class="modal-overlay" @click="cancelDeleteParcela">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja parcele</h3>
        <p>Da li ste sigurni da želite obrisati parcelu <strong>{{ parcelaToDelete.nazivparcele }}</strong>?</p>
        <div class="modal-actions">
          <button @click="cancelDeleteParcela" class="btn-secondary">Odustani</button>
          <button @click="deleteParcela" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>

    <!-- Delete Vinograd Confirmation Modal -->
    <div v-if="showDeleteVinogradModal" class="modal-overlay" @click="cancelDeleteVinograd">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja vinograda</h3>
        <p>Da li ste sigurni da želite obrisati vinograd <strong>{{ vinograd?.naziv }}</strong>?</p>
        <p class="warning-text">⚠️ Ova akcija će obrisati i sve parcele ovog vinograda!</p>
        <div class="modal-actions">
          <button @click="cancelDeleteVinograd" class="btn-secondary">Odustani</button>
          <button @click="deleteVinograd" class="btn-danger" :disabled="deletingVinograd">
            {{ deletingVinograd ? 'Brišem...' : 'Obriši' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Edit Vinograd Modal -->
    <div v-if="showEditVinogradModal" class="modal-overlay" @click="closeEditVinogradModal">
      <div class="modal" @click.stop>
        <h3>Uredi vinograd</h3>
        <form @submit.prevent="editVinograd">
          <div class="form-group">
            <label for="edit-naziv">Naziv *</label>
            <input
              id="edit-naziv"
              v-model="editVinogradForm.naziv"
              type="text"
              required
              placeholder="npr. Vinograd Sremski Karlovci"
            />
          </div>

          <div class="form-group">
            <label for="edit-datosn">Datum osnivanja *</label>
            <input
              id="edit-datosn"
              v-model="editVinogradForm.datosn"
              type="date"
              required
            />
          </div>

          <div class="form-group">
            <label for="edit-povrsina">Površina (ha) *</label>
            <input
              id="edit-povrsina"
              v-model.number="editVinogradForm.povrsina"
              type="number"
              step="0.01"
              required
              placeholder="npr. 15.5"
            />
          </div>

          <div class="form-group">
            <label for="edit-nadmorskavis">Nadmorska visina (m) *</label>
            <input
              id="edit-nadmorskavis"
              v-model.number="editVinogradForm.nadmorskavis"
              type="number"
              required
              placeholder="npr. 250"
            />
          </div>

          <div class="form-group">
            <label for="edit-tipzemljista">Tip zemljišta *</label>
            <select id="edit-tipzemljista" v-model="editVinogradForm.tipzemljista" required>
              <option value="">Izaberite tip</option>
              <option value="Černozem">Černozem</option>
              <option value="Krečnjak">Krečnjak</option>
              <option value="Pjeskovito">Pjeskovito</option>
              <option value="Glineno">Glineno</option>
              <option value="Mješovito">Mješovito</option>
            </select>
          </div>

          <div class="form-group">
            <label for="edit-navodnjavanje">Navodnjavanje *</label>
            <select id="edit-navodnjavanje" v-model="editVinogradForm.navodnjavanje" required>
              <option value="">Izaberite opciju</option>
              <option value="D">Da</option>
              <option value="N">Ne</option>
            </select>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeEditVinogradModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="editingVinograd">
              {{ editingVinograd ? 'Čuvam...' : 'Sačuvaj' }}
            </button>
          </div>

          <div v-if="editVinogradError" class="error-message">{{ editVinogradError }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import vinogradService from '@/services/vinogradService'
import sortagrozdjaService from '@/services/sortagrozdjaService'
import type { VinogradDetail, Parcela, CreateParcelaRequest } from '@/types/vinograd'
import type { Sortagrozdja } from '@/types/sortagrozdja'

const route = useRoute()
const router = useRouter()
const vinograd = ref<VinogradDetail | null>(null)
const loading = ref(false)
const error = ref('')
const sorte = ref<Sortagrozdja[]>([])

const showAddParcelaModal = ref(false)
const addingParcela = ref(false)
const addParcelaError = ref('')
const newParcela = ref<CreateParcelaRequest>({
  nazivparcele: '',
  brojcokota: 0,
  povrsina: 0
})

const showEditParcelaModal = ref(false)
const editingParcela = ref(false)
const editParcelaError = ref('')
const parcelaToEdit = ref<Parcela | null>(null)
const editParcelaForm = ref({
  nazivparcele: '',
  brojcokota: 0,
  povrsina: 0,
  sortagrozdjaIdsrt: undefined as number | undefined
})

const parcelaToDelete = ref<Parcela | null>(null)

const showEditVinogradModal = ref(false)
const editingVinograd = ref(false)
const editVinogradError = ref('')
const editVinogradForm = ref({
  naziv: '',
  datosn: '',
  povrsina: 0,
  nadmorskavis: 0,
  tipzemljista: '',
  navodnjavanje: ''
})

const showDeleteVinogradModal = ref(false)
const deletingVinograd = ref(false)

const loadVinograd = async () => {
  try {
    loading.value = true
    error.value = ''
    const id = Number(route.params.id)
    vinograd.value = await vinogradService.getVinogradById(id)
  } catch (err: any) {
    console.error('Error loading vinograd:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju vinograda'
  } finally {
    loading.value = false
  }
}

const loadSorte = async () => {
  try {
    sorte.value = await sortagrozdjaService.getAllSorte()
  } catch (err) {
    console.error('Error loading sorte:', err)
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const closeAddParcelaModal = () => {
  showAddParcelaModal.value = false
  addParcelaError.value = ''
  newParcela.value = {
    nazivparcele: '',
    brojcokota: 0,
    povrsina: 0
  }
}

const openEditParcelaModal = (parcela: Parcela) => {
  parcelaToEdit.value = parcela
  editParcelaForm.value = {
    nazivparcele: parcela.nazivparcele,
    brojcokota: parcela.brojcokota,
    povrsina: parcela.povrsina,
    sortagrozdjaIdsrt: parcela.sortagrozdjaIdsrt
  }
  showEditParcelaModal.value = true
}

const closeEditParcelaModal = () => {
  showEditParcelaModal.value = false
  editParcelaError.value = ''
  parcelaToEdit.value = null
}

const editParcela = async () => {
  if (!parcelaToEdit.value) return

  try {
    editingParcela.value = true
    editParcelaError.value = ''
    await vinogradService.updateParcela(parcelaToEdit.value.idp, editParcelaForm.value)
    await loadVinograd()
    closeEditParcelaModal()
  } catch (err: any) {
    console.error('Error updating parcela:', err)
    editParcelaError.value = err.response?.data?.message || 'Greška pri ažuriranju parcele'
  } finally {
    editingParcela.value = false
  }
}

const addParcela = async () => {
  try {
    addingParcela.value = true
    addParcelaError.value = ''
    const id = Number(route.params.id)
    await vinogradService.addParcela(id, newParcela.value)
    await loadVinograd()
    closeAddParcelaModal()
  } catch (err: any) {
    console.error('Error adding parcela:', err)
    addParcelaError.value = err.response?.data?.message || 'Greška pri dodavanju parcele'
  } finally {
    addingParcela.value = false
  }
}

const confirmDeleteParcela = (parcela: Parcela) => {
  parcelaToDelete.value = parcela
}

const cancelDeleteParcela = () => {
  parcelaToDelete.value = null
}

const deleteParcela = async () => {
  if (!parcelaToDelete.value) return

  try {
    await vinogradService.deleteParcela(parcelaToDelete.value.idp)
    await loadVinograd()
    parcelaToDelete.value = null
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri brisanju parcele'
    parcelaToDelete.value = null
  }
}

const openEditVinogradModal = () => {
  if (!vinograd.value) return
  
  editVinogradForm.value = {
    naziv: vinograd.value.naziv,
    datosn: vinograd.value.datosn,
    povrsina: vinograd.value.povrsina,
    nadmorskavis: vinograd.value.nadmorskavis,
    tipzemljista: vinograd.value.tipzemljista,
    navodnjavanje: vinograd.value.navodnjavanje
  }
  showEditVinogradModal.value = true
}

const closeEditVinogradModal = () => {
  showEditVinogradModal.value = false
  editVinogradError.value = ''
}

const editVinograd = async () => {
  try {
    editingVinograd.value = true
    editVinogradError.value = ''
    const id = Number(route.params.id)
    await vinogradService.updateVinograd(id, editVinogradForm.value)
    await loadVinograd()
    closeEditVinogradModal()
  } catch (err: any) {
    console.error('Error updating vinograd:', err)
    editVinogradError.value = err.response?.data?.message || 'Greška pri ažuriranju vinograda'
  } finally {
    editingVinograd.value = false
  }
}

const confirmDeleteVinograd = () => {
  showDeleteVinogradModal.value = true
}

const cancelDeleteVinograd = () => {
  showDeleteVinogradModal.value = false
}

const deleteVinograd = async () => {
  try {
    deletingVinograd.value = true
    const id = Number(route.params.id)
    await vinogradService.deleteVinograd(id)
    // Redirect to vinogradi list after successful deletion
    router.push('/vinogradi')
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri brisanju vinograda'
    showDeleteVinogradModal.value = false
  } finally {
    deletingVinograd.value = false
  }
}

onMounted(() => {
  loadVinograd()
  loadSorte()
})
</script>

<style scoped>
.vinograd-detail {
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
  margin-bottom: 24px;
}

.header {
  margin-bottom: 32px;
}

.back-link {
  display: inline-block;
  color: #666;
  text-decoration: none;
  font-size: 16px;
  margin-bottom: 12px;
  transition: color 0.3s;
}

.back-link:hover {
  color: #000;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.info-section,
.parcele-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

h2 {
  font-size: 24px;
  font-weight: 600;
  color: #000;
  margin: 0 0 24px 0;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
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
  font-size: 12px;
  font-weight: 600;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-item p {
  font-size: 18px;
  font-weight: 500;
  color: #000;
  margin: 0;
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

.section-header .header-actions {
  display: flex;
  gap: 12px;
}

.btn-primary {
  padding: 10px 20px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.no-parcele {
  padding: 40px;
  text-align: center;
  background: #f9f9f9;
  border-radius: 8px;
  border: 2px dashed #ccc;
}

.no-parcele p {
  margin: 0;
  color: #666;
  font-size: 16px;
}

.parcele-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}

.parcela-card {
  background: #f9f9f9;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  transition: all 0.3s;
}

.parcela-card:hover {
  border-color: #000;
}

.parcela-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.parcela-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #000;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.btn-edit-small,
.btn-delete-small {
  width: 28px;
  height: 28px;
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-edit-small {
  background: #000;
}

.btn-edit-small:hover {
  background: #333;
  transform: scale(1.1);
}

.btn-delete-small {
  background: #c00;
}

.btn-delete-small:hover {
  background: #a00;
  transform: scale(1.1);
}

.parcela-info .info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #e0e0e0;
}

.parcela-info .info-row:last-child {
  border-bottom: none;
}

.parcela-info .label {
  font-weight: 600;
  color: #666;
  font-size: 14px;
}

.parcela-info .value {
  color: #000;
  font-size: 14px;
}

.parcela-info .value.sorta-badge {
  background: #000;
  color: white;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
}

.parcela-info .value:not(.sorta-badge) {
  color: #666;
  font-style: italic;
}

.form-group small {
  display: block;
  margin-top: 4px;
  font-size: 12px;
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
  background: white;
  padding: 32px;
  border-radius: 12px;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.modal h3 {
  margin: 0 0 24px 0;
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
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #000;
}

.form-group select {
  cursor: pointer;
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

.btn-danger {
  padding: 10px 20px;
  background: #c00;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-danger:hover:not(:disabled) {
  background: #a00;
}

.btn-danger:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.warning-text {
  color: #c00 !important;
  font-weight: 600;
  font-size: 14px;
  margin-top: 12px;
}

@media (max-width: 768px) {
  .vinograd-detail {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .info-section,
  .parcele-section {
    padding: 24px;
  }

  .info-grid {
    grid-template-columns: 1fr;
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


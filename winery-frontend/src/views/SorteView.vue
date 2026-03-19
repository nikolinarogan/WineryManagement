<template>
  <div class="sorte">
    <div class="sorte-container">
      <h1>Sorte grožđa</h1>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Sorte Grid -->
      <div v-else-if="sorte.length > 0" class="sorte-grid">
        <div
          v-for="sorta in sorte"
          :key="sorta.idsrt"
          class="sorta-card"
          @click="viewSortaDetail(sorta.idsrt)"
        >
          <div class="card-header">
            <h3>{{ sorta.nazivsorte }}</h3>
            <span class="parcele-count">{{ sorta.brojParcela }} parcela</span>
          </div>
          
          <div class="card-body">
            <div class="info-row">
              <span class="label">Boja:</span>
              <span class="value">{{ sorta.bojasorte }}</span>
            </div>
            <div class="info-row">
              <span class="label">Porijeklo:</span>
              <span class="value">{{ sorta.porijeklosorte }}</span>
            </div>
            <div class="info-row">
              <span class="label">Period sazrijevanja:</span>
              <span class="value">{{ sorta.periodsazr }}</span>
            </div>
            <div class="info-row">
              <span class="label">Kiselost:</span>
              <span class="value">{{ sorta.kiselost }}</span>
            </div>
          </div>

          <!-- <div class="card-footer">
            <button @click.stop="confirmDelete(sorta)" class="btn-delete">
              Obriši
            </button>
          </div> -->
        </div>
      </div>

      <!-- No Results -->
      <div v-else class="no-results">
        <p>Nema sorti za prikaz.</p>
        <button @click="showCreateModal = true" class="btn-primary">
          Dodaj prvu sortu
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || selectedSorta" class="modal-overlay" @click="closeModal">
      <div class="modal" @click.stop>
        <h3>{{ selectedSorta ? 'Uredi Sortu' : 'Dodaj Novu Sortu' }}</h3>
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="nazivsorte">Naziv sorte *</label>
            <input
              id="nazivsorte"
              v-model="form.nazivsorte"
              type="text"
              required
              placeholder="npr. Chardonnay"
            />
          </div>

          <div class="form-group">
            <label for="bojasorte">Boja sorte *</label>
            <select id="bojasorte" v-model="form.bojasorte" required>
              <option value="">Izaberite boju</option>
              <option value="bela">Bela</option>
              <option value="crvena">Crvena</option>
              <option value="roze">Roze</option>
            </select>
          </div>

          <div class="form-group">
            <label for="porijeklosorte">Porijeklo *</label>
            <input
              id="porijeklosorte"
              v-model="form.porijeklosorte"
              type="text"
              required
              placeholder="npr. Francuska"
            />
          </div>

          <div class="form-group">
            <label for="periodsazr">Period sazrijevanja *</label>
            <input
              id="periodsazr"
              v-model="form.periodsazr"
              type="text"
              required
              placeholder="npr. Rani, Srednji, Kasni"
            />
          </div>

          <div class="form-group">
            <label for="kiselost">Kiselost *</label>
            <input
              id="kiselost"
              v-model.number="form.kiselost"
              type="number"
              step="0.1"
              required
              placeholder="npr. 6.5"
            />
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeModal" class="btn-secondary">
              Odustani
            </button>
            <button type="submit" class="btn-primary" :disabled="submitting">
              {{ submitting ? 'Čuvam...' : (selectedSorta ? 'Sačuvaj' : 'Dodaj') }}
            </button>
          </div>

          <div v-if="formError" class="error-message">{{ formError }}</div>
        </form>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="sortaToDelete" class="modal-overlay" @click="cancelDelete">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja</h3>
        <p>Da li ste sigurni da želite obrisati sortu <strong>{{ sortaToDelete.nazivsorte }}</strong>?</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="btn-secondary">Odustani</button>
          <button @click="deleteSorta" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>

    <!-- Detail Modal -->
    <div v-if="showDetailModal && sortaDetail" class="modal-overlay" @click="closeDetailModal">
      <div class="modal large" @click.stop>
        <h3>{{ sortaDetail.nazivsorte }}</h3>
        
        <div class="detail-info">
          <div class="info-row">
            <span class="label">Boja:</span>
            <span class="value">{{ sortaDetail.bojasorte }}</span>
          </div>
          <div class="info-row">
            <span class="label">Porijeklo:</span>
            <span class="value">{{ sortaDetail.porijeklosorte }}</span>
          </div>
          <div class="info-row">
            <span class="label">Period sazrijevanja:</span>
            <span class="value">{{ sortaDetail.periodsazr }}</span>
          </div>
          <div class="info-row">
            <span class="label">Kiselost:</span>
            <span class="value">{{ sortaDetail.kiselost }}</span>
          </div>
        </div>

        <h4>Parcele na kojima se uzgaja ({{ sortaDetail.parcele.length }})</h4>
        
        <div v-if="sortaDetail.parcele.length > 0" class="parcele-list">
          <div v-for="parcela in sortaDetail.parcele" :key="parcela.idp" class="parcela-item">
            <div class="parcela-name">
              <strong>{{ parcela.nazivparcele }}</strong>
              <span class="vinograd-name">{{ parcela.vinogradNaziv }}</span>
            </div>
            <div class="parcela-details">
              <span>{{ parcela.povrsina }} ha</span>
              <span>{{ parcela.brojcokota }} čokota</span>
            </div>
          </div>
        </div>
        
        <div v-else class="no-parcele">
          <p>Ova sorta se trenutno ne uzgaja ni na jednoj parceli.</p>
        </div>

        <div class="modal-actions">
          <button @click="closeDetailModal" class="btn-secondary">Zatvori</button>
          <button @click="openEditFromDetail" class="btn-primary">✎ Uredi sortu</button>
          <button @click="confirmDeleteFromDetail" class="btn-danger">Obriši sortu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import sortagrozdjaService from '@/services/sortagrozdjaService'
import type { Sortagrozdja, SortagrozdjaDetail, CreateSortagrozdjaRequest } from '@/types/sortagrozdja'

const sorte = ref<Sortagrozdja[]>([])
const loading = ref(false)
const error = ref('')
const sortaToDelete = ref<Sortagrozdja | null>(null)
const showCreateModal = ref(false)
const selectedSorta = ref<Sortagrozdja | null>(null)
const submitting = ref(false)
const formError = ref('')

const showDetailModal = ref(false)
const sortaDetail = ref<SortagrozdjaDetail | null>(null)

const form = ref<CreateSortagrozdjaRequest>({
  nazivsorte: '',
  bojasorte: '',
  porijeklosorte: '',
  periodsazr: '',
  kiselost: 0
})

const loadSorte = async () => {
  try {
    loading.value = true
    error.value = ''
    sorte.value = await sortagrozdjaService.getAllSorte()
  } catch (err: any) {
    console.error('Error loading sorte:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju sorti'
  } finally {
    loading.value = false
  }
}

const viewSortaDetail = async (id: number) => {
  try {
    sortaDetail.value = await sortagrozdjaService.getSortaById(id)
    showDetailModal.value = true
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri učitavanju detalja sorte'
  }
}

const closeDetailModal = () => {
  showDetailModal.value = false
  sortaDetail.value = null
}

const openEditFromDetail = () => {
  if (!sortaDetail.value) return
  
  // Postavi formu sa trenutnim podacima
  form.value = {
    nazivsorte: sortaDetail.value.nazivsorte,
    bojasorte: sortaDetail.value.bojasorte,
    porijeklosorte: sortaDetail.value.porijeklosorte,
    periodsazr: sortaDetail.value.periodsazr,
    kiselost: sortaDetail.value.kiselost
  }
  
  // Postavi selectedSorta da bi modal znao da je edit mode
  selectedSorta.value = {
    idsrt: sortaDetail.value.idsrt,
    nazivsorte: sortaDetail.value.nazivsorte,
    bojasorte: sortaDetail.value.bojasorte,
    porijeklosorte: sortaDetail.value.porijeklosorte,
    periodsazr: sortaDetail.value.periodsazr,
    kiselost: sortaDetail.value.kiselost,
    brojParcela: sortaDetail.value.parcele.length
  }
  
  // Zatvori detail modal
  showDetailModal.value = false
}

const closeModal = () => {
  const sortaIdToReopen = selectedSorta.value?.idsrt
  
  showCreateModal.value = false
  selectedSorta.value = null
  formError.value = ''
  form.value = {
    nazivsorte: '',
    bojasorte: '',
    porijeklosorte: '',
    periodsazr: '',
    kiselost: 0
  }
  
  // Ako je bilo editovanje (imali smo ID sorte), vrati na detail modal
  if (sortaIdToReopen) {
    viewSortaDetail(sortaIdToReopen)
  }
}

const handleSubmit = async () => {
  try {
    submitting.value = true
    formError.value = ''
    
    const isEditMode = !!selectedSorta.value
    const sortaIdToReopen = selectedSorta.value?.idsrt
    
    if (isEditMode) {
      // Edit mode
      await sortagrozdjaService.updateSorta(selectedSorta.value!.idsrt, form.value)
    } else {
      // Create mode
      await sortagrozdjaService.createSorta(form.value)
    }
    
    await loadSorte()
    
    // Resetuj formu i modal
    showCreateModal.value = false
    selectedSorta.value = null
    formError.value = ''
    form.value = {
      nazivsorte: '',
      bojasorte: '',
      porijeklosorte: '',
      periodsazr: '',
      kiselost: 0
    }
    
    // Ako je bilo editovanje, vrati na detail modal sa ažuriranim podacima
    if (isEditMode && sortaIdToReopen) {
      await viewSortaDetail(sortaIdToReopen)
    }
  } catch (err: any) {
    formError.value = err.response?.data?.message || 'Greška pri čuvanju sorte'
  } finally {
    submitting.value = false
  }
}

const confirmDelete = (sorta: Sortagrozdja) => {
  sortaToDelete.value = sorta
}

const cancelDelete = () => {
  sortaToDelete.value = null
}

const deleteSorta = async () => {
  if (!sortaToDelete.value) return

  try {
    await sortagrozdjaService.deleteSorta(sortaToDelete.value.idsrt)
    await loadSorte()
    sortaToDelete.value = null
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri brisanju sorte'
    sortaToDelete.value = null
  }
}

const confirmDeleteFromDetail = () => {
  if (!sortaDetail.value) return
  
  // Kreiraj sortaToDelete objekat iz sortaDetail
  sortaToDelete.value = {
    idsrt: sortaDetail.value.idsrt,
    nazivsorte: sortaDetail.value.nazivsorte,
    bojasorte: sortaDetail.value.bojasorte,
    porijeklosorte: sortaDetail.value.porijeklosorte,
    periodsazr: sortaDetail.value.periodsazr,
    kiselost: sortaDetail.value.kiselost,
    brojParcela: sortaDetail.value.parcele.length
  }
  
  // Zatvori detail modal
  showDetailModal.value = false
}

onMounted(() => {
  loadSorte()
})
</script>

<style scoped>
.sorte {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.sorte-container {
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 40px 0;
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
  margin-bottom: 16px;
}

.sorte-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 24px;
}

.sorta-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.3s;
  overflow: hidden;
}

.sorta-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.card-header {
  padding: 20px;
  background: linear-gradient(135deg, #000 0%, #434343 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  margin: 0;
  font-size: 22px;
  font-weight: 600;
}

.parcele-count {
  background: rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(10px);
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
}

.card-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #e0e0e0;
}

.info-row:last-child {
  border-bottom: none;
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

.card-footer {
  padding: 16px 20px;
  background: #f9f9f9;
  display: flex;
  justify-content: flex-end;
}

.btn-delete {
  padding: 8px 16px;
  background: transparent;
  color: #c00;
  border: 1px solid #c00;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-delete:hover {
  background: #c00;
  color: white;
}

.no-results {
  background: white;
  padding: 60px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.no-results p {
  font-size: 18px;
  color: #666;
  margin: 0 0 24px 0;
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
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  max-height: 90vh;
  overflow-y: auto;
}

.modal.large {
  max-width: 700px;
}

.modal h3 {
  margin: 0 0 24px 0;
  font-size: 24px;
  color: #000;
}

.modal h4 {
  margin: 24px 0 16px 0;
  font-size: 18px;
  color: #000;
  padding-bottom: 12px;
  border-bottom: 2px solid #e0e0e0;
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
  font-size: 15px;
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

.detail-info {
  background: #f9f9f9;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 16px;
}

.parcele-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.parcela-item {
  background: #f9f9f9;
  padding: 16px;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.parcela-name {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.parcela-name strong {
  font-size: 16px;
  color: #000;
}

.vinograd-name {
  font-size: 13px;
  color: #666;
}

.parcela-details {
  display: flex;
  gap: 16px;
  font-size: 14px;
  color: #666;
}

.no-parcele {
  text-align: center;
  padding: 32px;
  background: #f9f9f9;
  border-radius: 8px;
  border: 2px dashed #ccc;
}

.no-parcele p {
  margin: 0;
  color: #666;
  font-size: 15px;
}

@media (max-width: 768px) {
  .sorte {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .sorte-grid {
    grid-template-columns: 1fr;
  }

  .parcela-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>


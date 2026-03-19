<template>
  <div class="vinogradi">
    <div class="vinogradi-container">
      <h1>Vinogradi</h1>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Vinogradi Grid -->
      <div v-else-if="vinogradi.length > 0" class="vinogradi-grid">
        <div
          v-for="vinograd in vinogradi"
          :key="vinograd.idv"
          class="vinograd-card"
          @click="navigateToDetail(vinograd.idv)"
        >
          <div class="card-header">
            <h3>{{ vinograd.naziv }}</h3>
            <span class="parcele-count">{{ vinograd.brojParcela }} parcela</span>
          </div>
          
          <div class="card-body">
            <div class="info-row">
              <span class="label">Površina:</span>
              <span class="value">{{ vinograd.povrsina }} ha</span>
            </div>
            <div class="info-row">
              <span class="label">Nadmorska visina:</span>
              <span class="value">{{ vinograd.nadmorskavis }} m</span>
            </div>
            <div class="info-row">
              <span class="label">Tip zemljišta:</span>
              <span class="value">{{ vinograd.tipzemljista }}</span>
            </div>
            <div class="info-row">
              <span class="label">Navodnjavanje:</span>
              <span class="value">{{ vinograd.navodnjavanje === 'D' ? 'Da' : 'Ne' }}</span>
            </div>
            <div class="info-row">
              <span class="label">Datum osnivanja:</span>
              <span class="value">{{ formatDate(vinograd.datosn) }}</span>
            </div>
          </div>


          <!-- <div class="card-footer">
            <button @click.stop="confirmDelete(vinograd)" class="btn-delete">
              Obriši
            </button>
          </div> -->
        </div>
      </div>

      <!-- No Results -->
      <div v-else class="no-results">
        <p>Nema vinograda za prikaz.</p>
        <router-link to="/vinogradi/create" class="btn-primary">
          Kreiraj prvi vinograd
        </router-link>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="vinogradToDelete" class="modal-overlay" @click="cancelDelete">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja</h3>
        <p>Da li ste sigurni da želite obrisati vinograd <strong>{{ vinogradToDelete.naziv }}</strong>?</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="btn-secondary">Odustani</button>
          <button @click="deleteVinograd" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import vinogradService from '@/services/vinogradService'
import type { Vinograd } from '@/types/vinograd'

const router = useRouter()
const vinogradi = ref<Vinograd[]>([])
const loading = ref(false)
const error = ref('')
const vinogradToDelete = ref<Vinograd | null>(null)

const loadVinogradi = async () => {
  try {
    loading.value = true
    error.value = ''
    vinogradi.value = await vinogradService.getAllVinogradi()
  } catch (err: any) {
    console.error('Error loading vinogradi:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju vinograda'
  } finally {
    loading.value = false
  }
}

const navigateToDetail = (id: number) => {
  router.push(`/vinogradi/${id}`)
}

const confirmDelete = (vinograd: Vinograd) => {
  vinogradToDelete.value = vinograd
}

const cancelDelete = () => {
  vinogradToDelete.value = null
}

const deleteVinograd = async () => {
  if (!vinogradToDelete.value) return

  try {
    await vinogradService.deleteVinograd(vinogradToDelete.value.idv)
    await loadVinogradi()
    vinogradToDelete.value = null
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri brisanju vinograda'
    vinogradToDelete.value = null
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

onMounted(() => {
  loadVinogradi()
})
</script>

<style scoped>
.vinogradi {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.vinogradi-container {
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
  text-decoration: none;
  display: inline-block;
}

.btn-primary:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
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
}

.vinogradi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 24px;
}

.vinograd-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.3s;
  overflow: hidden;
}

.vinograd-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.card-header {
  padding: 20px;
  background: #000;
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
  background: white;
  color: #000;
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

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
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

.btn-danger:hover {
  background: #a00;
}

@media (max-width: 768px) {
  .vinogradi {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .vinogradi-grid {
    grid-template-columns: 1fr;
  }
}
</style>


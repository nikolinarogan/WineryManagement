<template>
  <div class="berbe">
    <div class="berbe-container">
      <div class="header">
        <h1>Berbe</h1>
        <!-- <router-link to="/berbe/create" class="btn-primary">
          + Kreiraj Berbu
        </router-link> -->
      </div>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Berbe Grid -->
      <div v-else-if="berbe.length > 0" class="berbe-grid">
        <div
          v-for="berba in berbe"
          :key="berba.idber"
          class="berba-card"
          @click="navigateToDetail(berba.idber)"
        >
          <div class="card-header">
            <h3>{{ berba.nazber }}</h3>
            <span class="sezona-badge">{{ berba.sezona }}</span>
          </div>
          
          <div class="card-body">
            <div class="info-row">
              <span class="label">Parcele:</span>
              <span class="value">{{ berba.brojParcela }} parcela</span>
            </div>
          </div>

          <!-- <div class="card-footer">
            <button @click.stop="confirmDelete(berba)" class="btn-delete">
              Obriši
            </button>
          </div> -->
        </div>
      </div>

      <!-- No Results -->
      <div v-else class="no-results">
        <p>Nema berbi za prikaz.</p>
        <router-link to="/berbe/create" class="btn-primary">
          Kreiraj prvu berbu
        </router-link>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="berbaToDelete" class="modal-overlay" @click="cancelDelete">
      <div class="modal" @click.stop>
        <h3>Potvrda brisanja</h3>
        <p>Da li ste sigurni da želite obrisati berbu <strong>{{ berbaToDelete.nazber }}</strong>?</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="btn-secondary">Odustani</button>
          <button @click="deleteBerba" class="btn-danger">Obriši</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import berbaService from '@/services/berbaService'
import type { Berba } from '@/types/berba'

const router = useRouter()
const berbe = ref<Berba[]>([])
const loading = ref(false)
const error = ref('')
const berbaToDelete = ref<Berba | null>(null)

const loadBerbe = async () => {
  try {
    loading.value = true
    error.value = ''
    berbe.value = await berbaService.getAllBerbe()
  } catch (err: any) {
    console.error('Error loading berbe:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju berbi'
  } finally {
    loading.value = false
  }
}

const navigateToDetail = (id: number) => {
  router.push(`/berbe/${id}`)
}

const confirmDelete = (berba: Berba) => {
  berbaToDelete.value = berba
}

const cancelDelete = () => {
  berbaToDelete.value = null
}

const deleteBerba = async () => {
  if (!berbaToDelete.value) return

  try {
    await berbaService.deleteBerba(berbaToDelete.value.idber)
    await loadBerbe()
    berbaToDelete.value = null
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Greška pri brisanju berbe'
    berbaToDelete.value = null
  }
}

onMounted(() => {
  loadBerbe()
})
</script>

<style scoped>
.berbe {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.berbe-container {
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 40px;
}

h1 {
  font-size: 48px;
  font-weight: 700;
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

.berbe-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 24px;
}

.berba-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.3s;
  overflow: hidden;
}

.berba-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.card-header {
  padding: 20px;
  background: linear-gradient(135deg, #1a1a1a 0%, #333 100%);
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

.sezona-badge {
  background: rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(10px);
  padding: 6px 16px;
  border-radius: 12px;
  font-size: 18px;
  font-weight: 700;
}

.card-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
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
  .berbe {
    padding: 40px 20px;
  }

  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  h1 {
    font-size: 36px;
  }

  .berbe-grid {
    grid-template-columns: 1fr;
  }
}
</style>


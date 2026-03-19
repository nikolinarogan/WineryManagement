<template>
  <div class="katalog-container">
    <div class="header">
      <div class="header-content">
        <div>
          <h1>Katalog Sirovina za Tretman</h1>
          <p class="subtitle">Upravljanje sirovinama koje se koriste u tretmanima</p>
        </div>
        <button @click="showCreateModal = true" class="btn-primary">
          Dodaj Novu Sirovinu
        </button>
      </div>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="sirovine.length === 0" class="no-data">
      <p>Katalog sirovina je prazan.</p>
    </div>

    <div v-else class="content">
      <div class="table-card">
        <table class="sirovine-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Naziv Sirovine</th>
              <th>Broj Korišćenja</th>
              <!-- <th>Akcije</th> -->
            </tr>
          </thead>
          <tbody>
            <tr v-for="sirovina in sirovine" :key="sirovina.idsir">
              <td class="id-cell">{{ sirovina.idsir }}</td>
              <td class="naziv-cell">{{ sirovina.naziv }}</td>
              <td>
                <span class="usage-badge" :class="{ 'used': sirovina.brojKoriscenjaUTretmanima > 0 }">
                  {{ sirovina.brojKoriscenjaUTretmanima }} puta
                </span>
              </td>
              <!-- <td>
                <div class="action-buttons">
                  <button @click="editSirovina(sirovina)" class="btn-edit">Izmijeni</button>
                  <button @click="deleteSirovina(sirovina)" class="btn-delete">Obriši</button>
                </div>
              </td> -->
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeModals">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ showEditModal ? 'Izmijeni Sirovinu' : 'Dodaj Novu Sirovinu' }}</h2>
          <button @click="closeModals" class="btn-close">×</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="form-group">
              <label for="naziv">Naziv Sirovine *</label>
              <input
                type="text"
                id="naziv"
                v-model="form.naziv"
                required
                maxlength="100"
                placeholder="npr. Kvasac Saccharomyces, SO2, Enzimi..."
                class="form-control"
              />
            </div>

            <div class="form-actions">
              <button type="button" @click="closeModals" class="btn-secondary">Odustani</button>
              <button type="submit" class="btn-primary" :disabled="submitting">
                {{ submitting ? 'Čuvanje...' : (showEditModal ? 'Sačuvaj Izmjene' : 'Kreiraj') }}
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
import sirovinazatretmanService from '@/services/sirovinazatretmanService'
import type { Sirovinazatretman } from '@/types/sirovinazatretman'

const sirovine = ref<Sirovinazatretman[]>([])
const loading = ref(false)
const error = ref('')
const submitting = ref(false)

const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingSirovina = ref<Sirovinazatretman | null>(null)

const form = ref({
  naziv: ''
})

const loadSirovine = async () => {
  loading.value = true
  error.value = ''
  try {
    sirovine.value = await sirovinazatretmanService.getAllSirovine()
  } catch (err: any) {
    console.error('Greška pri učitavanju sirovina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju sirovina'
  } finally {
    loading.value = false
  }
}

const editSirovina = (sirovina: Sirovinazatretman) => {
  editingSirovina.value = sirovina
  form.value.naziv = sirovina.naziv
  showEditModal.value = true
}

const handleSubmit = async () => {
  submitting.value = true
  try {
    if (showEditModal.value && editingSirovina.value) {
      // Ažuriranje
      await sirovinazatretmanService.updateSirovina(editingSirovina.value.idsir, {
        naziv: form.value.naziv
      })
    } else {
      // Kreiranje
      await sirovinazatretmanService.createSirovina({
        naziv: form.value.naziv
      })
    }
    closeModals()
    await loadSirovine()
  } catch (err: any) {
    console.error('Greška pri čuvanju sirovine:', err)
    alert(err.response?.data?.message || 'Greška pri čuvanju sirovine')
  } finally {
    submitting.value = false
  }
}

const deleteSirovina = async (sirovina: Sirovinazatretman) => {
  if (!confirm(`Da li ste sigurni da želite obrisati sirovinu "${sirovina.naziv}"?`)) {
    return
  }

  try {
    await sirovinazatretmanService.deleteSirovina(sirovina.idsir)
    alert('Sirovina uspješno obrisana!')
    await loadSirovine()
  } catch (err: any) {
    console.error('Greška pri brisanju sirovine:', err)
    alert(err.response?.data?.message || 'Greška pri brisanju sirovine')
  }
}

const closeModals = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingSirovina.value = null
  form.value.naziv = ''
}

onMounted(() => {
  loadSirovine()
})
</script>

<style scoped>
.katalog-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  margin-bottom: 30px;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
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

.btn-primary {
  background: #000;
  color: #fff;
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
}

.loading {
  text-align: center;
  padding: 40px;
  font-size: 18px;
  color: #666;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.no-data {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.table-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
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
  padding: 16px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.sirovine-table th {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 700;
}

.sirovine-table td {
  font-size: 14px;
  color: #000;
}

.sirovine-table tbody tr:hover {
  background: #f9f9f9;
}

.id-cell {
  color: #666;
  font-weight: 600;
}

.naziv-cell {
  font-weight: 600;
}

.usage-badge {
  display: inline-block;
  padding: 4px 12px;
  background: #e0e0e0;
  color: #666;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.usage-badge.used {
  background: #c8e6c9;
  color: #2e7d32;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn-edit,
.btn-delete {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-edit {
  background: #e3f2fd;
  color: #1976d2;
}

.btn-edit:hover {
  background: #1976d2;
  color: #fff;
}

.btn-delete {
  background: #ffebee;
  color: #c62828;
}

.btn-delete:hover {
  background: #c62828;
  color: #fff;
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


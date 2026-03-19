<template>
  <div class="create-vinograd">
    <div class="create-container">
      <h1>Kreiraj novi vinograd</h1>

      <form @submit.prevent="handleSubmit">
        <!-- Osnovni podaci vinograda -->
        <div class="form-section">
          <h2>Osnovni podaci</h2>
          
          <div class="form-row">
            <div class="form-group">
              <label for="naziv">Naziv *</label>
              <input
                id="naziv"
                v-model="form.naziv"
                type="text"
                required
                placeholder="npr. Vinograd Sremski Karlovci"
              />
            </div>

            <div class="form-group">
              <label for="datosn">Datum osnivanja *</label>
              <input
                id="datosn"
                v-model="form.datosn"
                type="date"
                required
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="povrsina">Površina (ha) *</label>
              <input
                id="povrsina"
                v-model.number="form.povrsina"
                type="number"
                step="0.01"
                required
                placeholder="npr. 15.5"
              />
            </div>

            <div class="form-group">
              <label for="nadmorskavis">Nadmorska visina (m) *</label>
              <input
                id="nadmorskavis"
                v-model.number="form.nadmorskavis"
                type="number"
                required
                placeholder="npr. 250"
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="tipzemljista">Tip zemljišta *</label>
              <select id="tipzemljista" v-model="form.tipzemljista" required>
                <option value="">Izaberite tip</option>
                <option value="Černozem">Černozem</option>
                <option value="Krečnjak">Krečnjak</option>
                <option value="Pjeskovito">Pjeskovito</option>
                <option value="Glineno">Glineno</option>
                <option value="Mješovito">Mješovito</option>
              </select>
            </div>

            <div class="form-group">
              <label for="navodnjavanje">Navodnjavanje *</label>
              <select id="navodnjavanje" v-model="form.navodnjavanje" required>
                <option value="">Izaberite opciju</option>
                <option value="D">Da</option>
                <option value="N">Ne</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Parcele -->
        <div class="form-section">
          <div class="section-header">
            <h2>Parcele</h2>
            <button type="button" @click="openAddParcelaModal" class="btn-add">
              + Dodaj parcelu
            </button>
          </div>

          <div v-if="form.parcele.length === 0" class="no-parcele">
            <p>Nijedna parcela nije dodata. Kliknite "Dodaj Parcelu" da dodate parcele.</p>
          </div>

          <div v-else class="parcele-list">
            <div v-for="(parcela, index) in form.parcele" :key="index" class="parcela-item">
              <div class="parcela-info">
                <h4>{{ parcela.nazivparcele }}</h4>
                <div class="parcela-details">
                  <span>{{ parcela.brojcokota }} čokota</span>
                  <span>{{ parcela.povrsina }} ha</span>
                  <span v-if="parcela.sortagrozdjaIdsrt">{{ getSortaName(parcela.sortagrozdjaIdsrt) }}</span>
                  <span v-else class="no-sorta">Sorta nije dodijeljeno</span>
                </div>
              </div>
              <button type="button" @click="removeParcela(index)" class="btn-remove-small">
                ✕
              </button>
            </div>
          </div>
        </div>

        <!-- Buttons -->
        <div class="form-actions">
          <router-link to="/vinogradi" class="btn-secondary">
            Odustani
          </router-link>
          <button type="submit" class="btn-primary" :disabled="loading">
            {{ loading ? 'Kreiranje...' : 'Kreiraj vinograd' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
      </form>
    </div>

    <!-- Add Parcela Modal -->
    <div v-if="showAddParcelaModal" class="modal-overlay" @click="closeAddParcelaModal">
      <div class="modal" @click.stop>
        <h3>Dodaj Parcelu</h3>
        <div class="form-group">
          <label for="new-nazivparcele">Naziv parcele *</label>
          <input
            id="new-nazivparcele"
            v-model="newParcela.nazivparcele"
            type="text"
            required
            placeholder="npr. Parcela A1"
          />
        </div>

        <div class="form-group">
          <label for="new-brojcokota">Broj čokota *</label>
          <input
            id="new-brojcokota"
            v-model.number="newParcela.brojcokota"
            type="number"
            required
            placeholder="npr. 150"
          />
        </div>

        <div class="form-group">
          <label for="new-povrsina">Površina (ha) *</label>
          <input
            id="new-povrsina"
            v-model.number="newParcela.povrsina"
            type="number"
            step="0.01"
            required
            placeholder="npr. 2.5"
          />
        </div>

        <div class="form-group">
          <label for="new-sorta">Sorta grožđa</label>
          <select id="new-sorta" v-model="newParcela.sortagrozdjaIdsrt">
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
          <button type="button" @click="addParcela" class="btn-primary">
            Dodaj
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import vinogradService from '@/services/vinogradService'
import sortagrozdjaService from '@/services/sortagrozdjaService'
import type { CreateVinogradRequest, CreateParcelaRequest } from '@/types/vinograd'
import type { Sortagrozdja } from '@/types/sortagrozdja'

const router = useRouter()
const loading = ref(false)
const error = ref('')
const sorte = ref<Sortagrozdja[]>([])
const showAddParcelaModal = ref(false)

const form = ref<CreateVinogradRequest>({
  naziv: '',
  datosn: '',
  povrsina: 0,
  nadmorskavis: 0,
  tipzemljista: '',
  navodnjavanje: '',
  parcele: []
})

const newParcela = ref<CreateParcelaRequest>({
  nazivparcele: '',
  brojcokota: 0,
  povrsina: 0
})

const loadSorte = async () => {
  try {
    sorte.value = await sortagrozdjaService.getAllSorte()
  } catch (err) {
    console.error('Error loading sorte:', err)
  }
}

const openAddParcelaModal = () => {
  newParcela.value = {
    nazivparcele: '',
    brojcokota: 0,
    povrsina: 0
  }
  showAddParcelaModal.value = true
}

const closeAddParcelaModal = () => {
  showAddParcelaModal.value = false
}

const addParcela = () => {
  if (!newParcela.value.nazivparcele || !newParcela.value.brojcokota || !newParcela.value.povrsina) {
    alert('Molimo popunite sva obavezna polja')
    return
  }

  if (!form.value.parcele) {
    form.value.parcele = []
  }
  
  form.value.parcele.push({ ...newParcela.value })
  closeAddParcelaModal()
}

const getSortaName = (sortaId: number) => {
  const sorta = sorte.value.find(s => s.idsrt === sortaId)
  return sorta ? `${sorta.nazivsorte} (${sorta.bojasorte})` : ''
}

onMounted(() => {
  loadSorte()
})

const removeParcela = (index: number) => {
  form.value.parcele?.splice(index, 1)
}

const handleSubmit = async () => {
  try {
    loading.value = true
    error.value = ''
    
    await vinogradService.createVinograd(form.value)
    router.push('/vinogradi')
  } catch (err: any) {
    console.error('Error creating vinograd:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju vinograda'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.create-vinograd {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.create-container {
  max-width: 900px;
  margin: 0 auto;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 40px 0;
}

.form-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
}

.form-section h2 {
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

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
  font-size: 14px;
}

.form-group input,
.form-group select {
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

.btn-add {
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

.btn-add:hover {
  background: #333;
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

.parcele-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.parcela-item {
  background: #f9f9f9;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: all 0.3s;
}

.parcela-item:hover {
  border-color: #000;
}

.parcela-info h4 {
  margin: 0 0 8px 0;
  font-size: 16px;
  font-weight: 600;
  color: #000;
}

.parcela-details {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

.parcela-details span {
  font-size: 14px;
  color: #666;
}

.parcela-details .no-sorta {
  font-style: italic;
}

.btn-remove-small {
  width: 28px;
  height: 28px;
  background: #c00;
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.btn-remove-small:hover {
  background: #a00;
  transform: scale(1.1);
}

/* Modal */
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
  margin: 0 0 24px 0;
  font-size: 24px;
  color: #000;
}

.modal .form-group {
  margin-bottom: 20px;
}

.modal .form-group small {
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

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 32px;
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

.btn-secondary {
  padding: 12px 24px;
  background: #f0f0f0;
  color: #000;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.error-message {
  margin-top: 16px;
  padding: 12px;
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  font-size: 14px;
  text-align: center;
}

@media (max-width: 768px) {
  .create-vinograd {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .form-section {
    padding: 24px;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .form-actions {
    flex-direction: column-reverse;
  }
}
</style>


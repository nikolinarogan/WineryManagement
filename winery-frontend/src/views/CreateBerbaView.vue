<template>
  <div class="create-berba">
    <div class="create-container">
      <h1>Kreiraj novu berbu</h1>

      <form @submit.prevent="handleSubmit">
        <!-- Osnovni podaci berbe -->
        <div class="form-section">
          <h2>Osnovni podaci</h2>
          
          <div class="form-row">
            <div class="form-group">
              <label for="nazber">Naziv berbe *</label>
              <input
                id="nazber"
                v-model="form.nazber"
                type="text"
                required
                placeholder="npr. Jesenja berba 2024"
              />
            </div>

            <div class="form-group">
              <label for="sezona">Sezona (godina) *</label>
              <input
                id="sezona"
                v-model.number="form.sezona"
                type="number"
                required
                :min="currentYear - 10"
                :max="currentYear + 1"
                placeholder="npr. 2024"
              />
            </div>
          </div>
        </div>

        <!-- Parcele -->
        <div class="form-section">
          <div class="section-header">
            <h2>Izbor parcela za berbu</h2>
            <button type="button" @click="toggleSelectAll" class="btn-secondary">
              {{ allSelected ? 'Poništi sve' : 'Izaberi sve' }}
            </button>
          </div>

          <div v-if="loadingParcele" class="loading">Učitavanje parcela...</div>

          <div v-else-if="vinogradi.length === 0" class="no-parcele">
            <p>Nema dostupnih vinograda sa parcelama.</p>
          </div>

          <div v-else class="vinogradi-list">
            <div v-for="vinograd in vinogradi" :key="vinograd.idv" class="vinograd-group">
              <h3 class="vinograd-name">{{ vinograd.naziv }}</h3>
              
              <div class="parcele-checklist">
                <label
                  v-for="parcela in vinograd.parcele"
                  :key="parcela.idp"
                  class="parcela-checkbox"
                  :class="{ selected: isParcelaSelected(parcela.idp) }"
                >
                  <input
                    type="checkbox"
                    :value="parcela.idp"
                    v-model="form.parcelaIds"
                  />
                  <div class="parcela-info">
                    <div class="parcela-header">
                      <strong>{{ parcela.nazivparcele }}</strong>
                      <span v-if="parcela.sortaNaziv" class="sorta-tag">
                        {{ parcela.sortaNaziv }}
                      </span>
                    </div>
                    <div class="parcela-details">
                      <span>{{ parcela.povrsina }} ha</span>
                      <span>{{ parcela.brojcokota }} čokota</span>
                    </div>
                  </div>
                </label>
              </div>
            </div>
          </div>

          <div v-if="form.parcelaIds.length > 0" class="selection-summary">
            Izabrano: <strong>{{ form.parcelaIds.length }}</strong> {{ form.parcelaIds.length === 1 ? 'parcela' : 'parcela' }}
          </div>
        </div>

        <!-- Buttons -->
        <div class="form-actions">
          <router-link to="/berbe" class="btn-secondary">
            Odustani
          </router-link>
          <button type="submit" class="btn-primary" :disabled="loading || form.parcelaIds.length === 0">
            {{ loading ? 'Kreiranje...' : 'Kreiraj berbu' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import berbaService from '@/services/berbaService'
import vinogradService from '@/services/vinogradService'
import type { CreateBerbaRequest } from '@/types/berba'
import type { VinogradDetail } from '@/types/vinograd'

const router = useRouter()
const loading = ref(false)
const loadingParcele = ref(false)
const error = ref('')
const vinogradi = ref<VinogradDetail[]>([])

const currentYear = new Date().getFullYear()

const form = ref<CreateBerbaRequest>({
  nazber: '',
  sezona: currentYear,
  parcelaIds: []
})

const allSelected = computed(() => {
  const totalParcele = vinogradi.value.reduce((sum, v) => sum + v.parcele.length, 0)
  return form.value.parcelaIds.length === totalParcele && totalParcele > 0
})

const loadVinogradi = async () => {
  try {
    loadingParcele.value = true
    const allVinogradi = await vinogradService.getAllVinogradi()
    
    // Učitaj detalje svakog vinograda sa parcelama
    const vinogradiWithParcele = await Promise.all(
      allVinogradi.map(v => vinogradService.getVinogradById(v.idv))
    )
    
    // Filtriraj samo vinogrede koji imaju parcele
    vinogradi.value = vinogradiWithParcele.filter(v => v && v.parcele.length > 0) as VinogradDetail[]
  } catch (err) {
    console.error('Error loading vinogradi:', err)
    error.value = 'Greška pri učitavanju vinograda'
  } finally {
    loadingParcele.value = false
  }
}

const isParcelaSelected = (parcelaId: number) => {
  return form.value.parcelaIds.includes(parcelaId)
}

const toggleSelectAll = () => {
  if (allSelected.value) {
    form.value.parcelaIds = []
  } else {
    form.value.parcelaIds = vinogradi.value.flatMap(v => v.parcele.map(p => p.idp))
  }
}

const handleSubmit = async () => {
  if (form.value.parcelaIds.length === 0) {
    error.value = 'Morate izabrati barem jednu parcelu'
    return
  }

  try {
    loading.value = true
    error.value = ''
    
    await berbaService.createBerba(form.value)
    router.push('/berbe')
  } catch (err: any) {
    console.error('Error creating berba:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju berbe'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadVinogradi()
})
</script>

<style scoped>
.create-berba {
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

.form-group input {
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #000;
}

.loading {
  text-align: center;
  padding: 40px;
  color: #666;
  font-size: 16px;
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

.vinogradi-list {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.vinograd-group {
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  background: #f9f9f9;
}

.vinograd-name {
  margin: 0 0 16px 0;
  font-size: 20px;
  font-weight: 600;
  color: #000;
}

.parcele-checklist {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 12px;
}

.parcela-checkbox {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px;
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
}

.parcela-checkbox:hover {
  border-color: #000;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.parcela-checkbox.selected {
  border-color: #000;
  background: #f0f0f0;
}

.parcela-checkbox input[type="checkbox"] {
  margin-top: 4px;
  cursor: pointer;
  width: 18px;
  height: 18px;
}

.parcela-info {
  flex: 1;
}

.parcela-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.parcela-header strong {
  font-size: 15px;
  color: #000;
}

.sorta-tag {
  background: #000;
  color: white;
  padding: 3px 10px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}

.parcela-details {
  display: flex;
  gap: 16px;
  font-size: 13px;
  color: #666;
}

.selection-summary {
  margin-top: 20px;
  padding: 16px;
  background: #000;
  color: white;
  border-radius: 8px;
  text-align: center;
  font-size: 16px;
}

.selection-summary strong {
  font-size: 20px;
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
  .create-berba {
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

  .parcele-checklist {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column-reverse;
  }
}
</style>


<template>
  <div class="create-radovi">
    <div class="container">
      <div class="header">
        <router-link to="/radovi" class="back-link">← Nazad na radove</router-link>
        <h1>Kreiraj Novi Rad</h1>
      </div>

      <form @submit.prevent="createRad" class="rad-form">
        <div class="form-section">
          <h2>Osnovne Informacije</h2>
          
          <div class="form-row">
            <div class="form-group">
              <label for="pocrad">Početak Rada *</label>
              <input id="pocrad" v-model="form.pocrad" type="date" required />
            </div>
            
            <div class="form-group">
              <label for="zavrrad">Završetak Rada *</label>
              <input id="zavrrad" v-model="form.zavrrad" type="date" required />
            </div>
          </div>

          <div class="form-group">
            <label for="oprema">Oprema *</label>
            <input id="oprema" v-model="form.oprema" type="text" required placeholder="npr. Traktor, motika, sekator..." />
          </div>
        </div>

        <div class="form-section">
          <h2>Parcele</h2>
          <p class="section-description">Izaberite parcele na kojima će se odvijati rad</p>
          
          <div v-if="loadingParcele" class="loading">Učitavanje parcela...</div>
          
          <div v-else class="parcele-grid">
            <label
              v-for="parcela in parcele"
              :key="parcela.idp"
              class="parcela-checkbox"
            >
              <input
                type="checkbox"
                :value="parcela.idp"
                v-model="form.parcelaIds"
              />
              <span class="checkbox-label">
                <strong>{{ parcela.nazivparcele }}</strong> ({{ parcela.vinogradNaziv }})
              </span>
            </label>
          </div>
        </div>

        <div class="form-actions">
          <button type="button" @click="$router.push('/radovi')" class="btn-secondary">
            Odustani
          </button>
          <button type="submit" class="btn-primary" :disabled="creating">
            {{ creating ? 'Kreiranje...' : 'Kreiraj Rad' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import radoviService from '@/services/radoviService'
import vinogradService from '@/services/vinogradService'

const router = useRouter()

const form = ref({
  pocrad: '',
  zavrrad: '',
  oprema: '',
  parcelaIds: [] as number[]
})

const parcele = ref<any[]>([])
const loadingParcele = ref(false)
const creating = ref(false)
const error = ref('')

const loadParcele = async () => {
  try {
    loadingParcele.value = true
    const vinogradi = await vinogradService.getAllVinogradiWithParcele()
    parcele.value = vinogradi.flatMap(v => 
      v.parcele.map(p => ({
        idp: p.idp,
        nazivparcele: p.nazivparcele,
        vinogradNaziv: v.naziv
      }))
    )
  } catch (err) {
    console.error('Error loading parcele:', err)
  } finally {
    loadingParcele.value = false
  }
}

const createRad = async () => {
  try {
    creating.value = true
    error.value = ''
    
    if (form.value.parcelaIds.length === 0) {
      error.value = 'Molimo izaberite bar jednu parcelu'
      return
    }

    await radoviService.createRadovi(form.value)
    router.push('/radovi')
  } catch (err: any) {
    console.error('Error creating rad:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju rada'
  } finally {
    creating.value = false
  }
}

onMounted(() => {
  loadParcele()
})
</script>

<style scoped>
.create-radovi {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
}

.container {
  max-width: 900px;
  margin: 0 auto;
}

.header {
  margin-bottom: 32px;
}

.back-link {
  display: inline-block;
  color: #000;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 16px;
  transition: color 0.3s;
}

.back-link:hover {
  color: #666;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.rad-form {
  background: white;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-section {
  margin-bottom: 32px;
}

.form-section h2 {
  font-size: 24px;
  font-weight: 600;
  color: #000;
  margin: 0 0 8px 0;
}

.section-description {
  color: #666;
  font-size: 14px;
  margin: 0 0 20px 0;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

.form-group input {
  padding: 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 16px;
  transition: all 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #000;
}

.parcele-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 12px;
}

.parcela-checkbox {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
}

.parcela-checkbox:hover {
  border-color: #000;
  background: #f9f9f9;
}

.parcela-checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.checkbox-label {
  font-size: 15px;
  color: #333;
}

.form-actions {
  display: flex;
  gap: 16px;
  justify-content: flex-end;
  margin-top: 32px;
}

.btn-primary,
.btn-secondary {
  padding: 12px 24px;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-primary {
  background: #000;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: #f0f0f0;
  color: #000;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.error-message {
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  padding: 16px;
  margin-top: 20px;
}

.loading {
  text-align: center;
  padding: 20px;
  color: #666;
}

@media (max-width: 768px) {
  .create-radovi {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .rad-form {
    padding: 24px;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .parcele-grid {
    grid-template-columns: 1fr;
  }
}
</style>


<template>
  <div class="punjenje-boca-container">
    <div class="header">
      <h1>Punjenje Boca</h1>
      <p class="subtitle">Napunite boce finalnim vinom i smestite ih u magacin</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else class="content">
      <form @submit.prevent="handlePunjenje" class="punjenje-form">
        <div class="form-section">
          <h2>Izbor Vina</h2>

          <div class="form-group">
            <label for="vino">Finalno Vino *</label>
            <select id="vino" v-model.number="form.vinoIdvina" required>
              <option value="">Izaberite vino</option>
              <option v-for="vino in vina" :key="vino.idvina" :value="vino.idvina">
                {{ vino.nazivvina }} ({{ vino.tipvina }})
              </option>
            </select>
          </div>
        </div>

        <div class="form-section">
          <h2>Detalji Punjenja</h2>

          <div class="form-row">
            <div class="form-group">
              <label for="brojBoca">Broj Boca *</label>
              <input
                id="brojBoca"
                v-model.number="form.brojBoca"
                type="number"
                min="1"
                required
                placeholder="npr. 100"
              />
              <small>Koliko boca želite da napunite</small>
            </div>

            <div class="form-group">
              <label for="zapremina">Zapremina Boce *</label>
              <select id="zapremina" v-model.number="form.zapremina" required>
                <option value="">Izaberite zapreminu</option>
                <option :value="0.375">0.375 L (mala boca)</option>
                <option :value="0.75">0.75 L (standardna)</option>
                <option :value="1.5">1.5 L (magnum)</option>
                <option :value="3.0">3.0 L (jeroboam)</option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="cena">Cena po Boci (opciono)</label>
              <input
                id="cena"
                v-model.number="form.cena"
                type="number"
                step="0.01"
                min="0"
                placeholder="npr. 25.50"
              />
              <small>Unesite cenu u RSD</small>
            </div>

            <div class="form-group">
              <label for="magacin">Magacin za Skladištenje *</label>
              <select id="magacin" v-model.number="form.magacinIdmag" required>
                <option value="">Izaberite magacin</option>
                <option 
                  v-for="magacin in magacini" 
                  :key="magacin.idmag" 
                  :value="magacin.idmag"
                >
                  {{ magacin.nazivmag }} 
                  ({{ magacin.brojBoca }}/{{ magacin.kapacitetmag }} boca)
                </option>
              </select>
              <small>Prikazana je trenutna popunjenost</small>
            </div>
          </div>
        </div>

        <div class="form-actions">
          <router-link to="/boca-inventar" class="btn-secondary">Odustani</router-link>
          <button type="submit" class="btn-primary" :disabled="submitting">
            {{ submitting ? 'Punim...' : 'Napuni Boce' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
        <div v-if="success" class="success-message">{{ success }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import bocaService from '@/services/bocaService'
import vinoService from '@/services/vinoService'
import magacinService from '@/services/magacinService'
import type { Vino } from '@/types/vino'
import type { Magacin } from '@/types/magacin'

const router = useRouter()

const loading = ref(false)
const submitting = ref(false)
const error = ref('')
const success = ref('')

const vina = ref<Vino[]>([])
const magacini = ref<Magacin[]>([])

const form = ref({
  vinoIdvina: 0,
  magacinIdmag: 0,
  brojBoca: 1,
  zapremina: 0.75,
  cena: null as number | null
})

const loadData = async () => {
  loading.value = true
  error.value = ''
  try {
    console.log('Učitavam vina i magacine...')
    
    const [vinaData, magaciniData] = await Promise.all([
      vinoService.getAllVina(),
      magacinService.getAllMagacini()
    ])

    console.log('Vina:', vinaData)
    console.log('Magacini:', magaciniData)

    vina.value = vinaData
    magacini.value = magaciniData
  } catch (err: any) {
    console.error('Greška pri učitavanju podataka:', err)
    console.error('Response:', err.response)
    console.error('Status:', err.response?.status)
    console.error('Data:', err.response?.data)
    
    if (err.response?.status === 401 || err.response?.status === 403) {
      error.value = 'Nemate pristup ovim podacima'
    } else {
      error.value = err.response?.data?.message || err.message || 'Greška pri učitavanju podataka'
    }
  } finally {
    loading.value = false
  }
}

const handlePunjenje = async () => {
  submitting.value = true
  error.value = ''
  success.value = ''

  try {
    const result = await bocaService.punjenjeBoca({
      ...form.value,
      cena: form.value.cena || null
    })

    success.value = `Uspješno napunjeno ${result.brojKreiranihBoca} boca vina "${result.nazivVina}" u magacin "${result.nazivMagacina}"!`

    // Reset forme
    form.value = {
      vinoIdvina: 0,
      magacinIdmag: 0,
      brojBoca: 1,
      zapremina: 0.75,
      cena: null
    }

    // Pauza pa redirect
    setTimeout(() => {
      router.push('/boca-inventar')
    }, 2000)
  } catch (err: any) {
    console.error('Greška pri punjenju boca:', err)
    error.value = err.response?.data?.message || 'Greška pri punjenju boca'
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.punjenje-boca-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  margin-bottom: 30px;
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

.loading {
  text-align: center;
  padding: 40px;
}

.punjenje-form {
  background: #fff;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-section {
  margin-bottom: 32px;
  padding-bottom: 32px;
  border-bottom: 2px solid #f0f0f0;
}

.form-section:last-of-type {
  border-bottom: none;
}

.form-section h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin-bottom: 16px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
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

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.btn-secondary,
.btn-primary {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: inline-block;
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

.error-message {
  margin-top: 20px;
  padding: 12px;
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
  text-align: center;
}

.success-message {
  margin-top: 20px;
  padding: 12px;
  background: #e8f5e9;
  color: #2e7d32;
  border-radius: 8px;
  text-align: center;
}
</style>


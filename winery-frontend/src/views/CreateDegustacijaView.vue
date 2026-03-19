<template>
  <div class="create-degustacija-container">
    <div class="header">
      <h1>Kreiraj novu degustaciju</h1>
      <p class="subtitle">Organizujte degustaciju vina za vaše goste</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else class="content">
      <form @submit.prevent="handleCreate" class="degustacija-form">
        <div class="form-section">
          <h2>Osnovne informacije</h2>

          <div class="form-group">
            <label for="naziv">Naziv degustacije *</label>
            <input
              id="naziv"
              v-model="form.nazivdeg"
              type="text"
              required
              placeholder="npr. Letnja degustacija crvenih vina"
            />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="datum">Datum degustacije *</label>
              <input
                id="datum"
                v-model="form.datdeg"
                type="date"
                required
                :min="minDate"
              />
              <small>Datum mora biti u budućnosti</small>
            </div>

            <div class="form-group">
              <label for="kapacitet">Kapacitet (broj osoba) *</label>
              <input
                id="kapacitet"
                v-model.number="form.kapacitetdeg"
                type="number"
                min="1"
                required
                placeholder="npr. 20"
              />
            </div>
          </div>
        </div>

        <div class="form-section">
          <h2>Izbor vina</h2>
          <p class="section-note">Izaberite vina koja će biti prezentovana na degustaciji</p>

          <div v-if="loadingVina" class="loading-small">Učitavanje vina...</div>
          
          <div v-else-if="vina.length === 0" class="no-vina">
            Nema dostupnih vina za degustaciju.
          </div>

          <div v-else class="vina-selection">
            <div v-for="vino in vina" :key="vino.idvina" class="vino-item">
              <label class="checkbox-label">
                <input
                  type="checkbox"
                  :value="vino.idvina"
                  v-model="form.vinaIds"
                />
                <div class="vino-info">
                  <strong>{{ vino.nazivvina }}</strong>
                  <span class="tip-badge" :class="getTipClass(vino.tipvina)">
                    {{ vino.tipvina }}
                  </span>
                </div>
              </label>
            </div>
          </div>

          <div v-if="form.vinaIds.length > 0" class="selected-count">
            Izabrano: <strong>{{ form.vinaIds.length }}</strong> vina
          </div>
        </div>

        <div class="form-actions">
          <router-link to="/degustacije" class="btn-secondary">Odustani</router-link>
          <button type="submit" class="btn-primary" :disabled="submitting || form.vinaIds.length === 0">
            {{ submitting ? 'Kreiranje...' : 'Kreiraj degustaciju' }}
          </button>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import degustacijaService from '@/services/degustacijaService'
import vinoService from '@/services/vinoService'
import type { Vino } from '@/types/vino'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const loadingVina = ref(false)
const submitting = ref(false)
const error = ref('')

const vina = ref<Vino[]>([])

const form = ref({
  nazivdeg: '',
  datdeg: '',
  kapacitetdeg: 20,
  vinaIds: [] as number[]
})

const minDate = computed(() => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow.toISOString().split('T')[0]
})

const loadVina = async () => {
  loadingVina.value = true
  try {
    vina.value = await vinoService.getAllVina()
  } catch (err: any) {
    console.error('Greška pri učitavanju vina:', err)
    error.value = 'Greška pri učitavanju vina'
  } finally {
    loadingVina.value = false
  }
}

const getTipClass = (tip: string) => {
  if (tip === 'Crveno') return 'tip-crveno'
  if (tip === 'Belo') return 'tip-belo'
  if (tip === 'Roze') return 'tip-roze'
  return ''
}

const handleCreate = async () => {
  if (form.value.vinaIds.length === 0) {
    error.value = 'Morate izabrati barem jedno vino.'
    return
  }

  submitting.value = true
  error.value = ''

  try {
    const somleijerIdzap = authStore.userId

    if (!somleijerIdzap) {
      error.value = 'Nije moguće identifikovati somelijera.'
      return
    }

    await degustacijaService.createDegustacija({
      ...form.value,
      somleijerIdzap
    })

    router.push('/degustacije')
  } catch (err: any) {
    console.error('Greška pri kreiranju degustacije:', err)
    error.value = err.response?.data?.message || 'Greška pri kreiranju degustacije'
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadVina()
})
</script>

<style scoped>
.create-degustacija-container {
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

.loading,
.loading-small {
  text-align: center;
  padding: 40px;
  color: #666;
}

.loading-small {
  padding: 20px;
}

.degustacija-form {
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
  margin-bottom: 8px;
}

.section-note {
  font-size: 14px;
  color: #666;
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

.form-group input[type="text"],
.form-group input[type="date"],
.form-group input[type="number"] {
  width: 100%;
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

.form-group small {
  display: block;
  margin-top: 4px;
  font-size: 12px;
  color: #666;
}

.no-vina {
  text-align: center;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 8px;
  color: #666;
}

.vina-selection {
  display: grid;
  gap: 12px;
}

.vino-item {
  background: #f9f9f9;
  border-radius: 8px;
  padding: 12px;
  transition: all 0.3s;
}

.vino-item:hover {
  background: #f0f0f0;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.vino-info {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.vino-info strong {
  font-size: 15px;
  color: #000;
}

.tip-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 700;
}

.tip-badge.tip-crveno {
  background: #ffebee;
  color: #c62828;
}

.tip-badge.tip-belo {
  background: #fff9c4;
  color: #f57f17;
}

.tip-badge.tip-roze {
  background: #fce4ec;
  color: #c2185b;
}

.selected-count {
  margin-top: 16px;
  padding: 12px;
  background: #e8f5e9;
  color: #2e7d32;
  border-radius: 8px;
  text-align: center;
  font-size: 14px;
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
</style>


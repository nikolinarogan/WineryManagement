<template>
  <div class="create-prijem-container">
    <div class="header">
      <button @click="goBack" class="btn-back">← Nazad</button>
      <h1>Kreiranje prijema sirovina</h1>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="details" class="content">
      <!-- Info Section -->
      <div class="info-card">
        <h2>Informacije o rasporedu</h2>
        
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Berba:</span>
            <span class="value">{{ details.berbaNaziv }}</span>
          </div>
          <div class="info-item">
            <span class="label">Lokacija:</span>
            <span class="value">{{ details.vinogradNaziv }} - {{ details.parcelaNaziv }}</span>
          </div>
          <div class="info-item" v-if="details.sortaNaziv">
            <span class="label">Sorta:</span>
            <span class="value">{{ details.sortaNaziv }}</span>
          </div>
          <div class="info-item">
            <span class="label">Period:</span>
            <span class="value">
              {{ formatDate(details.pocetakBranja) }} - {{ formatDate(details.zavrsetakBranja) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Statistika Section -->
      <div class="stats-card">
        <h2>Realizacija branja</h2>
        
        <div class="stats-grid">
          <div class="stat-box">
            <div class="stat-label">Očekivani prinos</div>
            <div class="stat-value">{{ details.ocekivaniPrinos.toFixed(2) }} kg</div>
          </div>
          <div class="stat-box">
            <div class="stat-label">Ukupno ubrano</div>
            <div class="stat-value highlight">{{ details.ukupnoUbrano.toFixed(2) }} kg</div>
          </div>
          <div class="stat-box">
            <div class="stat-label">Realizacija</div>
            <div class="stat-value" :class="getRealizacijaClass(details.procenatRealizacije)">
              {{ details.procenatRealizacije.toFixed(1) }}%
            </div>
          </div>
          <div class="stat-box">
            <div class="stat-label">Prosek po radniku</div>
            <div class="stat-value">{{ details.prosekPoRadniku.toFixed(2) }} kg</div>
          </div>
        </div>
      </div>

      <!-- Radnici Table -->
      <div class="radnici-card">
        <h2>Radnici i količine</h2>
        
        <div class="table-wrapper">
          <table class="radnici-table">
            <thead>
              <tr>
                <th>Radnik</th>
                <th>Količina (kg)</th>
                <th>Datum</th>
                <th>Odstupanje od prosjeka</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="radnik in details.radnici" 
                :key="radnik.radnikId"
                :class="{ 'outlier-row': radnik.isOutlier }"
              >
                <td>{{ radnik.ime }} {{ radnik.prezime }}</td>
                <td class="bold">{{ radnik.kolicina.toFixed(2) }} kg</td>
                <td>{{ formatDate(radnik.datum) }}</td>
                <td :class="getOdstupostotakClass(radnik.odstupostotakOdProseka)">
                  {{ radnik.odstupostotakOdProseka > 0 ? '+' : '' }}{{ radnik.odstupostotakOdProseka.toFixed(1) }}%
                  <span v-if="radnik.isOutlier" class="warning-badge">⚠️</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="details.brojAnomalija > 0" class="anomalije-section">
          <h3>⚠️ Detektovane anomalije ({{ details.brojAnomalija }})</h3>
          <div v-for="anomalija in details.anomalije" :key="anomalija.radnikId" class="anomalija-item">
            <strong>{{ anomalija.ime }} {{ anomalija.prezime }}</strong>: {{ anomalija.poruka }}
          </div>
        </div>
      </div>

      <!-- Forma za Prijem -->
      <div class="prijem-form-card">
        <h2>Podaci o prijemu</h2>
        
        <div class="form-section">
          <form @submit.prevent="handleSubmit" class="prijem-form">
            <div class="form-group">
              <label for="kolicina">Primljena količina (kg) *</label>
              <input
                type="number"
                id="kolicina"
                v-model.number="form.kolicina"
                step="0.01"
                min="0"
                required
                class="form-control"
              />
              <small class="form-hint">
                Unesite stvarnu količinu primljenih sirovina
              </small>
            </div>

            <div class="form-group">
              <label for="datprijema">Datum prijema *</label>
              <input
                type="date"
                id="datprijema"
                v-model="form.datprijema"
                required
                class="form-control"
              />
            </div>

            <div class="form-actions">
              <button type="button" @click="goBack" class="btn-secondary">Odustani</button>
              <button 
                type="submit" 
                class="btn-primary"
                :disabled="submitting"
              >
                {{ submitting ? 'Kreiranje...' : 'Potvrdi prijem' }}
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
import { useRoute, useRouter } from 'vue-router'
import ubranasirovinaService from '@/services/ubranasirovinaService'
import type { PrijemDetails } from '@/types/ubranasirovina'

const route = useRoute()
const router = useRouter()

const rasporedId = parseInt(route.params.id as string)

const details = ref<PrijemDetails | null>(null)
const loading = ref(false)
const error = ref('')
const submitting = ref(false)

const form = ref({
  kolicina: 0,
  datprijema: new Date().toISOString().split('T')[0]
})

const loadDetails = async () => {
  loading.value = true
  error.value = ''
  try {
    details.value = await ubranasirovinaService.getPrijemDetails(rasporedId)
    // Postavi preporučenu količinu kao default
    form.value.kolicina = details.value.preporucenaKolicina
  } catch (err: any) {
    console.error('Greška pri učitavanju detalja:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju detalja'
  } finally {
    loading.value = false
  }
}

const handleSubmit = async () => {
  submitting.value = true
  try {
    await ubranasirovinaService.createPrijem({
      kolicina: form.value.kolicina,
      datprijema: form.value.datprijema,
      rasporedbranjaIdras: rasporedId
    })

    router.push('/prijem-sirovina')
  } catch (err: any) {
    console.error('Greška pri kreiranju prijema:', err)
    alert(err.response?.data?.message || 'Greška pri kreiranju prijema')
  } finally {
    submitting.value = false
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const getRealizacijaClass = (procenat: number) => {
  if (procenat >= 90) return 'realizacija-odlicna'
  if (procenat >= 70) return 'realizacija-dobra'
  if (procenat >= 50) return 'realizacija-srednja'
  return 'realizacija-niska'
}

const getOdstupostotakClass = (procenat: number) => {
  const abs = Math.abs(procenat)
  if (abs >= 30) return 'odstupanje-veliko'
  if (abs >= 15) return 'odstupanje-srednje'
  return 'odstupanje-malo'
}

const goBack = () => {
  router.push('/prijem-sirovina')
}

onMounted(() => {
  loadDetails()
})
</script>

<style scoped>
.create-prijem-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 30px;
}

.btn-back {
  background: #f5f5f5;
  border: 1px solid #e0e0e0;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.btn-back:hover {
  background: #e0e0e0;
}

.header h1 {
  font-size: 28px;
  font-weight: 700;
  color: #000;
  margin: 0;
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

.content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.info-card,
.stats-card,
.radnici-card,
.prijem-form-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  padding: 24px;
}

.info-card h2,
.stats-card h2,
.radnici-card h2,
.prijem-form-card h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin-bottom: 20px;
  padding-bottom: 12px;
  border-bottom: 2px solid #000;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.info-item .label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
}

.info-item .value {
  font-size: 16px;
  color: #000;
  font-weight: 600;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.stat-box {
  background: #f5f5f5;
  padding: 16px;
  border-radius: 8px;
  text-align: center;
}

.stat-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 8px;
  display: block;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #000;
}

.stat-value.highlight {
  color: #1976d2;
}

.realizacija-odlicna {
  color: #2e7d32 !important;
}

.realizacija-dobra {
  color: #388e3c !important;
}

.realizacija-srednja {
  color: #f57c00 !important;
}

.realizacija-niska {
  color: #d32f2f !important;
}

.table-wrapper {
  overflow-x: auto;
}

.radnici-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 20px;
}

.radnici-table thead {
  background: #f5f5f5;
}

.radnici-table th,
.radnici-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.radnici-table th {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 700;
}

.radnici-table td {
  font-size: 14px;
  color: #000;
}

.radnici-table td.bold {
  font-weight: 700;
}

.outlier-row {
  background: #fff3e0;
}

.odstupanje-veliko {
  color: #d32f2f;
  font-weight: 700;
}

.odstupanje-srednje {
  color: #f57c00;
  font-weight: 600;
}

.odstupanje-malo {
  color: #666;
}

.warning-badge {
  margin-left: 8px;
}

.anomalije-section {
  background: #fff3e0;
  padding: 16px;
  border-radius: 8px;
  border-left: 4px solid #f57c00;
}

.anomalije-section h3 {
  font-size: 16px;
  color: #e65100;
  margin-bottom: 12px;
}

.anomalija-item {
  font-size: 14px;
  color: #000;
  margin-bottom: 8px;
}

.prijem-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  color: #000;
  font-weight: 600;
}

.form-control {
  padding: 12px;
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

.form-hint {
  font-size: 12px;
  color: #666;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 20px;
}

.btn-secondary,
.btn-primary {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-secondary {
  background: #f5f5f5;
  border: 1px solid #e0e0e0;
  color: #000;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.btn-primary {
  background: #000;
  color: #fff;
  border: none;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .info-grid,
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
  }

  .btn-secondary,
  .btn-primary {
    width: 100%;
  }
}
</style>


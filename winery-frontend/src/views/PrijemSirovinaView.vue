<template>
  <div class="prijem-container">
    <div class="header">
      <div class="header-content">
        <div>
          <h1>Prijem ubranih sirovina</h1>
          <p class="subtitle">Rasporedi spremni za prijem</p>
        </div>
        <!-- <router-link to="/prijemi" class="btn-secondary">
          Vidi Sve Prijeme
        </router-link> -->
      </div>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="rasporedi.length === 0" class="no-data">
      <p>Trenutno nema rasporeda spremnih za prijem.</p>
      <p class="hint">Rasporedi će biti prikazani kada svi radnici unesu svoje količine ili istekne planirani rok.</p>
    </div>

    <div v-else class="rasporedi-grid">
      <div 
        v-for="raspored in rasporedi" 
        :key="raspored.rasporedId"
        class="raspored-card"
      >
        <div class="card-header">
          <h3>{{ raspored.berbaNaziv }} ({{ raspored.sezona }})</h3>
          <span class="status-badge status-ready">Spremno za prijem</span>
        </div>

        <div class="card-body">
          <div class="info-section">
            <h4>Lokacija</h4>
            <p class="location">{{ raspored.vinogradNaziv }} - {{ raspored.parcelaNaziv }}</p>
            <p v-if="raspored.sortaNaziv" class="sorta">Sorta: {{ raspored.sortaNaziv }}</p>
          </div>

          <div class="info-section">
            <h4>Period branja</h4>
            <p>{{ formatDate(raspored.pocetakBranja) }} - {{ formatDate(raspored.zavrsetakBranja) }}</p>
          </div>

          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-label">Očekivano</span>
              <span class="stat-value">{{ raspored.ocekivaniPrinos.toFixed(2) }} kg</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Ubrano</span>
              <span class="stat-value">{{ raspored.ukupnoUbrano.toFixed(2) }} kg</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Realizacija</span>
              <span class="stat-value" :class="getRealizacijaClass(raspored.procenatRealizacije)">
                {{ raspored.procenatRealizacije.toFixed(1) }}%
              </span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Radnici</span>
              <span class="stat-value">{{ raspored.radnikaSaUnosom }}/{{ raspored.ukupnoRadnika }}</span>
            </div>
          </div>
        </div>

        <div class="card-actions">
          <button @click="goToCreatePrijem(raspored.rasporedId)" class="btn-primary">
            Kreiraj prijem
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import ubranasirovinaService from '@/services/ubranasirovinaService'
import type { RasporedZaPrijem } from '@/types/ubranasirovina'

const router = useRouter()
const rasporedi = ref<RasporedZaPrijem[]>([])
const loading = ref(false)
const error = ref('')

const loadRasporedi = async () => {
  loading.value = true
  error.value = ''
  try {
    rasporedi.value = await ubranasirovinaService.getRasporedsReadyForPrijem()
  } catch (err: any) {
    console.error('Greška pri učitavanju rasporeda:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju rasporeda'
  } finally {
    loading.value = false
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

const goToCreatePrijem = (rasporedId: number) => {
  router.push(`/prijem-sirovina/create/${rasporedId}`)
}

onMounted(() => {
  loadRasporedi()
})
</script>

<style scoped>
.prijem-container {
  max-width: 1400px;
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

.btn-secondary {
  display: inline-block;
  padding: 10px 20px;
  background: #fff;
  color: #000;
  border: 2px solid #000;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.3s ease;
}

.btn-secondary:hover {
  background: #000;
  color: #fff;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
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

.no-data p {
  font-size: 18px;
  margin-bottom: 12px;
}

.hint {
  font-size: 14px;
  color: #999;
}

.rasporedi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(450px, 1fr));
  gap: 24px;
}

.raspored-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.raspored-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.card-header {
  background: linear-gradient(135deg, #1a1a1a 0%, #333 100%);
  color: #fff;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  font-size: 20px;
  font-weight: 600;
  margin: 0;
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.status-ready {
  background: #ffd54f;
  color: #000;
}

.card-body {
  padding: 20px;
}

.info-section {
  margin-bottom: 20px;
}

.info-section h4 {
  font-size: 14px;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 8px;
}

.location {
  font-size: 16px;
  font-weight: 600;
  color: #000;
  margin-bottom: 4px;
}

.sorta {
  font-size: 14px;
  color: #666;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
  background: #f5f5f5;
  padding: 16px;
  border-radius: 8px;
}

.stat-item {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 4px;
}

.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: #000;
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

.card-actions {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  justify-content: flex-end;
}

.btn-primary {
  background: #000;
  color: #fff;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

@media (max-width: 768px) {
  .rasporedi-grid {
    grid-template-columns: 1fr;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>


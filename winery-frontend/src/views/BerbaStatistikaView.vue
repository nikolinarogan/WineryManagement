<template>
  <div class="berba-statistika">
    <div class="container">
      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Statistika -->
      <div v-else-if="statistika" class="statistika-content">
        <!-- Header -->
        <div class="header">
          <div class="header-top">
            <router-link :to="`/berbe/${statistika.berbaIdber}`" class="back-link">
              ← Nazad na berbu
            </router-link>
          </div>
          <h1>Statistika: {{ statistika.berbaNaziv }}</h1>
          <p class="sezona">Sezona {{ statistika.sezona }}</p>
        </div>

        <!-- Summary Cards -->
        <div class="summary-grid">
          <div class="summary-card">
            <div class="card-content">
              <h3>Ukupno ubrano</h3>
              <p class="big-number">{{ statistika.ukupnoUbrano.toFixed(2) }} kg</p>
            </div>
          </div>

          <div class="summary-card">
            <div class="card-content">
              <h3>Očekivano</h3>
              <p class="big-number">{{ statistika.ocekivanoUkupno.toFixed(2) }} kg</p>
            </div>
          </div>

          <div class="summary-card">
            <div class="card-content">
              <h3>Realizacija</h3>
              <p class="big-number" :class="getRealizacijaClass(statistika.procenatRealizacije)">
                {{ statistika.procenatRealizacije.toFixed(1) }}%
              </p>
            </div>
          </div>

          <div class="summary-card">
            <div class="card-content">
              <h3>Broj radnika</h3>
              <p class="big-number">{{ statistika.brojRadnika }}</p>
            </div>
          </div>

          <div class="summary-card">
            <div class="card-content">
              <h3>Broj rasporeda</h3>
              <p class="big-number">{{ statistika.brojRasporeda }}</p>
            </div>
          </div>

          <div class="summary-card">
            <div class="card-content">
              <h3>Prosjek po radniku</h3>
              <p class="big-number">
                {{ (statistika.ukupnoUbrano / statistika.brojRadnika || 0).toFixed(2) }} kg
              </p>
            </div>
          </div>
        </div>

        <!-- Progress Bar -->
        <div class="progress-section">
          <h2>Napredak Berbe</h2>
          <div class="progress-bar">
            <div 
              class="progress-fill" 
              :style="{ width: Math.min(statistika.procenatRealizacije, 100) + '%' }"
              :class="getProgressClass(statistika.procenatRealizacije)"
            ></div>
          </div>
          <div class="progress-labels">
            <span>0%</span>
            <span>{{ statistika.procenatRealizacije.toFixed(1) }}%</span>
            <span>100%</span>
          </div>
        </div>

        <!-- Radnici Učinak -->
        <div class="radnici-section">
          <h2>Učinak radnika</h2>
          
          <div v-if="statistika.radniciUcinak.length === 0" class="no-results">
            <p>Još nema unesenih količina od radnika.</p>
          </div>

          <div v-else class="radnici-table">
            <table>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Ime i prezime</th>
                  <th>Parcela</th>
                  <th>Količina (kg)</th>
                  <th>Datum branja</th>
                  <th>Procenat ukupno</th>
                </tr>
              </thead>
              <tbody>
                <tr 
                  v-for="(radnik, index) in statistika.radniciUcinak" 
                  :key="`${radnik.radnikId}-${index}`"
                  :class="{ 'top-performer': index < 3 }"
                >
                  <td class="rank">
                    <span :class="{ 'top-rank': index < 3 }">{{ index + 1 }}</span>
                  </td>
                  <td class="name">{{ radnik.radnikIme }} {{ radnik.radnikPrezime }}</td>
                  <td>{{ radnik.parcelaNaziv }}</td>
                  <td class="quantity">{{ radnik.kolicina.toFixed(2) }} kg</td>
                  <td>{{ radnik.datumBranja ? formatDate(radnik.datumBranja) : '-' }}</td>
                  <td class="percent">
                    {{ ((radnik.kolicina / statistika.ukupnoUbrano) * 100).toFixed(1) }}%
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import berbaService from '@/services/berbaService'
import type { BerbaStatistika } from '@/types/berba'

const route = useRoute()

const statistika = ref<BerbaStatistika | null>(null)
const loading = ref(false)
const error = ref('')

const loadStatistika = async () => {
  try {
    loading.value = true
    error.value = ''
    const berbaId = Number(route.params.id)
    statistika.value = await berbaService.getBerbaStatistika(berbaId)
  } catch (err: any) {
    console.error('Error loading statistika:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju statistike'
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS', { 
    day: '2-digit', 
    month: '2-digit', 
    year: 'numeric' 
  })
}

const getRealizacijaClass = (procenat: number) => {
  if (procenat >= 100) return 'excellent'
  if (procenat >= 80) return 'good'
  if (procenat >= 50) return 'average'
  return 'low'
}

const getProgressClass = (procenat: number) => {
  if (procenat >= 100) return 'progress-excellent'
  if (procenat >= 80) return 'progress-good'
  if (procenat >= 50) return 'progress-average'
  return 'progress-low'
}

onMounted(() => {
  loadStatistika()
})
</script>

<style scoped>
.berba-statistika {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.container {
  max-width: 1400px;
  margin: 0 auto;
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

.header {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 32px;
}

.header-top {
  margin-bottom: 20px;
}

.back-link {
  color: #000;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s;
}

.back-link:hover {
  color: #666;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 8px 0;
}

.sezona {
  font-size: 20px;
  color: #666;
  margin: 0;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 32px;
}

.summary-card {
  background: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s;
}

.summary-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.card-content {
  width: 100%;
}

.card-content h3 {
  margin: 0 0 8px 0;
  font-size: 14px;
  font-weight: 600;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.big-number {
  margin: 0;
  font-size: 32px;
  font-weight: 700;
  color: #000;
}

.big-number.excellent {
  color: #4caf50;
}

.big-number.good {
  color: #8bc34a;
}

.big-number.average {
  color: #ff9800;
}

.big-number.low {
  color: #f44336;
}

.progress-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 32px;
}

.progress-section h2 {
  margin: 0 0 24px 0;
  font-size: 24px;
  font-weight: 600;
  color: #000;
}

.progress-bar {
  width: 100%;
  height: 40px;
  background: #e0e0e0;
  border-radius: 20px;
  overflow: hidden;
  margin-bottom: 12px;
}

.progress-fill {
  height: 100%;
  transition: width 1s ease-out, background 0.3s;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-right: 12px;
  color: white;
  font-weight: 700;
}

.progress-excellent {
  background: linear-gradient(90deg, #4caf50 0%, #66bb6a 100%);
}

.progress-good {
  background: linear-gradient(90deg, #8bc34a 0%, #aed581 100%);
}

.progress-average {
  background: linear-gradient(90deg, #ff9800 0%, #ffb74d 100%);
}

.progress-low {
  background: linear-gradient(90deg, #f44336 0%, #e57373 100%);
}

.progress-labels {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: #666;
}

.radnici-section {
  background: white;
  padding: 32px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.radnici-section h2 {
  margin: 0 0 24px 0;
  font-size: 24px;
  font-weight: 600;
  color: #000;
}

.no-results {
  text-align: center;
  padding: 40px;
  background: #f9f9f9;
  border-radius: 8px;
  border: 2px dashed #ccc;
}

.no-results p {
  margin: 0;
  color: #666;
  font-size: 16px;
}

.radnici-table {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background: #f9f9f9;
}

th {
  padding: 16px 12px;
  text-align: left;
  font-weight: 600;
  color: #000;
  border-bottom: 2px solid #e0e0e0;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

td {
  padding: 16px 12px;
  border-bottom: 1px solid #e0e0e0;
}

tbody tr {
  transition: background 0.3s;
}

tbody tr:hover {
  background: #f9f9f9;
}

tbody tr.top-performer {
  background: #fffbf0;
}

tbody tr.top-performer:hover {
  background: #fff8e1;
}

.rank {
  font-weight: 700;
  color: #666;
  text-align: center;
  width: 60px;
}

.top-rank {
  font-weight: 800;
  font-size: 18px;
  color: #000;
}

.name {
  font-weight: 600;
  color: #000;
}

.quantity {
  font-weight: 700;
  color: #000;
  font-size: 16px;
}

.percent {
  color: #666;
  font-style: italic;
}

@media (max-width: 768px) {
  .berba-statistika {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .summary-grid {
    grid-template-columns: 1fr;
  }

  table {
    font-size: 14px;
  }

  th, td {
    padding: 12px 8px;
  }

  .big-number {
    font-size: 24px;
  }
}
</style>


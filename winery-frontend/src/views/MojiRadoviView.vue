<template>
  <div class="moji-radovi">
    <div class="container">
      <h1>Moji radovi</h1>
      <p class="subtitle">Pregled svih radova na kojima ste angažovani</p>

      <div v-if="loading" class="loading">Učitavanje...</div>
      <div v-else-if="error" class="error-message">{{ error }}</div>
      <div v-else-if="radovi.length === 0" class="no-results">
        <p>Trenutno niste angažovani ni na jednom radu.</p>
      </div>

      <div v-else class="radovi-grid">
        <div v-for="rad in radovi" :key="rad.idrad" class="rad-card">
          <div class="rad-header">
            <h3>Rad #{{ rad.idrad }}</h3>
            <span class="status-badge" :class="getStatusClass(rad)">
              {{ getStatus(rad) }}
            </span>
          </div>
          <div class="rad-body">
            <div class="info-row">
              <span class="label">Period:</span>
              <span class="value">{{ formatDate(rad.pocrad) }} - {{ formatDate(rad.zavrrad) }}</span>
            </div>
            <div class="info-row">
              <span class="label">Oprema:</span>
              <span class="value">{{ rad.oprema }}</span>
            </div>
            <div class="parcele-section">
              <h4>Parcele:</h4>
              <ul>
                <li v-for="parcela in rad.parcele" :key="parcela.idp">
                  {{ parcela.nazivparcele }} ({{ parcela.vinogradNaziv }})
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import radoviService from '@/services/radoviService'
import type { RadnikRad } from '@/types/radovi'

const radovi = ref<RadnikRad[]>([])
const loading = ref(false)
const error = ref('')

const loadRadovi = async () => {
  try {
    loading.value = true
    error.value = ''
    radovi.value = await radoviService.getMojiRadovi()
  } catch (err: any) {
    console.error('Error loading radovi:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju radova'
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

const getStatus = (rad: RadnikRad) => {
  const today = new Date()
  const start = new Date(rad.pocrad)
  const end = new Date(rad.zavrrad)

  if (today < start) return 'Planiran'
  if (today > end) return 'Završen'
  return 'U toku'
}

const getStatusClass = (rad: RadnikRad) => {
  const today = new Date()
  const start = new Date(rad.pocrad)
  const end = new Date(rad.zavrrad)

  if (today < start) return 'status-planned'
  if (today > end) return 'status-finished'
  return 'status-active'
}

onMounted(() => {
  loadRadovi()
})
</script>

<style scoped>
.moji-radovi {
  width: 100%;
  min-height: calc(100vh - 64px);
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
}

.container {
  max-width: 1400px;
  margin: 0 auto;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 12px 0;
}

.subtitle {
  color: #666;
  font-size: 18px;
  margin: 0 0 40px 0;
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

.no-results {
  background: white;
  padding: 60px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
  color: #666;
}

.radovi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 24px;
}

.rad-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: all 0.3s;
}

.rad-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.rad-header {
  padding: 20px;
  background: linear-gradient(135deg, #1a1a1a 0%, #333 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.rad-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
}

.status-active {
  background: #4caf50;
  color: white;
}

.status-planned {
  background: #2196f3;
  color: white;
}

.status-finished {
  background: #9e9e9e;
  color: white;
}

.rad-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
}

.info-row .label {
  font-weight: 600;
  color: #666;
}

.info-row .value {
  font-weight: 500;
  color: #000;
}

.parcele-section {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}

.parcele-section h4 {
  margin: 0 0 12px 0;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.parcele-section ul {
  margin: 0;
  padding-left: 20px;
  list-style-type: disc;
}

.parcele-section li {
  margin-bottom: 8px;
  color: #666;
}

@media (max-width: 768px) {
  .moji-radovi {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .radovi-grid {
    grid-template-columns: 1fr;
  }
}
</style>


<template>
  <div class="magacini-container">
    <div class="header">
      <h1>Magacini</h1>
      <p class="subtitle">Upravljanje magacinima za skladištenje boca vina</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="magacini.length === 0" class="no-data">
      <p>Još nema kreiranih magacina.</p>
    </div>

    <div v-else class="content">
      <div class="magacini-grid">
        <div 
          v-for="magacin in magacini" 
          :key="magacin.idmag"
          class="magacin-card"
          @click="viewDetail(magacin.idmag)"
        >
          <div class="card-header">
            <h3>{{ magacin.nazivmag }}</h3>
            <span class="temp-badge" :class="getTempClass(magacin.tempmag)">
              {{ magacin.tempmag }}°C
            </span>
          </div>

          <div class="card-body">
            <div class="capacity-info">
              <div class="capacity-bar">
                <div 
                  class="capacity-fill" 
                  :style="{ width: magacin.popunjenost + '%' }"
                  :class="getCapacityClass(magacin.popunjenost)"
                ></div>
              </div>
              <div class="capacity-text">
                {{ magacin.brojBoca }} / {{ magacin.kapacitetmag }} boca ({{ magacin.popunjenost }}%)
              </div>
            </div>

            <div class="info-row">
              <span class="label">Kapacitet:</span>
              <span class="value">{{ magacin.kapacitetmag }} boca</span>
            </div>
            <div class="info-row">
              <span class="label">Popunjenost:</span>
              <span class="value" :class="getCapacityClass(magacin.popunjenost)">
                {{ magacin.popunjenost }}%
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import magacinService from '@/services/magacinService'
import type { Magacin } from '@/types/magacin'

const router = useRouter()
const magacini = ref<Magacin[]>([])
const loading = ref(false)
const error = ref('')

const loadMagacini = async () => {
  loading.value = true
  error.value = ''
  try {
    magacini.value = await magacinService.getAllMagacini()
  } catch (err: any) {
    console.error('Greška pri učitavanju magacina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju magacina'
  } finally {
    loading.value = false
  }
}

const viewDetail = (id: number) => {
  router.push(`/magacini/${id}`)
}

const getTempClass = (temp: number) => {
  if (temp < 10) return 'temp-cold'
  if (temp < 18) return 'temp-optimal'
  return 'temp-warm'
}

const getCapacityClass = (percentage: number) => {
  if (percentage >= 90) return 'capacity-full'
  if (percentage >= 70) return 'capacity-high'
  if (percentage >= 40) return 'capacity-medium'
  return 'capacity-low'
}

onMounted(() => {
  loadMagacini()
})
</script>

<style scoped>
.magacini-container {
  max-width: 1400px;
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
.error-message,
.no-data {
  text-align: center;
  padding: 40px;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
}

.magacini-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.magacin-card {
  background: #fff;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.magacin-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
  border-color: #000;
}

.card-header {
  padding: 20px;
  background: #000;
  color: #fff;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}

.temp-badge {
  padding: 6px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 700;
}

.temp-badge.temp-cold {
  background: rgba(33, 150, 243, 0.3);
}

.temp-badge.temp-optimal {
  background: rgba(76, 175, 80, 0.3);
}

.temp-badge.temp-warm {
  background: rgba(255, 152, 0, 0.3);
}

.card-body {
  padding: 20px;
}

.capacity-info {
  margin-bottom: 16px;
}

.capacity-bar {
  height: 24px;
  background: #f0f0f0;
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 8px;
}

.capacity-fill {
  height: 100%;
  transition: width 0.3s ease;
  border-radius: 12px;
}

.capacity-fill.capacity-low {
  background: linear-gradient(90deg, #4CAF50, #66BB6A);
}

.capacity-fill.capacity-medium {
  background: linear-gradient(90deg, #FFC107, #FFD54F);
}

.capacity-fill.capacity-high {
  background: linear-gradient(90deg, #FF9800, #FFB74D);
}

.capacity-fill.capacity-full {
  background: linear-gradient(90deg, #F44336, #E57373);
}

.capacity-text {
  font-size: 13px;
  color: #666;
  text-align: center;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-row:last-child {
  border-bottom: none;
}

.info-row .label {
  font-size: 14px;
  color: #666;
}

.info-row .value {
  font-size: 14px;
  font-weight: 600;
  color: #000;
}

.info-row .value.capacity-low {
  color: #4CAF50;
}

.info-row .value.capacity-medium {
  color: #FFC107;
}

.info-row .value.capacity-high {
  color: #FF9800;
}

.info-row .value.capacity-full {
  color: #F44336;
}
</style>


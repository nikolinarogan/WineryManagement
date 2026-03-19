<template>
  <div class="podrumi-container">
    <div class="header">
      <h1>Podrumi</h1>
      <p class="subtitle">Upravljanje podrumima za lagering vina</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="podrumi.length === 0" class="no-data">
      <p>Još nema kreiranih podruma.</p>
    </div>

    <div v-else class="content">
      <div class="podrumi-grid">
        <div 
          v-for="podrum in podrumi" 
          :key="podrum.idpod"
          class="podrum-card"
          @click="viewDetail(podrum.idpod)"
        >
          <div class="card-header">
            <h3>{{ podrum.nazivpod }}</h3>
            <span class="temp-badge" :class="getTempClass(podrum.temp)">
              {{ podrum.temp }}°C
            </span>
          </div>

          <div class="card-body">
            <div class="info-row">
              <span class="label">Temperatura:</span>
              <span class="value">{{ podrum.temp }}°C</span>
            </div>
            <div class="info-row">
              <span class="label">Broj buradi:</span>
              <span class="value">{{ podrum.brojBuradi }}</span>
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
import podrumService from '@/services/podrumService'
import type { Podrum } from '@/types/podrum'

const router = useRouter()
const podrumi = ref<Podrum[]>([])
const loading = ref(false)
const error = ref('')

const loadPodrumi = async () => {
  loading.value = true
  error.value = ''
  try {
    podrumi.value = await podrumService.getAllPodrumi()
  } catch (err: any) {
    console.error('Greška pri učitavanju podruma:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju podruma'
  } finally {
    loading.value = false
  }
}

const viewDetail = (id: number) => {
  router.push(`/podrumi/${id}`)
}

const getTempClass = (temp: number) => {
  if (temp < 10) return 'temp-cold'
  if (temp < 18) return 'temp-optimal'
  return 'temp-warm'
}

onMounted(() => {
  loadPodrumi()
})
</script>

<style scoped>
.podrumi-container {
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

.podrumi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.podrum-card {
  background: #fff;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.podrum-card:hover {
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
</style>


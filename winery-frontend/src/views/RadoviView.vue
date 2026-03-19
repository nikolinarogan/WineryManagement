<template>
  <div class="radovi-view">
    <div class="container">
      <div class="header">
        <h1>Radovi na parcelama</h1>
        <!-- <button @click="$router.push('/radovi/new')" class="btn-primary">
          + Kreiraj Novi Rad
        </button> -->
      </div>

      <p class="subtitle">Pregled svih radova koji se odvijaju na parcelama vinograda</p>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- No radovi -->
      <div v-else-if="radovi.length === 0" class="no-results">
        <p>Nema kreiranih radova.</p>
        <button @click="$router.push('/radovi/new')" class="btn-primary">
          Kreiraj prvi rad
        </button>
      </div>

      <!-- Radovi Grid -->
      <div v-else class="radovi-grid">
        <div
          v-for="rad in radovi"
          :key="rad.idrad"
          class="rad-card"
          @click="$router.push(`/radovi/${rad.idrad}`)"
        >
          <div class="rad-header">
            <h3>Rad #{{ rad.idrad }}</h3>
            <span class="status-badge" :class="getStatusClass(rad)">
              {{ getStatus(rad) }}
            </span>
          </div>
          <div class="rad-body">
            <div class="info-row">
              <span class="label">Početak:</span>
              <span class="value">{{ formatDate(rad.pocrad) }}</span>
            </div>
            <div class="info-row">
              <span class="label">Završetak:</span>
              <span class="value">{{ formatDate(rad.zavrrad) }}</span>
            </div>
            <div class="info-row">
              <span class="label">Oprema:</span>
              <span class="value">{{ rad.oprema }}</span>
            </div>
            <div class="info-row">
              <span class="label">Parcele:</span>
              <span class="value">{{ rad.brojParcela }}</span>
            </div>
            <div class="info-row">
              <span class="label">Radnici:</span>
              <span class="value">{{ rad.brojRadnika }}</span>
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
import type { Radovi } from '@/types/radovi'

const radovi = ref<Radovi[]>([])
const loading = ref(false)
const error = ref('')

const loadRadovi = async () => {
  try {
    loading.value = true
    error.value = ''
    radovi.value = await radoviService.getAllRadovi()
  } catch (err: any) {
    console.error('Error loading radovi:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju radova'
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

const getStatus = (rad: Radovi) => {
  const today = new Date()
  const start = new Date(rad.pocrad)
  const end = new Date(rad.zavrrad)

  if (today < start) return 'Planiran'
  if (today > end) return 'Završen'
  return 'U toku'
}

const getStatusClass = (rad: Radovi) => {
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
.radovi-view {
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

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.subtitle {
  color: #666;
  font-size: 18px;
  margin: 0 0 40px 0;
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

.btn-primary:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
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
}

.no-results p {
  font-size: 18px;
  color: #666;
  margin: 0 0 24px 0;
}

.radovi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 24px;
}

.rad-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  cursor: pointer;
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
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-row .label {
  font-size: 14px;
  font-weight: 600;
  color: #999;
}

.info-row .value {
  font-size: 16px;
  font-weight: 500;
  color: #000;
}

@media (max-width: 768px) {
  .radovi-view {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .radovi-grid {
    grid-template-columns: 1fr;
  }
}
</style>


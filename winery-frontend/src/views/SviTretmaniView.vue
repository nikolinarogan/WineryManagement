<template>
  <div class="svi-tretmani-container">
    <div class="header">
      <h1>Svi Tretmani</h1>
      <p class="subtitle">Pregled svih kreiranih tretmana</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">{{ error }}</div>

    <div v-else-if="tretmani.length === 0" class="no-data">
      <p>Nema tretmana za prikaz.</p>
    </div>

    <div v-else class="tretmani-grid">
      <div 
        v-for="tretman in tretmani" 
        :key="tretman.idtretmana"
        class="tretman-card"
        @click="goToTretman(tretman.idtretmana)"
      >
        <div class="card-header">
          <h3>{{ tretman.naziv }}</h3>
          <span class="status-badge" :class="{ active: tretman.jeAktivan }">
            {{ tretman.jeAktivan ? 'Aktivan' : 'Završen' }}
          </span>
        </div>

        <div class="card-body">
          <div class="info-row">
            <span class="label">Ubrana sirovina ID:</span>
            <span class="value">{{ tretman.ubranasirovinaIdubrsir }}</span>
          </div>
          <div class="info-row">
            <span class="label">Datum početka:</span>
            <span class="value">{{ formatDate(tretman.datpocetkatret) }}</span>
          </div>
          <div class="info-row">
            <span class="label">Datum završetka:</span>
            <span class="value">
              {{ tretman.datzavresetkatret ? formatDate(tretman.datzavresetkatret) : 'U toku' }}
            </span>
          </div>
          <div class="info-row">
            <span class="label">Trajanje:</span>
            <span class="value">{{ tretman.trajanjeUDanima }} dana</span>
          </div>
          <div class="info-row">
            <span class="label">Broj sirovina:</span>
            <span class="value">{{ tretman.brojSirovina }}</span>
          </div>
        </div>

        <div class="card-footer">
          <button class="btn-view">Vidi Detalje →</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import tretmanService from '@/services/tretmanService'
import type { Tretman } from '@/types/tretman'

const router = useRouter()
const tretmani = ref<Tretman[]>([])
const loading = ref(false)
const error = ref('')

const loadTretmani = async () => {
  loading.value = true
  error.value = ''
  try {
    const aktivni = await tretmanService.getAktivniTretmani()
    const zavrseni = await tretmanService.getZavreniTretmani()
    tretmani.value = [...aktivni, ...zavrseni].sort((a, b) => b.idtretmana - a.idtretmana)
  } catch (err: any) {
    console.error('Greška pri učitavanju tretmana:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju tretmana'
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const goToTretman = (tretmanId: number) => {
  router.push(`/tretmani/detail/${tretmanId}`)
}

onMounted(() => {
  loadTretmani()
})
</script>

<style scoped>
.svi-tretmani-container {
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
  margin-bottom: 30px;
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

.tretmani-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.tretman-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.tretman-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.card-header {
  padding: 20px;
  background: #f5f5f5;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.card-header h3 {
  font-size: 18px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
  background: #e0e0e0;
  color: #666;
  white-space: nowrap;
}

.status-badge.active {
  background: #c8e6c9;
  color: #2e7d32;
}

.card-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f5f5f5;
}

.info-row:last-child {
  border-bottom: none;
}

.label {
  color: #666;
  font-size: 14px;
}

.value {
  color: #000;
  font-size: 14px;
  font-weight: 500;
  text-align: right;
}

.card-footer {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
}

.btn-view {
  width: 100%;
  background: #000;
  color: #fff;
  padding: 10px 16px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-view:hover {
  background: #333;
}
</style>


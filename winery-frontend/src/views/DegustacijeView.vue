<template>
  <div class="degustacije-container">
    <div class="header">
      <div class="header-content">
        <div>
          <h1>Degustacije vina</h1>
          <p class="subtitle">Pregled svih organizovanih degustacija</p>
        </div>
        <router-link to="/degustacije/nova" class="btn-primary">+ Kreiraj degustaciju</router-link>
      </div>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="degustacije.length === 0" class="no-data">
      <p>Još nema kreiranih degustacija.</p>
      <router-link to="/degustacije/nova" class="btn-primary">Kreirajte prvu degustaciju</router-link>
    </div>

    <div v-else class="degustacije-grid">
      <div v-for="degustacija in degustacije" :key="degustacija.iddeg" class="degustacija-card">
        <div class="card-header">
          <h3>{{ degustacija.nazivdeg }}</h3>
          <span class="vina-count">{{ degustacija.brojVina }} vina</span>
        </div>

        <div class="card-body">
          <div class="info-row">
            <span class="label"> Datum:</span>
            <span class="value">{{ formatDate(degustacija.datdeg) }}</span>
          </div>
          <div class="info-row">
            <span class="label"> Kapacitet:</span>
            <span class="value">{{ degustacija.kapacitetdeg }} osoba</span>
          </div>
        </div>

        <div class="card-actions">
          <router-link :to="`/degustacije/${degustacija.iddeg}`" class="btn-view">Detalji</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import degustacijaService from '@/services/degustacijaService'
import type { Degustacija } from '@/types/degustacija'

const degustacije = ref<Degustacija[]>([])
const loading = ref(false)
const error = ref('')

const loadDegustacije = async () => {
  loading.value = true
  error.value = ''
  try {
    degustacije.value = await degustacijaService.getAllDegustacije()
  } catch (err: any) {
    console.error('Greška pri učitavanju degustacija:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju degustacija'
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

onMounted(() => {
  loadDegustacije()
})
</script>

<style scoped>
.degustacije-container {
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
  align-items: flex-start;
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

.no-data {
  background: #f9f9f9;
  border-radius: 12px;
}

.degustacije-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.degustacija-card {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: all 0.3s;
}

.degustacija-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.card-header {
  background: linear-gradient(135deg, #000 0%, #333 100%);
  color: #fff;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}

.vina-count {
  background: rgba(255, 255, 255, 0.2);
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.card-body {
  padding: 20px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.info-row:last-child {
  margin-bottom: 0;
}

.label {
  font-size: 14px;
  color: #666;
  font-weight: 600;
}

.value {
  font-size: 14px;
  color: #000;
  font-weight: 700;
}

.card-actions {
  padding: 16px 20px;
  background: #f9f9f9;
  display: flex;
  gap: 8px;
}

.btn-primary,
.btn-view {
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

.btn-primary {
  background: #000;
  color: #fff;
}

.btn-primary:hover {
  background: #333;
}

.btn-view {
  background: #f0f0f0;
  color: #000;
  flex: 1;
  text-align: center;
}

.btn-view:hover {
  background: #e0e0e0;
}
</style>


<template>
  <div class="tretmani-list-container">
    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="ubranasirovinaInfo" class="content">
      <div class="header">
        <button @click="goBack" class="btn-back">← Nazad</button>
        <div class="header-info">
          <div>
            <h1>Tretmani: {{ ubranasirovinaInfo.berbaNaziv }} ({{ ubranasirovinaInfo.sezona }})</h1>
            <p class="subtitle">{{ ubranasirovinaInfo.parcelaNaziv }} - {{ ubranasirovinaInfo.sortaNaziv || 'Nije definisana' }}</p>
          </div>
          <span class="status-badge" :class="`status-${ubranasirovinaInfo.status.toLowerCase()}`">
            {{ getStatusLabel(ubranasirovinaInfo.status) }}
          </span>
        </div>
      </div>

      <div class="info-card">
        <div class="info-grid">
          <div class="info-item">
            <span class="label">Količina</span>
            <span class="value">{{ ubranasirovinaInfo.kolicina }} kg</span>
          </div>
          <div class="info-item">
            <span class="label">Datum prijema</span>
            <span class="value">{{ formatDate(ubranasirovinaInfo.datprijema) }}</span>
          </div>
          <div class="info-item">
            <span class="label">Ukupno tretmana</span>
            <span class="value">{{ ubranasirovinaInfo.brojTretmana }}</span>
          </div>
          <div class="info-item">
            <span class="label">Aktivni tretmani</span>
            <span class="value">{{ ubranasirovinaInfo.aktivniTretmani }}</span>
          </div>
        </div>
      </div>

      <div v-if="tretmani.length === 0" class="no-data">
        <p>Još nema kreiranih tretmana za ovu ubranu sirovinu.</p>
      </div>

      <div v-else class="tretmani-grid">
        <div 
          v-for="tretman in tretmani" 
          :key="tretman.idtretmana"
          class="tretman-card"
          @click="goToTretmanDetail(tretman.idtretmana)"
        >
          <div class="card-header">
            <h3>{{ tretman.naziv }}</h3>
            <span class="status-badge" :class="{ 'active': tretman.jeAktivan }">
              {{ tretman.jeAktivan ? 'Aktivan' : 'Završen' }}
            </span>
          </div>

          <div class="card-body">
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
            <div class="info-row">
              <span class="label">Enolozi:</span>
              <span class="value">{{ tretman.enolozi.map(e => `${e.ime} ${e.prez}`).join(', ') || 'Nema' }}</span>
            </div>
          </div>

          <div class="card-footer">
            <button class="btn-view">Vidi Detalje →</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import tretmanService from '@/services/tretmanService'
import type { UbranasirovinaZaTretman, Tretman } from '@/types/tretman'

const route = useRoute()
const router = useRouter()
const ubranasirovinaId = parseInt(route.params.id as string)

const ubranasirovinaInfo = ref<UbranasirovinaZaTretman | null>(null)
const tretmani = ref<Tretman[]>([])
const loading = ref(false)
const error = ref('')

const loadData = async () => {
  loading.value = true
  error.value = ''
  try {
    // Učitaj info o ubranoj sirovini
    ubranasirovinaInfo.value = await tretmanService.getUbranaSirovinaById(ubranasirovinaId)
    
    // Učitaj tretmane
    tretmani.value = await tretmanService.getTretmaniForUbranaSirovina(ubranasirovinaId)
  } catch (err: any) {
    console.error('Greška pri učitavanju podataka:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju podataka'
  } finally {
    loading.value = false
  }
}

const getStatusLabel = (status: string): string => {
  const labels: Record<string, string> = {
    'Nova': 'Nova',
    'UTretmanu': 'U Tretmanu',
    'SpremaZaVino': 'Spremna za Vino',
    'Obradjena': 'Obrađena'
  }
  return labels[status] || status
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const goBack = () => {
  router.push('/ubrane-sirovine')
}

const goToTretmanDetail = (tretmanId: number) => {
  router.push(`/tretmani/detail/${tretmanId}`)
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.tretmani-list-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.loading,
.error-message {
  text-align: center;
  padding: 40px;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  border-radius: 8px;
}

.header {
  margin-bottom: 30px;
}

.btn-back {
  background: none;
  border: none;
  font-size: 16px;
  color: #666;
  cursor: pointer;
  margin-bottom: 16px;
  padding: 8px 0;
}

.btn-back:hover {
  color: #000;
}

.header-info {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;
}

.header-info h1 {
  font-size: 28px;
  font-weight: 700;
  color: #000;
  margin-bottom: 8px;
}

.subtitle {
  font-size: 16px;
  color: #666;
}

.status-badge {
  padding: 8px 16px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  white-space: nowrap;
}

.status-badge.active {
  background: #c8e6c9;
  color: #2e7d32;
}

.status-nova {
  background: #e3f2fd;
  color: #1976d2;
}

.status-utretmanu {
  background: #fff3e0;
  color: #f57c00;
}

.status-spremazavino {
  background: #c8e6c9;
  color: #2e7d32;
}

.status-obradjena {
  background: #e0e0e0;
  color: #666;
}

.info-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 30px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.info-item {
  text-align: center;
}

.info-item .label {
  display: block;
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 4px;
}

.info-item .value {
  display: block;
  font-size: 18px;
  color: #000;
  font-weight: 700;
}

.no-data {
  text-align: center;
  padding: 60px 20px;
  color: #666;
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


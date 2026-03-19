<template>
  <div class="vina-container">
    <div class="header">
      <h1>Finalna vina</h1>
      <p class="subtitle">Pregled svih finalnih vina spremnih za prodaju</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="vina.length === 0" class="no-data">
      <p>Još nema kreiranih finalnih vina.</p>
    </div>

    <div v-else class="content">
      <div class="vina-grid">
        <div 
          v-for="vino in vina" 
          :key="vino.idvina"
          class="vino-card"
          @click="viewDetail(vino.idvina)"
        >
          <div class="card-header" :class="`tip-${vino.tipvina.toLowerCase()}`">
            <h3>{{ vino.nazivvina }}</h3>
            <span class="alcohol-badge">{{ vino.procalk }}%</span>
          </div>

          <div class="card-body">
            <div class="info-row">
              <span class="label">Tip:</span>
              <span class="value">{{ vino.tipvina }}</span>
            </div>
            <div class="info-row">
              <span class="label">Blend:</span>
              <span class="value">{{ vino.brojSirovihVina }} sirovo vino{{ vino.brojSirovihVina > 1 ? '' : '' }}</span>
            </div>
            
            <div class="sirova-vina-section">
              <h4>Sastav:</h4>
              <div v-for="sv in vino.sirovaVina" :key="sv.idsirvina" class="sirovo-vino-item">
                {{ sv.nazivsirvina }} ({{ sv.godproizvodnje }})
              </div>
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
import vinoService from '@/services/vinoService'
import type { Vino } from '@/types/vino'

const router = useRouter()
const vina = ref<Vino[]>([])
const loading = ref(false)
const error = ref('')

const loadVina = async () => {
  loading.value = true
  error.value = ''
  try {
    vina.value = await vinoService.getAllVina()
  } catch (err: any) {
    console.error('Greška pri učitavanju finalnih vina:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju finalnih vina'
  } finally {
    loading.value = false
  }
}

const viewDetail = (id: number) => {
  router.push(`/vina/${id}`)
}

onMounted(() => {
  loadVina()
})
</script>

<style scoped>
.vina-container {
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

.vina-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.vino-card {
  background: #fff;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.vino-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
  border-color: #000;
}

.card-header {
  padding: 20px;
  color: #fff;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header.tip-crveno {
  background: linear-gradient(135deg, #8B0000, #B22222);
}

.card-header.tip-belo {
  background: linear-gradient(135deg, #DAA520, #F4E4C1);
  color: #000;
}

.card-header.tip-roze {
  background: linear-gradient(135deg, #FF1493, #FFB6C1);
}

.card-header h3 {
  font-size: 20px;
  font-weight: 700;
  margin: 0;
}

.alcohol-badge {
  padding: 6px 12px;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 12px;
  font-size: 14px;
  font-weight: 700;
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

.info-row .label {
  font-size: 14px;
  color: #666;
}

.info-row .value {
  font-size: 14px;
  font-weight: 600;
  color: #000;
}

.sirova-vina-section {
  margin-top: 16px;
}

.sirova-vina-section h4 {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 8px;
}

.sirovo-vino-item {
  background: #f5f5f5;
  padding: 8px 12px;
  border-radius: 6px;
  font-size: 13px;
  margin-bottom: 4px;
  color: #000;
}
</style>


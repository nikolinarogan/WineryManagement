<template>
  <div class="boca-inventar-container">
    <div class="header">
      <div class="header-content">
        <div>
          <h1>Inventar boca</h1>
          <p class="subtitle">Pregled svih napunjenih boca vina</p>
        </div>
        <router-link v-if="isEnolog" to="/punjenje-boca" class="btn-primary">+ Napuni nove boce</router-link>
      </div>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="boce.length === 0" class="no-data">
      <p>Još nema napunjenih boca.</p>
      <router-link v-if="isEnolog" to="/punjenje-boca" class="btn-primary">Napunite prve boce</router-link>
    </div>

    <div v-else class="content">
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-label">Ukupno boca</div>
          <div class="stat-value">{{ boce.length }}</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Ukupna zapremina</div>
          <div class="stat-value">{{ ukupnaZapremina.toFixed(2) }} L</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Različitih vina</div>
          <div class="stat-value">{{ brojVina }}</div>
        </div>
      </div>

      <div class="table-container">
        <table class="boce-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Vino</th>
              <th>Tip</th>
              <th>Zapremina</th>
              <th>Cena</th>
              <th>Magacin</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="boca in boce" :key="boca.idboce">
              <td>{{ boca.idboce }}</td>
              <td><strong>{{ boca.nazivVina }}</strong></td>
              <td>
                <span class="tip-badge" :class="getTipClass(boca.tipVina)">
                  {{ boca.tipVina }}
                </span>
              </td>
              <td>{{ boca.zapremina }} L</td>
              <td>{{ boca.cena ? boca.cena.toFixed(2) + ' RSD' : '-' }}</td>
              <td>{{ boca.nazivMagacina }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import bocaService from '@/services/bocaService'
import type { BocaInventar } from '@/types/boca'

const authStore = useAuthStore()
const isEnolog = computed(() => authStore.role === 'Enolog')

const boce = ref<BocaInventar[]>([])
const loading = ref(false)
const error = ref('')

const ukupnaZapremina = computed(() => {
  return boce.value.reduce((sum, boca) => sum + boca.zapremina, 0)
})

const brojVina = computed(() => {
  const uniqueVina = new Set(boce.value.map(b => b.nazivVina))
  return uniqueVina.size
})

const loadBoce = async () => {
  loading.value = true
  error.value = ''
  try {
    boce.value = await bocaService.getAllBoce()
  } catch (err: any) {
    console.error('Greška pri učitavanju boca:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju boca'
  } finally {
    loading.value = false
  }
}

const getTipClass = (tip: string) => {
  if (tip === 'Crveno') return 'tip-crveno'
  if (tip === 'Belo') return 'tip-belo'
  if (tip === 'Roze') return 'tip-roze'
  return ''
}

onMounted(() => {
  loadBoce()
})
</script>

<style scoped>
.boca-inventar-container {
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

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: #fff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.stat-label {
  font-size: 14px;
  color: #666;
  margin-bottom: 8px;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #000;
}

.table-container {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.boce-table {
  width: 100%;
  border-collapse: collapse;
}

.boce-table thead {
  background: #000;
  color: #fff;
}

.boce-table th {
  padding: 16px;
  text-align: left;
  font-weight: 600;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.boce-table tbody tr {
  border-bottom: 1px solid #e0e0e0;
  transition: background 0.3s;
}

.boce-table tbody tr:hover {
  background: #f9f9f9;
}

.boce-table tbody tr:last-child {
  border-bottom: none;
}

.boce-table td {
  padding: 16px;
  font-size: 14px;
  color: #000;
}

.tip-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 700;
}

.tip-badge.tip-crveno {
  background: #ffebee;
  color: #c62828;
}

.tip-badge.tip-belo {
  background: #fff9c4;
  color: #f57f17;
}

.tip-badge.tip-roze {
  background: #fce4ec;
  color: #c2185b;
}

.btn-primary {
  padding: 12px 24px;
  background: #000;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  text-decoration: none;
  display: inline-block;
}

.btn-primary:hover {
  background: #333;
}
</style>


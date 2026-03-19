<template>
  <div class="prijemi-container">
    <div class="header">
      <h1>Svi prijemi sirovina</h1>
      <p class="subtitle">Pregled svih kreiranih prijema ubranih sirovina</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="prijemi.length === 0" class="no-data">
      <p>Još nema kreiranih prijema sirovina.</p>
      <router-link to="/prijem-sirovina" class="btn-primary">
        Idi na Prijeme Spremne za Obradu
      </router-link>
    </div>

    <div v-else class="content">
      <!-- Statistika -->
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-label">Ukupno prijema</div>
          <div class="stat-value">{{ prijemi.length }}</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Ukupno primljeno</div>
          <div class="stat-value">{{ ukupnoPrimljeno.toFixed(2) }} kg</div>
        </div>
      </div>

      <!-- Tabela -->
      <div class="table-card">
        <div class="table-header">
          <h2>Lista prijema</h2>
        </div>

        <div class="table-wrapper">
          <table class="prijemi-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Berba</th>
                <th>Lokacija</th>
                <th>Sorta grožđa</th>
                <th>Količina (kg)</th>
                <th>Datum prijema</th>
                <th>Akcije</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="prijem in prijemi" 
                :key="prijem.idubrsir"
              >
                <td class="id-cell">{{ prijem.idubrsir }}</td>
                <td>
                  <strong>{{ prijem.berbaNaziv }}</strong>
                </td>
                <td>
                  <div class="location-cell">
                    <div class="vinograd">{{ prijem.vinogradNaziv }}</div>
                    <div class="parcela">{{ prijem.parcelaNaziv }}</div>
                  </div>
                </td>
                <td>
                  <span v-if="prijem.sortaNaziv" class="sorta-badge">
                    {{ prijem.sortaNaziv }}
                  </span>
                  <span v-else class="no-data-badge">-</span>
                </td>
                <td class="kolicina-cell">
                  <strong>{{ prijem.kolicina.toFixed(2) }}</strong> kg
                </td>
                <td>{{ formatDate(prijem.datprijema) }}</td>
                <td>
                  <div class="action-buttons">
                    <button 
                      @click="viewRaspored(prijem.rasporedbranjaIdras)" 
                      class="btn-view"
                      title="Vidi raspored"
                    >
                      Raspored
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import ubranasirovinaService from '@/services/ubranasirovinaService'
import type { Ubranasirovina } from '@/types/ubranasirovina'

const router = useRouter()
const prijemi = ref<Ubranasirovina[]>([])
const loading = ref(false)
const error = ref('')

const loadPrijemi = async () => {
  loading.value = true
  error.value = ''
  try {
    prijemi.value = await ubranasirovinaService.getAllPrijemi()
  } catch (err: any) {
    console.error('Greška pri učitavanju prijema:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju prijema'
  } finally {
    loading.value = false
  }
}

const ukupnoPrimljeno = computed(() => {
  return prijemi.value.reduce((sum, prijem) => sum + prijem.kolicina, 0)
})

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS')
}

const viewRaspored = (rasporedId: number) => {
  router.push(`/rasporedi/${rasporedId}`)
}

onMounted(() => {
  loadPrijemi()
})
</script>

<style scoped>
.prijemi-container {
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

.loading {
  text-align: center;
  padding: 40px;
  font-size: 18px;
  color: #666;
}

.error-message {
  background: #ffebee;
  color: #c62828;
  padding: 16px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.no-data {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.no-data p {
  font-size: 18px;
  margin-bottom: 20px;
}

.btn-primary {
  display: inline-block;
  background: #000;
  color: #fff;
  padding: 12px 24px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.stat-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  padding: 24px;
  text-align: center;
}

.stat-label {
  font-size: 14px;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
  margin-bottom: 12px;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #000;
}

.table-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  overflow: hidden;
}

.table-header {
  padding: 24px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.table-header h2 {
  font-size: 20px;
  font-weight: 700;
  color: #000;
  margin: 0;
}

.table-wrapper {
  overflow-x: auto;
}

.prijemi-table {
  width: 100%;
  border-collapse: collapse;
}

.prijemi-table thead {
  background: #f5f5f5;
}

.prijemi-table th,
.prijemi-table td {
  padding: 16px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.prijemi-table th {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  font-weight: 700;
  white-space: nowrap;
}

.prijemi-table td {
  font-size: 14px;
  color: #000;
}

.prijemi-table tbody tr:hover {
  background: #f9f9f9;
}

.id-cell {
  color: #666;
  font-weight: 600;
}

.location-cell {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.vinograd {
  font-weight: 600;
  color: #000;
}

.parcela {
  font-size: 13px;
  color: #666;
}

.sorta-badge {
  display: inline-block;
  padding: 4px 12px;
  background: #000;
  color: #fff;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.no-data-badge {
  color: #ccc;
  font-style: italic;
}

.kolicina-cell {
  font-size: 16px;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn-view,
.btn-delete {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-view {
  background: #e3f2fd;
  color: #1976d2;
}

.btn-view:hover {
  background: #1976d2;
  color: #fff;
}

@media (max-width: 768px) {
  .table-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .prijemi-table {
    font-size: 12px;
  }

  .prijemi-table th,
  .prijemi-table td {
    padding: 12px 8px;
  }

  .action-buttons {
    flex-direction: column;
  }
}
</style>


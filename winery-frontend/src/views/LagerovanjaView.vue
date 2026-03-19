<template>
  <div class="lagerovanja-container">
    <div class="header">
      <h1>Pregled svih lagerovanja</h1>
      <p class="subtitle">Istorija lagerovanja sirovih vina u buradima</p>
    </div>

    <div v-if="loading" class="loading">Učitavanje...</div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else-if="lagerovanja.length === 0" class="no-data">
      <p>Još nema evidencije o lagerovanju.</p>
    </div>

    <div v-else class="content">
      <!-- Lagerovanja Table -->
      <div class="table-container">
        <table class="lagerovanja-table">
          <thead>
            <tr>
              <th>Sirovo Vino</th>
              <th>Bure</th>
              <th>Podrum</th>
              <th>Materijal</th>
              <th>Zapremina</th>
              <th>Datum punjenja</th>
              <th>Datum pražnjenja</th>
              <th>Trajanje</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="lagerovanje in lagerovanja" :key="`${lagerovanje.sirovovinoIdsirvina}-${lagerovanje.bureIdbur}`">
              <td>
                <router-link 
                  :to="`/sirova-vina/${lagerovanje.sirovovinoIdsirvina}`"
                  class="link"
                >
                  {{ lagerovanje.nazivSirovogVina }}
                </router-link>
              </td>
              <td><strong>{{ lagerovanje.oznakaBureta }}</strong></td>
              <td>{{ lagerovanje.nazivPodruma }}</td>
              <td>{{ lagerovanje.materijalBureta }}</td>
              <td>{{ lagerovanje.zapreminaBureta }} L</td>
              <td>{{ formatDate(lagerovanje.datpunjenja) }}</td>
              <td>{{ lagerovanje.jeAktivno ? '-' : formatDate(lagerovanje.datpraznjenja) }}</td>
              <td><strong>{{ lagerovanje.brojDana }}</strong> dana</td>
              <td>
                <span v-if="lagerovanje.jeAktivno" class="status-badge aktivno">Aktivno</span>
                <span v-else class="status-badge zavrseno">Završeno</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import lagerovanjeService from '@/services/lagerovanjeService'
import type { LagerovanoVino } from '@/types/lagerovanje'

const lagerovanja = ref<LagerovanoVino[]>([])
const loading = ref(false)
const error = ref('')

const loadLagerovanja = async () => {
  loading.value = true
  error.value = ''
  try {
    lagerovanja.value = await lagerovanjeService.getSvaLagerovanja()
  } catch (err: any) {
    console.error('Greška pri učitavanju lagerovanja:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju lagerovanja'
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString: string) => {
  if (!dateString || dateString === '9999-12-31') return '-'
  const date = new Date(dateString)
  return date.toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

onMounted(() => {
  loadLagerovanja()
})
</script>

<style scoped>
.lagerovanja-container {
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
  margin-bottom: 20px;
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

.table-container {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.lagerovanja-table {
  width: 100%;
  border-collapse: collapse;
}

.lagerovanja-table thead {
  background: #000;
  color: #fff;
}

.lagerovanja-table th {
  padding: 16px;
  text-align: left;
  font-weight: 600;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.lagerovanja-table tbody tr {
  border-bottom: 1px solid #e0e0e0;
  transition: background 0.3s;
}

.lagerovanja-table tbody tr:hover {
  background: #f9f9f9;
}

.lagerovanja-table tbody tr:last-child {
  border-bottom: none;
}

.lagerovanja-table td {
  padding: 16px;
  font-size: 14px;
  color: #000;
}

.link {
  color: #1976d2;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s;
}

.link:hover {
  color: #0d47a1;
  text-decoration: underline;
}

.status-badge {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
}

.status-badge.aktivno {
  background: #4CAF50;
  color: #fff;
}

.status-badge.zavrseno {
  background: #9E9E9E;
  color: #fff;
}
</style>


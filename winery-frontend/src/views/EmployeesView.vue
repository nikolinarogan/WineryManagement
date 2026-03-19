<template>
  <div class="employees">
    <div class="employees-container">
      <h1>Zaposleni</h1>
      
      <!-- Filter -->
      <div class="filter-section">
        <div class="filter-group">
          <label for="kategorija-filter">Filtriraj po kategoriji:</label>
          <select id="kategorija-filter" v-model="selectedKategorija" @change="loadEmployees">
            <option value="">Sve kategorije</option>
            <option value="Menadzer">Menadžer</option>
            <option value="Enolog">Enolog</option>
            <option value="Radnik">Radnik</option>
            <option value="Somleijer">Somleijer</option>
          </select>
        </div>
        
        <div class="summary">
          <span class="count">Ukupno: <strong>{{ employees.length }}</strong></span>
        </div>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="loading">Učitavanje...</div>

      <!-- Error -->
      <div v-else-if="error" class="error-message">{{ error }}</div>

      <!-- Employees Table -->
      <div v-else-if="employees.length > 0" class="employees-table">
        <table>
          <thead>
            <tr>
              <th>Ime</th>
              <th>Prezime</th>
              <th>Email</th>
              <th>JMBG</th>
              <th>Kategorija</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="employee in employees" :key="employee.idzap">
              <td>{{ employee.ime }}</td>
              <td>{{ employee.prez }}</td>
              <td>{{ employee.email }}</td>
              <td>{{ employee.jmbg }}</td>
              <td>
                <span class="category-badge" :class="`category-${employee.kategorija.toLowerCase()}`">
                  {{ employee.kategorija }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- No Results -->
      <div v-else class="no-results">
        <p>Nema zaposlenih za prikaz.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import authService from '@/services/authService'
import type { Employee } from '@/types/auth'

const employees = ref<Employee[]>([])
const loading = ref(false)
const error = ref('')
const selectedKategorija = ref('')

const loadEmployees = async () => {
  try {
    loading.value = true
    error.value = ''
    employees.value = await authService.getAllEmployees(
      selectedKategorija.value || undefined
    )
  } catch (err: any) {
    console.error('Error loading employees:', err)
    error.value = err.response?.data?.message || 'Greška pri učitavanju zaposlenih'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadEmployees()
})
</script>

<style scoped>
.employees {
  width: 100%;
  min-height: calc(100vh - 64px);
  margin: 0;
  padding: 60px 40px;
  background: linear-gradient(135deg, #f5f5f5 0%, #e0e0e0 100%);
  overflow: auto;
}

.employees-container {
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  font-size: 48px;
  font-weight: 700;
  color: #000;
  margin: 0 0 40px 0;
}

.filter-section {
  background: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 12px;
}

.filter-group label {
  font-weight: 600;
  color: #000;
  font-size: 14px;
  white-space: nowrap;
}

.filter-group select {
  padding: 10px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
  min-width: 200px;
}

.filter-group select:focus {
  outline: none;
  border-color: #000;
}

.summary {
  font-size: 16px;
  color: #666;
}

.summary strong {
  color: #000;
  font-size: 18px;
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

.employees-table {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background: #000;
  color: white;
}

thead th {
  padding: 16px;
  text-align: left;
  font-weight: 600;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

tbody tr {
  border-bottom: 1px solid #e0e0e0;
  transition: background 0.2s;
}

tbody tr:hover {
  background: #f9f9f9;
}

tbody tr:last-child {
  border-bottom: none;
}

tbody td {
  padding: 16px;
  color: #333;
  font-size: 15px;
}

.category-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.category-menadzer {
  background: #000;
  color: white;
}

.category-enolog {
  background: #e8f5e9;
  color: #2e7d32;
  border: 1px solid #2e7d32;
}

.category-radnik {
  background: #e3f2fd;
  color: #1565c0;
  border: 1px solid #1565c0;
}

.category-somleijer {
  background: #fff3e0;
  color: #e65100;
  border: 1px solid #e65100;
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
  margin: 0;
}

@media (max-width: 768px) {
  .employees {
    padding: 40px 20px;
  }

  h1 {
    font-size: 36px;
  }

  .filter-section {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-group {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-group select {
    width: 100%;
  }

  .employees-table {
    overflow-x: auto;
  }

  table {
    min-width: 600px;
  }

  tbody td,
  thead th {
    padding: 12px;
    font-size: 14px;
  }
}
</style>

